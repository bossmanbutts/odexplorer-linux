// Functional in-memory JournalParserStore.
// Scans journal .log files, maps each JSON line to a typed JournalEntry and
// dispatches it to registered IProcessJournalLogs parsers. Mirrors the public
// surface of the real (excluded) store so the real one can be dropped in later.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EliteJournalReader;
using EliteJournalReader.Events;
using Newtonsoft.Json.Linq;
using ODExplorer.Database;
using ODExplorer.Journal;
using ODExplorer.Models;
using ODUtils.Database.Interfaces;
using ODUtils.Journal;
using ODUtils.Models;

namespace ODExplorer.Stores
{
    public sealed class JournalParserStore
    {
        public event EventHandler<string?>? OnJournalStoreStatusChange;
        public event EventHandler<bool>? OnParserStoreLive;
        public event EventHandler? OnCommandersUpdated;
        public event EventHandler<StatusFileEvent>? StatusUpdated;

        private readonly IOdToolsDatabaseProvider? databaseProvider;
        private readonly SettingsStore? settingsStore;
        private readonly List<IProcessJournalLogs> journalLogParserList = [];
        private readonly List<JournalCommander> _journalCommanders = [];
        private string? currentDirectory;

        public JournalParserStore(IOdToolsDatabaseProvider? databaseProvider = null,
                                  SettingsStore? settingsStore = null)
        {
            this.databaseProvider = databaseProvider;
            this.settingsStore = settingsStore;
        }

        private bool isLive;
        public bool IsLive
        {
            get => isLive;
            private set
            {
                if (isLive == value)
                    return;
                isLive = value;
                Raise(() => OnParserStoreLive?.Invoke(this, value));
            }
        }

        public bool Odyssey => true;

        public List<JournalCommander> JournalCommanders => _journalCommanders;

        public NavigationRoute? GetNavRoute() => null;

        public void RegisterParser(IProcessJournalLogs journalLogParser)
        {
            if (journalLogParserList.Contains(journalLogParser) == false)
            {
                journalLogParserList.Add(journalLogParser);
            }
        }

        public void UnregisterParser(IProcessJournalLogs journalLogParser)
        {
            journalLogParserList.Remove(journalLogParser);
        }

        public void ReadNewCommander(int commanderID)
        {
            _ = Task.Run(() => LoadCommanderAsync(commanderID));
        }

        public void ReadNewDirectory(string path)
        {
            _ = Task.Run(() => LoadDirectoryAsync(path));
        }

        public Task UpdateCommanders()
        {
            _journalCommanders.Clear();

            var commanders = databaseProvider?.GetAllJournalCommanders(true)
                .GetAwaiter().GetResult() ?? [];

            _journalCommanders.AddRange(commanders);

            if (settingsStore is not null &&
                settingsStore.SelectedCommanderID <= 0 &&
                _journalCommanders.Count != 0)
            {
                settingsStore.SelectedCommanderID = _journalCommanders[0].Id;
            }

            Raise(() => OnCommandersUpdated?.Invoke(this, EventArgs.Empty));
            return Task.CompletedTask;
        }

        public Task ResetDataBase(IOdToolsDatabaseProvider provider)
        {
            IsLive = false;

            if (provider is OdExplorerDatabaseProvider dbProvider)
            {
                dbProvider.ClearCommanders();
            }

            _journalCommanders.Clear();
            Raise(() => OnCommandersUpdated?.Invoke(this, EventArgs.Empty));
            return Task.CompletedTask;
        }

        private async Task LoadCommanderAsync(int commanderID)
        {
            if (databaseProvider is null || settingsStore is null)
                return;

            settingsStore.SelectedCommanderID = commanderID;

            var commander = databaseProvider.GetCommander(commanderID);
            if (commander is null || string.IsNullOrWhiteSpace(commander.JournalPath))
            {
                Raise(() => OnJournalStoreStatusChange?.Invoke(this, "No Commanders Found"));
                return;
            }

            currentDirectory = commander.JournalPath;
            await ParseDirectoryAsync(commander.JournalPath, commanderID, commander.Name);
        }

