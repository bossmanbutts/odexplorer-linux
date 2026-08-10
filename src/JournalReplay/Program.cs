using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NLog;
using ODExplorer.Database;
using ODExplorer.Journal;
using ODExplorer.Stores;
using ODUtils.APis;
using ODUtils.Exobiology;

// Headless real-journal validation: replays a live game's journal directory
// through the exact in-memory pipeline the app uses, then reports mapping and
// store-state sanity. Exit code 0 = all checks passed.
//
// Usage: JournalReplay --dir <journal-directory> [--db <sqlite-file>]

class Program
{
    static int Main(string[] args)
    {
        try
        {
            string dir = Arg(args, "--dir") ?? "";
            if (string.IsNullOrWhiteSpace(dir) || Directory.Exists(dir) == false)
            {
                Console.Error.WriteLine("usage: JournalReplay --dir <journal-directory> [--db <sqlite-file>]");
                return 2;
            }

            // Surface store-internal swallowed exceptions (App.Logger) on the console.
            var logConfig = new NLog.Config.LoggingConfiguration();
            logConfig.AddRule(LogLevel.Warn, LogLevel.Fatal, new NLog.Targets.ConsoleTarget("console"));
            LogManager.Configuration = logConfig;

            var dbFile = Arg(args, "--db") ?? Path.Combine(Path.GetTempPath(), "odex_replay.db");
            if (File.Exists(dbFile)) File.Delete(dbFile);

            var dbFactory = new OdExplorerDbContextFactory($"Data Source={dbFile}");
            using (var migrationContext = dbFactory.CreateDbContext())
            {
                migrationContext.Database.Migrate();
            }

            var db = new OdExplorerDatabaseProvider(dbFactory);
            var settings = new SettingsStore(db);
            var notifications = new NotificationStore(settings);
            var exo = new ExoData();

            var parser = new JournalParserStore(db, settings);
            var checklist = new OrganicCheckListDataStore(parser, exo, settings, registerWithParser: false);
            var exploration = new ExplorationDataStore(parser, new OfflineEdsmApiService(), db, notifications, settings, exo, checklist);
            parser.RegisterParser(checklist);

            int currentSystemEvents = 0;
            exploration.OnCurrentSystemUpdated += (_, s) => { if (s is not null) Interlocked.Increment(ref currentSystemEvents); };

            // ── Pass 1: independent pre-scan — every line the store will feed through
            //    JournalEventMapper, bucketed so a real parser gap is visible. ──
            var files = Directory.GetFiles(dir, "*.log")
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();

            int rawLines = 0, blankLines = 0;
            var mappedByName = new SortedDictionary<string, int>();
            var unmappedByName = new SortedDictionary<string, int>();
            var unmappedSamples = new List<string>();

            foreach (var file in files)
            {
                foreach (var raw in File.ReadAllText(file).Split('\n'))
                {
                    if (raw.Length == 0) { blankLines++; continue; }
                    rawLines++;

                    var entry = JournalEventMapper.Map(raw, Path.GetFileName(file), 0);
                    var eventName = EventNameOf(raw);
                    var bucket = entry is not null ? mappedByName : unmappedByName;
                    bucket[eventName] = bucket.TryGetValue(eventName, out var n) ? n + 1 : 1;
                    if (entry is null && unmappedSamples.Count < 5)
                        unmappedSamples.Add(raw.TrimEnd('\r', '\n'));
                }
            }

            // ── Pass 2: real pipeline run (history parse → live). ──
            parser.ReadNewDirectory(dir);

            var deadline = DateTime.UtcNow.AddSeconds(60);
            while (!parser.IsLive && DateTime.UtcNow < deadline) Thread.Sleep(50);

            if (!parser.IsLive)
            {
                Console.Error.WriteLine("FAIL: parser never became live");
                return 1;
            }
            Thread.Sleep(1500); // let the watcher idle once

            int failures = 0;
            void Check(string name, bool ok)
            {
                Console.WriteLine($"{(ok ? "PASS" : "FAIL")}: {name}");
                if (!ok) failures++;
            }

            var commander = parser.JournalCommanders.FirstOrDefault();
            Check("commander identified", commander is { Name.Length: > 0 });
            Check("journal files found", files.Length > 0);
            Check("raw journal lines read", rawLines > 0);

            var mustMap = new HashSet<string>(MustMapEvents);
            var mustMapUnmapped = unmappedByName.Where(kv => mustMap.Contains(kv.Key)).ToList();
            Check("no pipeline events left unmapped", mustMapUnmapped.Count == 0);

            Console.WriteLine();
            Console.WriteLine($"files: {files.Length}, raw lines: {rawLines}, blank lines: {blankLines}");
            Console.WriteLine($"mapped: {mappedByName.Values.Sum()}, unmapped: {unmappedByName.Values.Sum()}");
            Console.WriteLine();
            Console.WriteLine("mapped by event:");
            foreach (var kv in mappedByName) Console.WriteLine($"  {kv.Key,-24} {kv.Value}");
            Console.WriteLine("unmapped by event:");
            foreach (var kv in unmappedByName) Console.WriteLine($"  {kv.Key,-24} {kv.Value}");
            if (unmappedSamples.Count != 0)
            {
                Console.WriteLine("unmapped samples:");
                foreach (var s in unmappedSamples) Console.WriteLine($"  {s}");
            }

            Check("pipeline Scan events parsed", mappedByName.TryGetValue("Scan", out var scans) && scans > 0);
            Check("pipeline FSDJump events parsed", mappedByName.TryGetValue("FSDJump", out var jumps) && jumps > 0);
            Check("pipeline FSSBodySignals parsed", mappedByName.TryGetValue("FSSBodySignals", out var fss) && fss > 0);
            Check("pipeline ScanOrganic parsed", mappedByName.TryGetValue("ScanOrganic", out var org) && org > 0);
            Check("pipeline CodexEntry parsed", mappedByName.TryGetValue("CodexEntry", out var codex) && codex > 0);

            Check("current system set from real FSDJump", exploration.CurrentSystem is { Name.Length: > 0 });
            Console.WriteLine($"  current system: {exploration.CurrentSystemName ?? "(null)"}  region: {exploration.CurrentSystemRegion ?? "(null)"}");
            Check("current system event raised", currentSystemEvents >= 1);
            Check("organic bodies recorded", exploration.OrganicScanItems.Count > 0);
            Console.WriteLine($"  organic bodies: {exploration.OrganicScanItems.Count}, route systems: {exploration.Route.Count}");

            var scannedSpecies = checklist.OrganicScanItems.Values
                .SelectMany(items => items)
                .Where(i => i.Region.Values.Any(s => s is ODUtils.Models.OrganicScanState.Discovered
                    or ODUtils.Models.OrganicScanState.Analysed
                    or ODUtils.Models.OrganicScanState.Sold))
                .ToList();
            var soldSpecies = scannedSpecies.Count(i => i.Region.Values.Contains(ODUtils.Models.OrganicScanState.Sold));
            Check("scan organic recorded in checklist", scannedSpecies.Count > 0);
            Console.WriteLine($"  scanned species in checklist: {scannedSpecies.Count}, sold: {soldSpecies}");

            // DB persistence parity: every mapped history line must be stored.
            int dbEntries = db.GetAllJournalEntries(commander?.Id ?? 0).GetAwaiter().GetResult().Count;
            Check("DB journal entries match mapped lines", dbEntries == mappedByName.Values.Sum());
            Console.WriteLine($"  DB journal entries: {dbEntries}");

            Console.WriteLine();
            if (failures == 0) { Console.WriteLine("ALL REPLAY CHECKS PASSED"); return 0; }
            Console.WriteLine($"{failures} REPLAY CHECK(S) FAILED");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"UNEXPECTED: {ex}");
            return 2;
        }
    }

    static string? Arg(string[] args, string name)
    {
        for (int i = 0; i + 1 < args.Length; i++)
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        return null;
    }

    static string EventNameOf(string line)
    {
        try
        {
            var obj = JObject.Parse(line);
            var n = obj["event"]?.ToString();
            return string.IsNullOrEmpty(n) ? "(no event)" : n;
        }
        catch { return "(malformed json)"; }
    }

    // Events the stores depend on; any of these left unmapped is a parser gap.
    static readonly string[] MustMapEvents =
    [
        "Fileheader", "LoadGame", "Location", "FSDJump", "CarrierJump", "CarrierStats",
        "CarrierLocation", "CarrierJumpRequest", "CarrierJumpCancelled", "StartJump", "FSDTarget",
        "NavRoute", "NavRouteClear", "SupercruiseEntry", "FSSDiscoveryScan", "FSSAllBodiesFound",
        "FSSBodySignals", "ScanBaryCentre", "Scan", "SAAScanComplete", "SAASignalsFound",
        "SellExplorationData", "MultiSellExplorationData", "Disembark", "Embark", "Died",
        "ApproachBody", "CodexEntry", "ScanOrganic", "SellOrganicData"
    ];

    // Offline EDSM: keeps the replay deterministic and network-free.
    sealed class OfflineEdsmApiService : EdsmApiService
    {
        public override System.Threading.Tasks.Task<ODUtils.Models.StarType> GetPrimaryStarClassAsync(string systemName)
            => System.Threading.Tasks.Task.FromResult(ODUtils.Models.StarType.Unknown);

        public override System.Threading.Tasks.Task<EdsmSystemValue?> GetSystemValueAsync(string systemName)
            => System.Threading.Tasks.Task.FromResult<EdsmSystemValue?>(null);

        public override System.Threading.Tasks.Task<(int Count, int Scanned)> GetBodyCountAsync(long systemAddress)
            => System.Threading.Tasks.Task.FromResult((0, 0));
    }
}
