using EliteJournalReader;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ODExplorer.Database.DTOs;
using ODExplorer.Models;
using ODUtils.Database.DTOs;
using ODUtils.Database.Interfaces;
using ODUtils.Journal;
using ODUtils.Spansh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODExplorer.Database
{
    public class OdExplorerDatabaseProvider(IOdExplorerDBContextFactory contextFactory) : IOdToolsDatabaseProvider
    {
        private readonly IOdExplorerDBContextFactory _contextFactory = contextFactory;

        #region Commander Methods
        public virtual async Task<List<JournalCommander>> GetAllJournalCommanders(bool includeHidden = false)
        {
            using var context = _contextFactory.CreateDbContext();

            if (!context.JournalCommanders.Any())
                return [];

            if (includeHidden)
            {
                var allCmdrs = await context.JournalCommanders
                    .Select(x => new JournalCommander(x.Id, x.Name, x.JournalDir, x.LastFile, x.IsHidden))
                    .ToListAsync();

                var reslt = allCmdrs.OrderBy(x => x.Name.Contains("(Legacy)"))
                                    .ThenBy(x => x.Name);
                return [.. reslt];
            }

            var cmdrs = await context.JournalCommanders
                .Where(x => x.IsHidden == false)
                .Select(x => new JournalCommander(x.Id, x.Name, x.JournalDir, x.LastFile, x.IsHidden))
                .ToListAsync();

            var ret = cmdrs.OrderBy(x => x.Name.Contains("(Legacy)"))
                            .ThenBy(x => x.Name);

            await context.Database.CloseConnectionAsync();
            return [.. ret];
        }

        public virtual JournalCommander AddCommander(JournalCommander cmdr)
        {
            using var context = _contextFactory.CreateDbContext();

            var known = context.JournalCommanders.FirstOrDefault(x => x.Name == cmdr.Name);

            if (known == null)
            {
                known = new JournalCommanderDTO()
                {
                    Name = cmdr.Name,
                    LastFile = cmdr.LastFile ?? string.Empty,
                    JournalDir = cmdr.JournalPath ?? string.Empty,
                    IsHidden = cmdr.IsHidden
                };
                context.JournalCommanders.Add(known);
                context.SaveChanges();
                return new(known.Id, known.Name, known.JournalDir, known.LastFile, known.IsHidden);
            }

            known.LastFile = cmdr.LastFile ?? string.Empty;
            known.JournalDir = cmdr.JournalPath ?? string.Empty;
            known.Name = cmdr.Name;
            known.IsHidden = cmdr.IsHidden;
            context.SaveChanges();
            context.Database.CloseConnection();
            return new(known.Id, known.Name, known.JournalDir, known.LastFile, known.IsHidden);
        }

        public virtual JournalCommander? GetCommander(int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var known = context.JournalCommanders.FirstOrDefault(x => x.Id == cmdrId);

            if (known == null)
            {
                return null;
            }

            return new(known.Id, known.Name, known.JournalDir, known.LastFile, known.IsHidden);
        }

        public HashSet<string> GetAllReadFilenames()
        {
            using var context = _contextFactory.CreateDbContext();

            var entries = context.JournalEntries
                .Select(x => x.Filename)
                .Distinct()
                .ToHashSet();

            return entries;
        }
        #endregion

        #region Journal Methods
        public virtual void AddJournalEntries(List<JournalEntry> journalEntries)
        {
            var entriesToAdd = journalEntries
                .OrderBy(x => x.Filename)
                .ThenBy(x => x.Offset)
                .Select(x => new JournalEntryDTO()
                {
                    CommanderID = x.CommanderID,
                    EventData = x.OriginalEvent?.ToString(Formatting.None) ?? string.Empty,
                    EventTypeId = (int)x.EventType,
                    Filename = x.Filename,
                    Offset = x.Offset,
                    TimeStamp = x.TimeStamp,
                }
                ).ToArray();

            using var context = _contextFactory.CreateDbContext();

            foreach (var batch in entriesToAdd.Chunk(1000))
            {
                var filenames = batch.Select(x => x.Filename).Distinct().ToArray();
                var existingKeys = context.JournalEntries
                    .Where(x => filenames.Contains(x.Filename))
                    .Select(x => new { x.Filename, x.Offset })
                    .AsEnumerable()
                    .Select(x => (x.Filename, x.Offset))
                    .ToHashSet();

                foreach (var entry in batch)
                {
                    var current = context.JournalEntries.Find(entry.Filename, entry.Offset);
                    if (current is null)
                    {
                        context.JournalEntries.Add(entry);
                    }
                    else
                    {
                        current.TimeStamp = entry.TimeStamp;
                        current.CommanderID = entry.CommanderID;
                        current.EventTypeId = entry.EventTypeId;
                        current.EventData = entry.EventData;
                    }
                }

                context.SaveChanges();
            }
        }

        public virtual async Task<List<JournalEntry>> GetAllJournalEntries(int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var ret = await context.JournalEntries
                .Where(x => x.CommanderID == cmdrId)
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset)
                .Select(x => new JournalEntry(
                    x.Filename,
                    x.Offset,
                    x.CommanderID,
                    (JournalTypeEnum)x.EventTypeId,
                    JournalWatcher.GetEventData(x.EventData),
                    null))
                .ToListAsync();

            return ret;
        }

        public virtual long GetMaxJournalOffset(int cmdrId, string filename)
        {
            using var context = _contextFactory.CreateDbContext();

            return context.JournalEntries
                .Where(x => x.CommanderID == cmdrId && x.Filename == filename)
                .Select(x => (long?)x.Offset)
                .Max() ?? 0;
        }

        public virtual async Task<List<JournalEntry>> GetJournalEntriesOfType(int cmdrId, List<JournalTypeEnum> types)
        {
            return await GetJournalEntriesOfType(cmdrId, types, DateTime.MinValue);
        }

        public async Task GetJournalsStream(int cmdrId, List<JournalTypeEnum> types, DateTime age, Func<JournalEntry, Task> callBack)
        {
            using var context = _contextFactory.CreateDbContext();
            var journals = context.JournalEntries.Where(x => x.CommanderID == cmdrId
                            && x.TimeStamp.Date >= age.Date
                            && types.Contains((JournalTypeEnum)x.EventTypeId))
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset)
                .AsNoTrackingWithIdentityResolution();
            await StreamJournals(journals, callBack);
        }

        private static async Task StreamJournals(IQueryable<JournalEntryDTO> journals, Func<JournalEntry, Task> callBack)
        {
            foreach (var x in journals)
            {
                var entry = new JournalEntry(
                    x.Filename,
                    x.Offset,
                    x.CommanderID,
                    (JournalTypeEnum)x.EventTypeId,
                    JournalWatcher.GetEventData(x.EventData),
                    null);

                await callBack(entry);
            }
        }

        public virtual async Task<List<JournalEntry>> GetJournalEntriesOfType(int cmdrId, List<JournalTypeEnum> types, DateTime age)
        {
            if (types.Contains(JournalTypeEnum.Fileheader) == false)
                types.Add(JournalTypeEnum.Fileheader);

            using var context = _contextFactory.CreateDbContext();

            var ret = await context.JournalEntries
                .Where(x => x.CommanderID == cmdrId
                            && x.TimeStamp.Date >= age.Date
                            && types.Contains((JournalTypeEnum)x.EventTypeId))
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset)
                .Select(x => new JournalEntry(
                    x.Filename,
                    x.Offset,
                    x.CommanderID,
                    (JournalTypeEnum)x.EventTypeId,
                    JournalWatcher.GetEventData(x.EventData),
                    null))
                .ToListAsync();

            var firstHeader = ret.FirstOrDefault(x => x.EventType == JournalTypeEnum.Fileheader);

            if (firstHeader != null)
            {
                var index = ret.IndexOf(firstHeader);

                if (index > 0)
                {
                    ret.RemoveRange(0, index);
                }
            }

            return ret;
        }

        public virtual Task ParseJournalEventsOfType(int cmdrId, List<JournalTypeEnum> types, Action<JournalEntry> callback, DateTime age)
        {
            using var context = _contextFactory.CreateDbContext();

            var entries = context.JournalEntries
                .Where(x => x.TimeStamp >= age && cmdrId == x.CommanderID)
                .EventTypeCompare(types)
                .OrderBy(x => x.TimeStamp)
                .ThenBy(x => x.Offset);

            foreach (var e in entries)
            {
                callback.Invoke(new JournalEntry(
                    e.Filename,
                    e.Offset,
                    e.CommanderID,
                    (JournalTypeEnum)e.EventTypeId,
                    JournalWatcher.GetEventData(e.EventData),
                    null));
            }
            return Task.CompletedTask;
        }
        #endregion

        #region IgnoredSystems Methods
        public Task AddIgnoreSystem(long address, string name, int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var cmdr = context.JournalCommanders.First(x => x.Id == cmdrId);

            var system = context.CartoIgnoredSystems.Include(x => x.Commanders).FirstOrDefault(x => x.Address == address);

            if (system == null)
            {
                context.CartoIgnoredSystems.Add(new CartoIgnoredSystemsDTO
                {
                    Address = address,
                    Name = name,
                    Commanders = [cmdr]
                });
                context.SaveChanges(true);
                return Task.CompletedTask;
            }
            if (system.Commanders.Contains(cmdr) == false)
            {
                system.Commanders.Add(cmdr);
                context.SaveChanges(true);
            }
            return Task.CompletedTask;
        }

        public Task RemoveIgnoreSystem(long address, int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var cmdr = context.JournalCommanders.First(x => x.Id == cmdrId);

            var system = context.CartoIgnoredSystems.Include(x => x.Commanders).FirstOrDefault(x => x.Address == address && x.Commanders.Contains(cmdr));

            if (system != null)
            {
                system.Commanders.Remove(cmdr);

                if (system.Commanders.Count == 0)
                {
                    context.CartoIgnoredSystems.Remove(system);
                }

                context.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Dictionary<long, string> GetIgnoredSystemsDictionary(int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var cmdr = context.JournalCommanders.FirstOrDefault(x => x.Id == cmdrId);

            if (cmdr == null)
            {
                return [];
            }
            var systems = context.CartoIgnoredSystems
                .Include(x => x.Commanders)
                .Where(x => x.Commanders.Contains(cmdr));

            return systems.ToDictionary(x => x.Address, x => x.Name);
        }

        public List<IgnoredSystem> GetIgnoredSystems(int cmdrId)
        {
            using var context = _contextFactory.CreateDbContext();

            var cmdr = context.JournalCommanders.FirstOrDefault(x => x.Id == cmdrId);

            if (cmdr == null)
            {
                return [];
            }

            var ret = context.CartoIgnoredSystems
                .Include(x => x.Commanders)
                .Where(x => x.Commanders.Contains(cmdr))
                .OrderBy(x => x.Name)
                .Select(x => new IgnoredSystem(
                    x.Address,
                    x.Name,
                    cmdrId))
                    .ToList();

            return ret;
        }
        #endregion

        #region EdAstro
        public void AddEdAstroPois(List<EdAstroPoi> data)
        {
            var entriesToAdd = data
                .Select(x => new EdAstroPoiDTO()
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    GalMapName = x.GalMapName ?? string.Empty,
                    SystemAddress = x.SystemAddress,
                    X = x.SystemPosition.X,
                    Y = x.SystemPosition.Y,
                    Z = x.SystemPosition.Z,
                    Type = (int)x.Type,
                    Type2 = (int)x.Type2,
                    Summary = x.Summary ?? string.Empty,
                    DistanceFromSol = x.DistanceFromSol,
                    PoiUrl = x.PoiUrl?.OriginalString ?? string.Empty,
                    MarkDown = x.MarkDown ?? string.Empty
                }
                );


            using var context = _contextFactory.CreateDbContext();

            foreach (var entry in entriesToAdd)
            {
                var current = context.EdAstroPois.Find(entry.Id);
                if (current is null)
                {
                    context.EdAstroPois.Add(entry);
                }
                else
                {
                    current.Name = entry.Name;
                    current.GalMapName = entry.GalMapName;
                    current.SystemAddress = entry.SystemAddress;
                    current.X = entry.X;
                    current.Y = entry.Y;
                    current.Z = entry.Z;
                    current.Type = entry.Type;
                    current.Type2 = entry.Type2;
                    current.Summary = entry.Summary;
                    current.DistanceFromSol = entry.DistanceFromSol;
                    current.PoiUrl = entry.PoiUrl;
                    current.MarkDown = entry.MarkDown;
                }
            }

            context.SaveChanges();
        }

        public async Task<List<EdAstroPoi>> GetAstroPoisAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var pio = context.EdAstroPois.Select(x => new EdAstroPoi(x));

            return await pio.ToListAsync();
        }

        public List<EdAstroPoi> GetAstroPois()
        {
            using var context = _contextFactory.CreateDbContext();

            var pio = context.EdAstroPois.Select(x => new EdAstroPoi(x));

            return [.. pio];
        }
        #endregion

        #region Settings
        public List<SettingsDTO> GetAllSettings()
        {
            using var context = _contextFactory.CreateDbContext();

            return [.. context.Settings];
        }

        public void AddSettings(List<SettingsDTO> settings)
        {
            using var context = _contextFactory.CreateDbContext();

            foreach (var setting in settings)
            {
                UpsertSetting(context, setting);
            }

            context.SaveChanges();

            context.Database.CloseConnection();
        }

        public void AddSetting(SettingsDTO settings)
        {
            using var context = _contextFactory.CreateDbContext();

            UpsertSetting(context, settings);

            context.SaveChanges();
        }

        private static void UpsertSetting(DbContext context, SettingsDTO setting)
        {
            var current = context.Set<SettingsDTO>().Find(setting.Id);
            if (current is null)
            {
                context.Set<SettingsDTO>().Add(setting);
            }
            else
            {
                current.IntValue = setting.IntValue;
                current.DoubleValue = setting.DoubleValue;
                current.StringValue = setting.StringValue;
            }
        }
        #endregion

        #region Database Methods
        public Task ResetDataBaseAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            context.Database.ExecuteSqlRaw(
                "DELETE FROM CommanderIgnoredSystems;" +
                "DELETE FROM CartoIgnoredSystems;" +
                "DELETE FROM JournalEntries;" +
                "DELETE FROM Settings;" +
                "DELETE FROM JournalCommanders;" +
                "DELETE FROM SQLITE_SEQUENCE WHERE name IN ('JournalCommanders','JournalEntries');");

            return Task.CompletedTask;
        }
        #endregion

        internal Dictionary<CsvType, SpanshCsvContainer> GetSpanshCSVsDictionary(int commanderID)
        {
            using var context = _contextFactory.CreateDbContext();

            var dict = context.SpanshCsvs
                .Where(x => x.CommanderID == commanderID)
                .ToDictionary(x => (CsvType)x.CsvType, y => JsonConvert.DeserializeObject<SpanshCsvContainer>(y.Json));

            var ret = new Dictionary<CsvType, SpanshCsvContainer>();

            foreach (var kvp in dict)
            {
                if (kvp.Value is not null)
                {
                    ret.Add(kvp.Key, kvp.Value);
                }
            }
            return ret;
        }

        internal void SaveCVSs(Dictionary<CsvType, SpanshCsvContainer> csvs, int commanderID)
        {
            List<SpanshCsvDTO> csvList = [];

            foreach (var kvp in csvs)
            {
                if (kvp.Value.HasValue == false)
                    continue;

                var json = JsonConvert.SerializeObject(kvp.Value, Formatting.None);

                if (string.IsNullOrEmpty(json))
                    continue;

                csvList.Add(new() { CsvType = (int)kvp.Key, CommanderID = commanderID, Json = json });
            }

            using var context = _contextFactory.CreateDbContext();

            foreach (var csv in csvList)
            {
                var current = context.SpanshCsvs.Find(csv.CsvType, csv.CommanderID);
                if (current is null)
                {
                    context.SpanshCsvs.Add(csv);
                }
                else
                {
                    current.Json = csv.Json;
                }
            }

            context.SaveChanges();
        }

        internal async Task DeleteCommander(int commanderID)
        {
            using var context = _contextFactory.CreateDbContext();

            var cmdr = context.JournalCommanders.FirstOrDefault(x => x.Id == commanderID);

            if (cmdr == null)
            {
                return;
            }

            await context.JournalEntries.Where(x => x.CommanderID == commanderID).ExecuteDeleteAsync();

            await context.SpanshCsvs.Where(x => x.CommanderID == commanderID).ExecuteDeleteAsync();

            var ignoredSystems = context.CartoIgnoredSystems
                                             .Include(x => x.Commanders)
                                             .Where(x => x.Commanders.Contains(cmdr));

            foreach (var system in ignoredSystems)
            {
                if (system is null)
                    continue;

                system.Commanders.Remove(cmdr);

                if (system.Commanders.Count == 0)
                    context.CartoIgnoredSystems.Remove(system);
            }

            context.JournalCommanders.Remove(cmdr);

            await context.SaveChangesAsync(true);
        }

        internal async Task VacuumDatabase()
        {
            using var context = _contextFactory.CreateDbContext();

            await context.Database.ExecuteSqlRawAsync("VACUUM;");
        }
    }
}
