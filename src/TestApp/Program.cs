using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using ODExplorer.Database;
using ODExplorer.Stores;
using ODUtils.APis;
using ODUtils.Exobiology;
using ODUtils.Models;
using ODUtils.Spansh;

// Headless pipeline smoke test: replays a sample journal directory through the
// in-memory stores and verifies the state populates and events fire.
class Program
{
    static int Main()
    {
        try
        {
            var dbFile = Path.Combine(Path.GetTempPath(), "odex_pipeline_smoke.db");
            if (File.Exists(dbFile)) File.Delete(dbFile);

            var dbContextFactory = new OdExplorerDbContextFactory($"Data Source={dbFile}");
            using (var migrationContext = dbContextFactory.CreateDbContext())
            {
                migrationContext.Database.Migrate();
            }

            var db = new OdExplorerDatabaseProvider(dbContextFactory);
            var settings = new SettingsStore(db);
            var notifications = new NotificationStore(settings);
            var exo = new ExoData();

            var parser = new JournalParserStore(db, settings);
            var organicChecklist = new OrganicCheckListDataStore(parser, exo, settings, registerWithParser: false);
            var exploration = new ExplorationDataStore(parser, new EdsmApiService(), db, notifications, settings, exo, organicChecklist);
            parser.RegisterParser(organicChecklist);

            // ── Spansh CSV: real store + parser ──────────────────────────────────
            var spansh = new SpanshCsvStore(parser, db, settings, notifications);

            int failures = 0;
            void Check(string name, bool condition)
            {
                Console.WriteLine($"{(condition ? "PASS" : "FAIL")}: {name}");
                if (!condition) failures++;
            }

            var rtrCsv = Path.Combine(Path.GetTempPath(), "odex_rtr.csv");
            File.WriteAllText(rtrCsv,
                "System Name,Body Name,Body Subtype,Is Terraformable,Distance To Arrival,Estimated Scan Value,Estimated Mapping Value,Jumps\n" +
                "Sol,Sol,Star,No,0,100000,200000,1\n" +
                "Sirius,\"Sirius A 1\",Planet,Yes,1200.6,150000,300000,2\n");

            var parsed = SpanshCSVParser.ParseCsv(rtrCsv);
            Check("SpanshCSVParser detects Road to Riches from header", parsed is { CsvType: CsvType.RoadToRiches });
            Check("SpanshCSVParser maps targets", parsed is { Targets.Count: 2 } && parsed.Targets[0].SystemName == "SOL");
            Check("SpanshCSVParser maps quoted body field",
                parsed is not null && parsed.Targets[1].BodiesInfo is { Count: 1 } && parsed.Targets[1].BodiesInfo[0].Body == "A 1");
            Check("SpanshCSVParser maps body distance", parsed is not null && parsed.Targets[1].BodiesInfo is { Count: 1 } && parsed.Targets[1].BodiesInfo[0].Distance == "1,201 ls");

            var noMarker = Path.Combine(Path.GetTempPath(), "odex_no_marker.csv");
            File.WriteAllText(noMarker, "system_name,system_id64\nA,1\nB,2\n");
            Check("ParseCsv rejects unmatched header", SpanshCSVParser.ParseCsv(noMarker) is null);
            var forced = SpanshCSVParser.ForceParse(noMarker, CsvType.NeutronRoute);
            Check("ForceParse overrides unknown header", forced is { CsvType: CsvType.NeutronRoute, Targets.Count: 2 });

            Check("store ParseCSV succeeds", spansh.ParseCSV(rtrCsv));
            Check("store container populated", spansh.CurrentContainer is { Targets.Count: 2, CsvType: CsvType.RoadToRiches });
            Check("store CurrentIndex starts at 0", spansh.CurrentIndex == 0);
            spansh.CurrentIndex = 1;
            Check("store CurrentIndex navigates targets", spansh.CurrentTarget?.SystemName == "SIRIUS" && spansh.NextTarget is null);

            var gpCsv = Path.Combine(Path.GetTempPath(), "odex_gp.csv");
            File.WriteAllText(gpCsv,
                "System Name,Distance,Distance Remaining,Fuel Left,Fuel Used,Refuel,Neutron Star,Inject\n" +
                "Alpha,0.1,50.0,8,5,Yes,No,No\n" +
                "Beta,5.0,45.0,8,6,No,Yes,Yes\n");
            Check("store ForceParseCSV succeeds", spansh.ForceParseCSV(gpCsv, CsvType.GalaxyPlotter));
            Check("store refuel property mapped",
                spansh.CurrentTarget?.SystemName == "ALPHA" && spansh.CurrentTarget?.Property3 == "Yes"
                && settings.SpanshCSVSettings[settings.SelectedCommanderID] == CsvType.GalaxyPlotter);

            int timerTicks = 0;
            spansh.OnCarrierTimeTick += (_, _) => Interlocked.Increment(ref timerTicks);
            spansh.StartFleetCarrierTimer();
            Check("carrier timer running after start", spansh.CarrierTimerRunning);
            Thread.Sleep(600);
            Check("carrier timer ticks", timerTicks >= 1);
            spansh.StopFleetCarrierTimer();
            Check("carrier timer stops", spansh.CarrierTimerRunning == false);

            // ── NotificationStore: toast event emission ──────────────────────────
            var toasts = new List<ODExplorer.Models.ToastMessage>();
            notifications.OnToast += toasts.Add;

            notifications.ShowTestNotification();
            Check("test toast emitted", toasts.Count == 1 && toasts[0].Title == "OD Explorer" && toasts[0].Message == "Test notification");

            notifications.ShowWorthMappingNotification(new SystemBody { BodyName = "HD 1234" });
            Check("worth mapping toast", toasts.Count == 2 && toasts[1].Title == "Worth Mapping" && toasts[1].Message.Contains("HD 1234"));

            notifications.ShowExoBioNotification(new OrganicScanItem
            {
                SpeciesLocalised = "Bacterium Acerosis",
                Body = new SystemBody { BodyName = "Testes 1" }
            }, "Minimum Distance Travelled");
            Check("exo bio toast uses localised species + body",
                toasts.Count == 3 && toasts[2].Title == "Minimum Distance Travelled" && toasts[2].Message == "Bacterium Acerosis on Testes 1");

            notifications.ShowHighValueExoBodyNotification("Testes 2", "10,000,000", "3 Signals");
            Check("high value exo toast", toasts.Count == 4 && toasts[3].Title == "Valuable Exobiology Body" && toasts[3].Message.Contains("TESTES 2"));

            notifications.ShowNewCodexEntriesNotification("Testes 3", new Dictionary<string, bool> { ["A"] = true, ["B"] = false }, null);
            Check("new codex toast filters new-only", toasts.Count == 5 && toasts[4].Message.Contains("A") && !toasts[4].Message.Contains("B"));

            notifications.FleetCarrierNotification("FC-1000");
            Check("fleet carrier toast", toasts.Count == 6 && toasts[5].Title == "Fleet Carrier" && toasts[5].Message == "FC-1000");

            settings.NotificationSettings.NotificationsEnabled = false;
            notifications.ShowTestNotification();
            Check("disabled notifications suppress emission", toasts.Count == 6);
            settings.NotificationSettings.NotificationsEnabled = true;

            // ── EdAstro: GEC JSON → EdAstroPoi mapping (offline) ──────────────────
            const string gecJson =
                "[{\"id\":10,\"type\":\"Sights and Scenery\",\"type2\":\"\",\"name\":\"The Ammonia Lyceum\"," +
                "\"galMapSearch\":\"Athaip WR-H d11-7577\",\"coordinates\":[334.969,-55.4375,23014.3]," +
                "\"summary\":\"A nested moon close to its parent.\"," +
                "\"descriptionMardown\":\"![caption](https://edastro.com/poiimages/2023/11/x.jpg)\r\n\r\nBody *3 b a*.\"," +
                "\"solDistance\":23016.8,\"id64\":260354282082915,\"poiUrl\":\"https://edastro.com/gec/view/10\"}," +
                "{\"id\":14,\"type\":\"Green Gas Giants\",\"type2\":\"Historical\",\"name\":\"Dueling Rings\"," +
                "\"galMapSearch\":\"Qiefaa EB-U d4-20\",\"coordinates\":[8814.06,-85,-5725.47]," +
                "\"summary\":\"A ringed icy moon.\",\"descriptionMardown\":\"No photos yet.\"," +
                "\"solDistance\":\"10519.4\",\"id64\":\"2817395361024\"}]";

            var pois = ODUtils.APis.EdAstroApiService.ParsePois(gecJson);
            Check("GEC maps all POIs", pois.Count == 2);
            Check("GEC maps name + galMap", pois is { Count: 2 } && pois[0].Name == "The Ammonia Lyceum" && pois[0].GalMapName == "Athaip WR-H d11-7577");
            Check("GEC maps types", pois[0].Type == ODUtils.Models.EdAstro.EDAstroType.SightsAndScenery
                && pois[0].Type2 == ODUtils.Models.EdAstro.EDAstroType.Unknown
                && pois[1].Type == ODUtils.Models.EdAstro.EDAstroType.GreenGasGiants
                && pois[1].Type2 == ODUtils.Models.EdAstro.EDAstroType.Historical);
            Check("GEC maps numeric id64", pois[0].SystemAddress == 260354282082915);
            Check("GEC maps string id64", pois[1].SystemAddress == 2817395361024);
            Check("GEC maps coordinates", Math.Abs(pois[0].SystemPosition.X - 334.969) < 0.001
                && Math.Abs(pois[0].SystemPosition.Y - (-55.4375)) < 0.001
                && Math.Abs(pois[0].SystemPosition.Z - 23014.3) < 0.001);
            Check("GEC maps solDistance (number + string)", Math.Abs(pois[0].DistanceFromSol - 23016.8) < 0.001
                && Math.Abs(pois[1].DistanceFromSol - 10519.4) < 0.001);
            Check("GEC keeps markdown", pois[0].MarkDown.Contains("https://edastro.com/poiimages"));
            Check("GEC fills missing poiUrl", pois[1].PoiUrl.OriginalString == "https://edastro.com/gec/view/14");

            // EdAstroPoiViewModel: markdown rewrite + distance from commander.
            var poiVm = new ODExplorer.ViewModels.ModelVMs.EdAstroPoiViewModel(pois[0], new(0, 0, 0));
            Check("POI VM exposes markdown", poiVm.MarkDown.Contains("poiimages") && poiVm.DistanceFromCommander > 23000);

            int liveCount = 0, currentSystemEvents = 0, organicDetailsEvents = 0, cartoSold = 0, bioSold = 0;
            parser.OnParserStoreLive += (_, live) => { if (live) Interlocked.Increment(ref liveCount); };
            exploration.OnCurrentSystemUpdated += (_, s) => { if (s is not null) Interlocked.Increment(ref currentSystemEvents); };
            organicChecklist.OnOrganicScanDetailsUpdated += (_, _) => Interlocked.Increment(ref organicDetailsEvents);
            exploration.OnCartoDataSold += (_, _) => Interlocked.Increment(ref cartoSold);
            exploration.OnBioDataSold += (_, _) => Interlocked.Increment(ref bioSold);

            var journalDir = Path.Combine(Path.GetTempPath(), "odex_pipeline_smoke");
            Directory.CreateDirectory(journalDir);
            foreach (var f in Directory.GetFiles(journalDir, "*.log")) File.Delete(f);
            File.WriteAllLines(Path.Combine(journalDir, "Journal.240101000000.01.log"), SampleLines());

            parser.ReadNewDirectory(journalDir);

            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (!parser.IsLive && DateTime.UtcNow < deadline) Thread.Sleep(50);

            Check("parser became live", parser.IsLive);
            Check("parser live raised once", liveCount == 1);
            Check("current system = NextSys", exploration.CurrentSystem?.Name == "NextSys");
            Check("current system region name set", string.IsNullOrEmpty(exploration.CurrentSystemRegion) == false);
            Check("Testes in sold carto", exploration.GetSoldCartoSystems().Any(x => x.Name == "Testes"));
            Check("NextSys has no unsold", exploration.GetUnsoldCartoSystems().All(x => x.Name != "NextSys"));
            Check("current system event raised", currentSystemEvents >= 1);
            Check("organic details event raised", organicDetailsEvents >= 1);

            var bacterial = organicChecklist.OrganicScanItems.TryGetValue("$Codex_Ent_Bacterial_Genus_Name;", out var list) ? list : null;
            Check("bacterial genus key present", bacterial is { Count: > 0 });
            var acerosis = bacterial?.FirstOrDefault(x => x.SpeciesCodex == "$Codex_Ent_Bacterial_01_Name;");
            Check("Bacterial 01 present", acerosis is not null);
            Check("Bacterial 01 sold after sale", acerosis is not null && acerosis.Region.Any(r => r.Value == ODUtils.Models.OrganicScanState.Sold));
            Check("Bacterial 01 has variants", acerosis is not null && acerosis.Variants.Count > 0);
            Check("carto sold event not raised during history (live false)", cartoSold == 0);
            Check("bio sold event not raised during history (live false)", bioSold == 0);

            // Live tailing: append a new FSDJump to the current journal file and a
            // brand-new .log file; both should be picked up while IsLive is true
            // and fire current-system events.
            int liveEventsBefore = currentSystemEvents;
            var journalFile = Path.Combine(journalDir, "Journal.240101000000.01.log");
            File.AppendAllText(journalFile,
                "{\"timestamp\":\"2024-01-01T00:00:13Z\",\"event\":\"FSDJump\",\"StarSystem\":\"LiveSys\",\"SystemAddress\":3103895106050,\"StarPos\":[10.0,20.0,30.0],\"StarType\":\"G\",\"Body\":7,\"Bodies\":1,\"JumpDist\":40.0}\n");
            File.AppendAllText(Path.Combine(journalDir, "Journal.240101010000.02.log"),
                "{\"timestamp\":\"2024-01-01T01:00:00Z\",\"event\":\"FSDJump\",\"StarSystem\":\"RolledSys\",\"SystemAddress\":3103895106051,\"StarPos\":[50.0,60.0,70.0],\"StarType\":\"K\",\"Body\":7,\"Bodies\":1,\"JumpDist\":80.0}\n");

            deadline = DateTime.UtcNow.AddSeconds(15);
            while (exploration.CurrentSystem?.Name is "NextSys" or null && DateTime.UtcNow < deadline) Thread.Sleep(100);

            Check("live tail picks up appended lines", exploration.CurrentSystem?.Name is "LiveSys" or "RolledSys");
            Check("live tail fired current-system events", currentSystemEvents > liveEventsBefore);

            // Persistence: a fresh provider over the same SQLite file must see the
            // commander and journal entries written during the parse.
            var persistedProvider = new OdExplorerDatabaseProvider(dbContextFactory);
            var savedCommanders = persistedProvider.GetAllJournalCommanders(true).GetAwaiter().GetResult();
            Check("commander persisted", savedCommanders.Any(x => x.Name == "TestCMDR"));

            var savedId = savedCommanders.FirstOrDefault(x => x.Name == "TestCMDR")?.Id ?? 0;
            var savedEntries = savedId == 0 ? [] : persistedProvider.GetAllJournalEntries(savedId).GetAwaiter().GetResult();
            Check("journal entries persisted", savedEntries.Count >= 13);
            Check("FSDJump entries persisted", savedEntries.Count(x => x.EventType == ODUtils.Journal.JournalTypeEnum.FSDJump) >= 2);

            // Journal entries can be reconstructed back into typed event data.
            var scanEntries = savedEntries.Where(x => x.EventType == ODUtils.Journal.JournalTypeEnum.Scan).ToList();
            Check("reconstructed Scan events have data", scanEntries.Count >= 2 && scanEntries.All(x => x.EventData is not null));

            // Restart simulation: re-parsing the same journal dir against the same
            // DB must update (not duplicate) the commander and journal entries.
            var restartParser = new JournalParserStore(persistedProvider, settings);
            _ = new ExplorationDataStore(restartParser, new EdsmApiService(), persistedProvider, notifications, settings, exo, organicChecklist);
            restartParser.ReadNewDirectory(journalDir);
            deadline = DateTime.UtcNow.AddSeconds(30);
            while (!restartParser.IsLive && DateTime.UtcNow < deadline) Thread.Sleep(50);

            var finalCommanders = persistedProvider.GetAllJournalCommanders(true).GetAwaiter().GetResult();
            var finalEntries = persistedProvider.GetAllJournalEntries(savedId).GetAwaiter().GetResult();
            Check("restart keeps commander count at 1", finalCommanders.Count(x => x.Name == "TestCMDR") == 1);
            Check("restart does not duplicate journal entries", finalEntries.Count == 15);

            // Resume: loading the SAVED commander with a fresh store must skip the
            // already-parsed history and pick up only lines appended after the last read.
            File.AppendAllText(Path.Combine(journalDir, "Journal.240101010000.02.log"),
                "{\"timestamp\":\"2024-01-01T01:00:01Z\",\"event\":\"FSDJump\",\"StarSystem\":\"ResumeSys\",\"SystemAddress\":3103895106052,\"StarPos\":[1.0,2.0,3.0],\"StarType\":\"M\",\"Body\":7,\"Bodies\":1,\"JumpDist\":60.0}\n");

            var resumeParser = new JournalParserStore(persistedProvider, settings);
            var resumeExploration = new ExplorationDataStore(resumeParser, new EdsmApiService(), persistedProvider, notifications, settings, exo, organicChecklist);
            resumeParser.ReadNewCommander(savedId);
            deadline = DateTime.UtcNow.AddSeconds(30);
            while (!resumeParser.IsLive && DateTime.UtcNow < deadline) Thread.Sleep(50);

            var resumeEntries = persistedProvider.GetAllJournalEntries(savedId).GetAwaiter().GetResult();
            Check("resume adds only new lines (no history re-parse)", resumeEntries.Count == 16);
            Check("resume parsed the appended line", resumeExploration.CurrentSystem?.Name == "ResumeSys");

            // ── Live bio discovery toasts: a new-species ScanOrganic and a CodexEntry
            //    must raise the valuable-exo / new-species / new-codex notifications. ──
            settings.NotificationOptions = ODExplorer.Models.NotificationOptions.ValuableBioPlanet
                | ODExplorer.Models.NotificationOptions.NewBioCodexEntry
                | ODExplorer.Models.NotificationOptions.NewBioSpecies;
            settings.SystemGridSetting.ExoValuableBodyValue = 1;

            File.AppendAllText(Path.Combine(journalDir, "Journal.240101010000.02.log"),
                "{\"timestamp\":\"2024-01-01T01:00:04Z\",\"event\":\"ScanOrganic\",\"ScanType\":\"Analyse\",\"Genus\":\"$Codex_Ent_Cactoid_Genus_Name;\",\"Genus_Localised\":\"Cactoida\",\"Species\":\"$Codex_Ent_Cactoid_01_Name;\",\"Species_Localised\":\"Cactoida Cortexum\",\"Variant\":\"$Codex_Ent_Cactoid_01_A_Name;\",\"Variant_Localised\":\"Cactoida Cortexum Amethyst\",\"SystemAddress\":10477373803,\"Body\":2,\"Latitude\":5.0,\"Longitude\":6.0}\n");

            deadline = DateTime.UtcNow.AddSeconds(15);
            while (toasts.Any(x => x.Title == "Possible New Species Codex") == false && DateTime.UtcNow < deadline) Thread.Sleep(100);

            Check("valuable exo body toast fired on live scan", toasts.Any(x => x.Title == "Valuable Exobiology Body"));
            Check("new species toast fired on live scan",
                toasts.Any(x => x.Title == "Possible New Species Codex" && x.Message.Contains("Cactoida Cortexum")));

            File.AppendAllText(Path.Combine(journalDir, "Journal.240101010000.02.log"),
                "{\"timestamp\":\"2024-01-01T01:00:05Z\",\"event\":\"CodexEntry\",\"System\":\"Testes\",\"SystemAddress\":10477373803,\"Body\":\"Testes 2\",\"BodyID\":2,\"Name\":\"$Codex_Ent_Cactoid_01_A_Name;\",\"Name_Localised\":\"Cactoida Cortexum Amethyst\",\"Category\":\"$Codex_Category_Biology;\",\"SubCategory\":\"$Codex_SubCategory_Organic_Structures;\",\"Region\":1,\"IsNewEntry\":true}\n");

            deadline = DateTime.UtcNow.AddSeconds(15);
            while (toasts.Any(x => x.Title == "Possible New Personal Codex") == false && DateTime.UtcNow < deadline) Thread.Sleep(100);

            Check("new codex toast fired on live codex entry",
                toasts.Any(x => x.Title == "Possible New Personal Codex" && x.Message.Contains("Cactoida Cortexum Amethyst")));

            // ── Settings persistence: SaveSettings/LoadSettings round-trip via the DB. ──
            settings.DeveloperMode = true;
            settings.MinimiseToTray = true;
            settings.UiScale = 1.35;
            settings.NotificationSettings.DisplayTime = 42;
            settings.SystemGridSetting.ExoValuableBodyValue = 9_999_999;
            settings.SaveSettings();

            settings.DeveloperMode = false;
            settings.MinimiseToTray = false;
            settings.UiScale = 1.0;
            settings.NotificationSettings.DisplayTime = 7;
            settings.SystemGridSetting.ExoValuableBodyValue = 20_000_000;
            settings.LoadSettings();

            Check("settings round-trip DeveloperMode", settings.DeveloperMode);
            Check("settings round-trip MinimiseToTray", settings.MinimiseToTray);
            Check("settings round-trip UiScale", Math.Abs(settings.UiScale - 1.35) < 0.001);
            Check("settings round-trip NotificationSettings", settings.NotificationSettings.DisplayTime == 42);
            Check("settings round-trip SystemGridSetting", settings.SystemGridSetting.ExoValuableBodyValue == 9_999_999);

            Console.WriteLine();
            Console.WriteLine(failures == 0 ? "ALL CHECKS PASSED" : $"{failures} CHECK(S) FAILED");
            return failures == 0 ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"UNEXPECTED: {ex}");
            return 2;
        }
    }

    static string[] SampleLines() =>
    [
        "{\"timestamp\":\"2024-01-01T00:00:00Z\",\"event\":\"Fileheader\",\"part\":1,\"language\":\"English\"}",
        "{\"timestamp\":\"2024-01-01T00:00:01Z\",\"event\":\"LoadGame\",\"Commander\":\"TestCMDR\",\"Ship\":\"CobraMkIII\",\"GameMode\":\"Solo\",\"Credits\":1000000}",
        "{\"timestamp\":\"2024-01-01T00:00:02Z\",\"event\":\"FSDJump\",\"StarSystem\":\"Testes\",\"SystemAddress\":10477373803,\"StarPos\":[30.0,-40.0,5.0],\"StarType\":\"K\",\"Body\":7,\"Bodies\":4,\"JumpDist\":8.5}",
        "{\"timestamp\":\"2024-01-01T00:00:03Z\",\"event\":\"Scan\",\"ScanType\":\"Detailed\",\"BodyName\":\"Testes\",\"BodyID\":0,\"StarSystem\":\"Testes\",\"SystemAddress\":10477373803,\"DistanceFromArrivalLS\":0.0,\"StarType\":\"K\",\"StarClass\":\"K5 Va\",\"StellarMass\":0.7,\"Radius\":500000000.0,\"AbsoluteMagnitude\":7.5,\"Age_MY\":4000.0,\"WasDiscovered\":true,\"WasMapped\":false}",
        "{\"timestamp\":\"2024-01-01T00:00:04Z\",\"event\":\"Scan\",\"ScanType\":\"Detailed\",\"BodyName\":\"Testes 1\",\"BodyID\":1,\"StarSystem\":\"Testes\",\"SystemAddress\":10477373803,\"DistanceFromArrivalLS\":1800.0,\"PlanetClass\":\"Rocky body\",\"Landable\":true,\"SurfaceTemperature\":280.0,\"MassEM\":0.05,\"Radius\":800000.0,\"SurfaceGravity\":1.1,\"OrbitalPeriod\":100000.0,\"WasDiscovered\":false,\"WasMapped\":false,\"Signals\":[{\"Type\":\"$SAA_SignalType_Biological;\",\"Type_Localised\":\"Biological\",\"Count\":3}]}",
        "{\"timestamp\":\"2024-01-01T00:00:05Z\",\"event\":\"FSSBodySignals\",\"SystemAddress\":10477373803,\"BodyID\":1,\"Signals\":[{\"Type\":\"$SAA_SignalType_Biological;\",\"Type_Localised\":\"Biological\",\"Count\":3}]}",
        "{\"timestamp\":\"2024-01-01T00:00:06Z\",\"event\":\"SAAScanComplete\",\"SystemName\":\"Testes\",\"SystemAddress\":10477373803,\"BodyID\":1,\"ProbesUsed\":5,\"EfficiencyTargetMet\":true}",
        "{\"timestamp\":\"2024-01-01T00:00:07Z\",\"event\":\"ApproachBody\",\"StarSystem\":\"Testes\",\"SystemAddress\":10477373803,\"Body\":\"Testes 1\",\"BodyID\":1}",
        "{\"timestamp\":\"2024-01-01T00:00:08Z\",\"event\":\"ScanOrganic\",\"ScanType\":\"Analyse\",\"Genus\":\"$Codex_Ent_Bacterial_Genus_Name;\",\"Genus_Localised\":\"Bacterium\",\"Species\":\"$Codex_Ent_Bacterial_01_Name;\",\"Species_Localised\":\"Bacterium Acerosis\",\"Variant\":\"$Codex_Ent_Bacterial_01_A_Name;\",\"Variant_Localised\":\"Bacterium Acerosis Amethyst\",\"SystemAddress\":10477373803,\"Body\":1,\"Latitude\":10.0,\"Longitude\":20.0}",
        "{\"timestamp\":\"2024-01-01T00:00:09Z\",\"event\":\"CodexEntry\",\"System\":\"Testes\",\"SystemAddress\":10477373803,\"Body\":\"Testes 1\",\"BodyID\":1,\"Name\":\"$Codex_Ent_Bacterial_01_A_Name;\",\"Name_Localised\":\"Bacterium Acerosis Amethyst\",\"Category\":\"$Codex_Category_Biology;\",\"SubCategory\":\"$Codex_SubCategory_Organic_Structures;\",\"Region\":1,\"IsNewEntry\":true}",
        "{\"timestamp\":\"2024-01-01T00:00:10Z\",\"event\":\"SellOrganicData\",\"MarketID\":3229234944,\"BioData\":[{\"Name\":\"$Codex_Ent_Bacterial_01_A_Name;\",\"Name_Localised\":\"Bacterium Acerosis Amethyst\",\"Genus\":\"$Codex_Ent_Bacterial_Genus_Name;\",\"Species\":\"$Codex_Ent_Bacterial_01_Name;\",\"Variant\":\"$Codex_Ent_Bacterial_01_A_Name;\",\"Value\":50000,\"Bonus\":50000,\"TotalValue\":100000}]}",
        "{\"timestamp\":\"2024-01-01T00:00:11Z\",\"event\":\"SellExplorationData\",\"Systems\":[\"Testes\"],\"Discovered\":[{\"SystemName\":\"Testes\",\"NumBodies\":4}],\"BaseValue\":50000,\"Bonus\":10000,\"TotalEarnings\":60000}",
        "{\"timestamp\":\"2024-01-01T00:00:12Z\",\"event\":\"FSDJump\",\"StarSystem\":\"NextSys\",\"SystemAddress\":3103895106049,\"StarPos\":[-100.0,200.0,300.0],\"StarType\":\"G\",\"Body\":7,\"Bodies\":1,\"JumpDist\":350.0}"
    ];
}