        private async Task LoadDirectoryAsync(string path)
        {
            if (databaseProvider is null || settingsStore is null)
                return;

            if (Directory.Exists(path) == false)
            {
                Raise(() => OnJournalStoreStatusChange?.Invoke(this, "No Commanders Found"));
                return;
            }

            var commanderName = FindCommanderName(path);
            if (string.IsNullOrWhiteSpace(commanderName))
            {
                Raise(() => OnJournalStoreStatusChange?.Invoke(this, "No Commanders Found"));
                return;
            }

            var existing = databaseProvider.GetAllJournalCommanders(true)
                .GetAwaiter().GetResult()
                .FirstOrDefault(x => string.Equals(x.Name, commanderName, StringComparison.OrdinalIgnoreCase));

            int id;
            if (existing is not null)
            {
                id = existing.Id;
                databaseProvider.AddCommander(new JournalCommander(id, existing.Name, path, existing.LastFile, existing.IsHidden));
            }
            else
            {
                var all = databaseProvider.GetAllJournalCommanders(true).GetAwaiter().GetResult();
                id = all.Count == 0 ? 1 : all.Max(x => x.Id) + 1;
                databaseProvider.AddCommander(new JournalCommander(id, commanderName, path, null, false));
            }

            settingsStore.SelectedCommanderID = id;
            currentDirectory = path;
            await ParseDirectoryAsync(path, id, commanderName);
        }

        private async Task ParseDirectoryAsync(string directory, int commanderId, string commanderName)
        {
            IsLive = false;

            foreach (var parser in journalLogParserList)
            {
                parser.ClearData();
            }
            foreach (var parser in journalLogParserList)
            {
                parser.RunBeforeParsingLogs(commanderId);
            }

            Raise(() => OnJournalStoreStatusChange?.Invoke(this, $"Processing History for CMDR {commanderName}"));

            try
            {
                string? lastFile = null;

                foreach (var file in Directory.EnumerateFiles(directory, "*.log", SearchOption.TopDirectoryOnly)
                             .OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                {
                    lastFile = Path.GetFileName(file);
                    await Task.Yield();

                    foreach (var line in File.ReadLines(file))
                    {
                        var entry = JournalEventMapper.Map(line, Path.GetFileName(file), commanderId);
                        if (entry is null)
                            continue;

                        foreach (var parser in journalLogParserList)
                        {
                            parser.ParseJournalEvent(entry);
                        }
                    }

                    UpdateCommanderLastFile(commanderId, lastFile);
                }

                if (databaseProvider is not null && lastFile is not null)
                {
                    var commander = databaseProvider.GetCommander(commanderId);
                    if (commander is not null)
                    {
                        databaseProvider.AddCommander(new JournalCommander(commanderId, commander.Name,
                            commander.JournalPath, lastFile, commander.IsHidden));
                    }
                }
            }
            catch (Exception ex)
            {
                App.Logger.Error(ex, "Exception parsing journal directory");
            }

            await UpdateCommanders();

            Raise(() => OnJournalStoreStatusChange?.Invoke(this, "Completed"));

            IsLive = true;
        }

        private void UpdateCommanderLastFile(int commanderId, string lastFile)
        {
            var commander = _journalCommanders.FirstOrDefault(x => x.Id == commanderId);
            if (commander is not null)
            {
                commander.LastFile = lastFile;
            }
        }

        private static string? FindCommanderName(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*.log", SearchOption.TopDirectoryOnly)
                         .OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                foreach (var line in File.ReadLines(file))
                {
                    try
                    {
                        var obj = JObject.Parse(line);
                        if (string.Equals(obj["event"]?.ToString(), "LoadGame", StringComparison.OrdinalIgnoreCase))
                        {
                            var name = obj["Commander"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                return name;
                            }
                        }
                    }
                    catch
                    {
                        // Not a JSON journal line; skip.
                    }
                }
            }

            return null;
        }

        private void Raise(Action action)
        {
            DispatcherHelper.Invoke(action);
        }
    }
}
