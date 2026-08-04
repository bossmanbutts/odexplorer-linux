// Stub implementations for ODExplorer store classes that are excluded from the core build.
// These allow ViewModels to compile without the full store implementations.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ODExplorer.Database;
using ODExplorer.Models;
using ODExplorer.ViewModels.ModelVMs;
using ODUtils.Database.Interfaces;
using ODUtils.Journal;
using ODUtils.Models;
using ODUtils.Spansh;

namespace ODExplorer.Stores
{
    // ─── JournalParserStore ────────────────────────────────────────────────────
    public sealed class JournalParserStore
    {
        public event EventHandler<string?>? OnJournalStoreStatusChange;
        public event EventHandler<bool>? OnParserStoreLive;
        public event EventHandler? OnCommandersUpdated;

        private readonly IOdToolsDatabaseProvider? databaseProvider;
        private readonly SettingsStore? settingsStore;

        public JournalParserStore(IOdToolsDatabaseProvider? databaseProvider = null,
                                  SettingsStore? settingsStore = null)
        {
            this.databaseProvider = databaseProvider;
            this.settingsStore = settingsStore;
        }

        // IsLive=true so the shell's UiEnabled gate is unlocked for UI porting/testing.
        public bool IsLive { get; } = true;
        public List<JournalCommander> JournalCommanders { get; } = new();

        /// <summary>Selects a commander and reports the current status to the Loading view.</summary>
        public Task ReadNewCommander(int id)
        {
            if (settingsStore is not null)
            {
                settingsStore.SelectedCommanderID = id;
            }

            var commander = databaseProvider?.GetAllJournalCommanders(true)
                .GetAwaiter().GetResult()
                .FirstOrDefault(x => x.Id == id);

            OnJournalStoreStatusChange?.Invoke(this,
                commander is null ? "No Commanders Found" : $"Ready — Selected Commander {commander.Name}");

            return Task.CompletedTask;
        }

        /// <summary>Refreshes the commander list from the provider and selects the first if needed.</summary>
        public Task UpdateCommanders()
        {
            JournalCommanders.Clear();

            var commanders = databaseProvider?.GetAllJournalCommanders(true)
                .GetAwaiter().GetResult() ?? [];

            JournalCommanders.AddRange(commanders);

            if (settingsStore is not null &&
                settingsStore.SelectedCommanderID <= 0 &&
                JournalCommanders.Count != 0)
            {
                settingsStore.SelectedCommanderID = JournalCommanders[0].Id;
            }

            OnCommandersUpdated?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        /// <summary>Clears all commander data in the provider.</summary>
        public Task ResetDataBase(IOdToolsDatabaseProvider provider)
        {
            if (provider is OdExplorerDatabaseProvider dbProvider)
            {
                dbProvider.ClearCommanders();
            }

            JournalCommanders.Clear();
            OnCommandersUpdated?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scans a journal directory, extracts the commander name from the LoadGame
        /// event, and registers/updates the commander in the provider.
        /// </summary>
        public void ReadNewDirectory(string path)
        {
            if (databaseProvider is null || settingsStore is null)
            {
                return;
            }

            var commanderName = FindCommanderName(path);
            if (string.IsNullOrWhiteSpace(commanderName))
            {
                OnJournalStoreStatusChange?.Invoke(this, "No Commander Found in the selected folder");
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
            UpdateCommanders();
            OnJournalStoreStatusChange?.Invoke(this, $"Registered Commander {commanderName}");
        }

        private static string? FindCommanderName(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*.log", SearchOption.TopDirectoryOnly)
                         .OrderBy(x => x))
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
    }

    // ─── ExplorationDataStore ──────────────────────────────────────────────────
    public sealed class ExplorationDataStore
    {
        public event EventHandler<StarSystem?>? OnCurrentSystemUpdated;
        public event EventHandler<string>? OnFSDJump;
        public event EventHandler? OnCartoDataSold;
        public event EventHandler? OnCartoDataLost;
        public event EventHandler? OnBioDataSold;
        public event EventHandler<OrganicScanItem>? OnBioDataUpdated;
        public event EventHandler? OnBioDataLost;
        public event EventHandler<SystemBody>? OnBodyUpdated;
        public event EventHandler<List<StarSystem>>? OnRouteUpdated;
        public event EventHandler<StarSystem>? OnSystemUpdatedFromEDSM;
        public event EventHandler<StarSystem?>? OnAllBodiesDiscovered;
        public event EventHandler<SystemBody>? OnBodyBiosUpdated;
        public event EventHandler<SystemBody>? OnBodyTargeted;

        public StarSystem? CurrentSystem { get; } = null;
        public string? CurrentSystemName { get; } = null;
        public string CurrentSystemRegion { get; } = string.Empty;
        public List<ODExplorer.Models.EdAstroPoi> EdAstroPois { get; } = new();
        public List<SystemBody> OrganicScanItems { get; } = new();
        public List<StarSystem> Route { get; } = new();
        public long SelectedBodyId { get; set; }

        public string GetUnsoldCartoValueString() => string.Empty;
        public string GetUnsoldExoValueString() => string.Empty;
        public void PopulateIgnoredSystems(int commanderId) { }
        public IEnumerable<StarSystem> GetUnsoldCartoSystems() => Array.Empty<StarSystem>();
        public IEnumerable<StarSystem> GetSoldCartoSystems() => Array.Empty<StarSystem>();
        public IEnumerable<StarSystem> GetLostCartoSystems() => Array.Empty<StarSystem>();
    }

    // ─── OrganicCheckListDataStore ─────────────────────────────────────────────
    public sealed class OrganicCheckListDataStore
    {
        public event EventHandler? OnOrganicScanDetailsUpdated;
        public event EventHandler<string>? OnSpeciesUpdated;

        // Species checklist keyed by genus codex, matching the real store
        public Dictionary<string, List<OrganicChecklistItem>> OrganicScanItems { get; } = new();

        public OrganicCheckListDataStore(JournalParserStore parserStore,
                                          ODUtils.Exobiology.ExoData exoData,
                                          SettingsStore settings)
        {
            // Pre-populate every genus key the OrganicViewModel indexers hit, so
            // navigation to the Exobiology view doesn't throw KeyNotFoundException.
            foreach (var key in new[]
            {
                "$Codex_Ent_Aleoids_Genus_Name;",
                "$Codex_Ent_Bacterial_Genus_Name;",
                "$Codex_Ent_Cactoid_Genus_Name;",
                "$Codex_Ent_Clypeus_Genus_Name;",
                "$Codex_Ent_Conchas_Genus_Name;",
                "$Codex_Ent_Electricae_Genus_Name;",
                "$Codex_Ent_Fonticulus_Genus_Name;",
                "$Codex_Ent_Fumerolas_Genus_Name;",
                "$Codex_Ent_Fungoids_Genus_Name;",
                "$Codex_Ent_Osseus_Genus_Name;",
                "$Codex_Ent_Recepta_Genus_Name;",
                "$Codex_Ent_Shrubs_Genus_Name;",
                "$Codex_Ent_Stratum_Genus_Name;",
                "$Codex_Ent_Tubus_Genus_Name;",
                "$Codex_Ent_Tussocks_Genus_Name;",
                "Other"
            })
            {
                OrganicScanItems[key] = new List<OrganicChecklistItem>();
            }
        }
    }

    // ─── SpanshCsvStore ───────────────────────────────────────────────────────
    public sealed class SpanshCsvStore
    {
        public event EventHandler<ExplorationTarget?>? OnCurrentTargetChanged;
        public event EventHandler<SpanshCsvContainer?>? OnCurrentContainerChanged;
        public event EventHandler<bool>? OnCarrierTimerRunning;
        public event EventHandler<string>? OnCarrierTimeTick;

        public int CurrentIndex { get; set; } = 0;
        public bool CarrierTimerRunning { get; } = false;
        public SpanshCsvContainer? CurrentContainer { get; } = null;
        public ExplorationTarget? CurrentTarget { get; } = null;
        public ExplorationTarget? NextTarget { get; } = null;

        public SpanshCsvContainer? GetCurrentContainer(CsvType csvType) => null;
        public bool ParseCSV(string fileName) => false;
        public bool ForceParseCSV(string fileName, CsvType csvType) => false;
        public void SaveCSVs() { }
        public void StartFleetCarrierTimer() { }
        public void StopFleetCarrierTimer() { }
    }
}
