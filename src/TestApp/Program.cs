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

            // ── Notable-body notification thresholds (CheckForNotableNotifications) ─
            var notableSettings = new SettingsStore(db);
            var notable = new NotificationStore(notableSettings);
            var notableToasts = new List<ODExplorer.Models.ToastMessage>();
            notable.OnToast += notableToasts.Add;

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.DiverseLife;
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Bio Planet", BiologicalSignals = 9 });
            Check("notable diverse life fires at limit", notableToasts.Count == 1 && notableToasts[0].Title == "Diverse Exobiology Body" && notableToasts[0].Message.Contains("9 Signals"));
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Bio Planet", BiologicalSignals = 8 });
            Check("notable diverse life stays silent below limit", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.SmallPlanet;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Tiny", Radius = 300 });
            Check("notable small radius fires at limit", notableToasts.Count == 1 && notableToasts[0].Title == "Small Radius Body" && notableToasts[0].Message.Contains("300 km"));
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Tiny", Radius = 301 });
            Check("notable small radius stays silent above limit", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.HighEccentricity;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Ecc", Eccentricity = 0.9 });
            Check("notable high eccentricity fires at limit", notableToasts.Count == 1 && notableToasts[0].Message.Contains("0.9000"));
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Ecc", Eccentricity = 0.8 });
            Check("notable high eccentricity stays silent below limit", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.NestedMoon;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Nested Moon",
                Parents =
                {
                    new Parent { Type = ParentType.Planet, BodyID = 1 },
                    new Parent { Type = ParentType.Planet, BodyID = 2 }
                }
            });
            Check("notable nested moon fires for planet-planet parents", notableToasts.Count == 1 && notableToasts[0].Title == "Nested Moon");
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Plain Moon",
                Parents = { new Parent { Type = ParentType.Planet, BodyID = 1 } }
            });
            Check("notable nested moon stays silent with a single parent", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.FastRotation;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Spinny", RotationPeriod = 0.1, TidalLock = false });
            Check("notable fast rotation fires below the hours limit", notableToasts.Count == 1 && notableToasts[0].Message.Contains("2.4 hours"));
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Tidally Locked", RotationPeriod = 0.1, TidalLock = true });
            Check("notable fast rotation stays silent when tidally locked", notableToasts.Count == 0);
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Slow", RotationPeriod = 0.5, TidalLock = false });
            Check("notable fast rotation stays silent above limit", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.FastOrbit;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Fast Orbiter", OrbitalPeriod = 0.25 });
            Check("notable fast orbit fires below the hours limit", notableToasts.Count == 1 && notableToasts[0].Message.Contains("6.0 hours"));
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Slow Orbiter", OrbitalPeriod = 0.5 });
            Check("notable fast orbit stays silent above limit", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.WideRings;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Ringed",
                Radius = 10,
                Rings = new List<EliteJournalReader.Events.PlanetRing>
                {
                    new() { Name = "Ring A", InnerRad = 1000, OuterRad = 60000 },
                    new() { Name = "Ring B Belt", InnerRad = 1000, OuterRad = 60000 }
                }
            });
            Check("notable wide ring fires past the radius multiplier and skips belts", notableToasts.Count == 1 && notableToasts[0].Title == "Body With Wide Ring");
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Ringed",
                Radius = 10,
                Rings = new List<EliteJournalReader.Events.PlanetRing>
                {
                    new() { Name = "Ring A", InnerRad = 1000, OuterRad = 20000 }
                }
            });
            Check("notable wide ring stays silent when narrow", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.ShepherdMoon;
            notableToasts.Clear();
            var shepherdOwner = new Owner();
            shepherdOwner.SystemBodies.Add(new SystemBody
            {
                BodyID = 1,
                Rings = new List<EliteJournalReader.Events.PlanetRing>
                {
                    new() { Name = "Parent Ring", InnerRad = 100, OuterRad = 5000 }
                }
            });
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Shepherd",
                SemiMajorAxis = 2000,
                Parents = { new Parent { Type = ParentType.Planet, BodyID = 1 } },
                Owner = shepherdOwner
            });
            Check("notable shepherd moon fires when orbiting inside the parent ring", notableToasts.Count == 1 && notableToasts[0].Title == "Shepherd Moon");
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Not Shepherd",
                SemiMajorAxis = 9000,
                Parents = { new Parent { Type = ParentType.Planet, BodyID = 1 } },
                Owner = shepherdOwner
            });
            Check("notable shepherd moon stays silent outside the parent ring", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.LandableTerraformable;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Terra", Landable = true, Terraformable = true, TerraformState = "Candidate for terraforming" });
            Check("notable landable terraformable fires with terraform state", notableToasts.Count == 1 && notableToasts[0].Message.Contains("Candidate for terraforming"));
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Terra", Landable = false, Terraformable = true });
            Check("notable landable checks stay silent for non-landable bodies", notableToasts.Count == 0);

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.LandableWithRings;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody
            {
                BodyName = "Ringed Land",
                Landable = true,
                Rings = new List<EliteJournalReader.Events.PlanetRing> { new() { Name = "R" } }
            });
            Check("notable landable with rings fires", notableToasts.Count == 1 && notableToasts[0].Title == "Landable Body With Rings");

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.LandableHighGravity;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Heavy", Landable = true, SurfaceGravity = 30 });
            Check("notable landable high gravity fires at/above limit", notableToasts.Count == 1 && notableToasts[0].Message.Contains("30.00 g"));

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.LandableLargeRadius;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Big", Landable = true, Radius = 20000 });
            Check("notable landable large radius fires above limit", notableToasts.Count == 1 && notableToasts[0].Title == "Landable Large Radius Body");

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.BioSignals;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Bio", BiologicalSignals = 1, Landable = true });
            Check("notable bio signal fires on landable body with singular wording", notableToasts.Count == 1 && notableToasts[0].Message.Contains("1 Biological Signal"));

            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.GeoSignals;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Geo", GeologicalSignals = 2, Landable = true });
            Check("notable geo signal fires on landable body", notableToasts.Count == 1 && notableToasts[0].Message.Contains("2 Geological Signals"));

            notableSettings.NotificationSettings.NotificationsEnabled = false;
            notableSettings.NotableSettings.BodyNotifications = ODExplorer.Models.BodyNotification.DiverseLife;
            notableToasts.Clear();
            notable.CheckForNotableNotifications(new SystemBody { BodyName = "Bio", BiologicalSignals = 9 });
            Check("notable checks suppressed when notifications disabled", notableToasts.Count == 0);

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

            // ── ODUtils stub layer: jumponium material mapping + species table ──
            var carbon = new ShipMaterials { Name = "carbon" };
            var vanadium = new ShipMaterials { Name = "vanadium" };
            var germanium = new ShipMaterials { Name = "germanium" };
            var cadmium = new ShipMaterials { Name = "cadmium" };
            var niobium = new ShipMaterials { Name = "niobium" };
            var yttrium = new ShipMaterials { Name = "yttrium" };
            var polonium = new ShipMaterials { Name = "polonium" };
            Check("material name maps to flag", carbon.Name_AsMaterial == PlanetMaterial.carbon
                && new ShipMaterials { Name = "CARBON" }.Name_AsMaterial == PlanetMaterial.carbon
                && polonium.Name_AsMaterial == PlanetMaterial.polonium);
            Check("non-jumponium material maps to None", new ShipMaterials { Name = "iron" }.Name_AsMaterial == PlanetMaterial.None);

            PlanetMaterial allJumponium = PlanetMaterial.carbon | PlanetMaterial.vanadium | PlanetMaterial.germanium
                | PlanetMaterial.cadmium | PlanetMaterial.niobium | PlanetMaterial.yttrium | PlanetMaterial.polonium;
            Check("ContainsAllShipMaterials basic subset", ODUtils.Helpers.EnumUtility.ContainsAllShipMaterials(allJumponium,
                PlanetMaterial.carbon | PlanetMaterial.vanadium | PlanetMaterial.germanium));
            Check("ContainsAllShipMaterials standard subset", ODUtils.Helpers.EnumUtility.ContainsAllShipMaterials(allJumponium,
                PlanetMaterial.carbon | PlanetMaterial.vanadium | PlanetMaterial.germanium | PlanetMaterial.cadmium | PlanetMaterial.niobium));
            Check("ContainsAllShipMaterials premium requires all seven", ODUtils.Helpers.EnumUtility.ContainsAllShipMaterials(allJumponium, allJumponium));
            Check("ContainsAllShipMaterials missing material is false", ODUtils.Helpers.EnumUtility.ContainsAllShipMaterials(allJumponium & ~PlanetMaterial.polonium, allJumponium) == false);
            Check("ContainsAllShipMaterials empty mats is false", ODUtils.Helpers.EnumUtility.ContainsAllShipMaterials(PlanetMaterial.None, PlanetMaterial.carbon) == false);

            var jumponiumBody = new SystemBody { Landable = true, IsPlanet = true, Materials = [carbon, vanadium, germanium, cadmium, niobium, yttrium, polonium] };
            var jumponiumSysVm = new ODExplorer.ViewModels.ModelVMs.StarSystemViewModel(new StarSystem(), settings, notifications)
            {
                Bodies = new System.Collections.ObjectModel.ObservableCollection<ODExplorer.ViewModels.ModelVMs.SystemBodyViewModel>
                {
                    new ODExplorer.ViewModels.ModelVMs.SystemBodyViewModel(jumponiumBody, settings)
                }
            };
            Check("system with all jumponium materials reports Premium", jumponiumSysVm.GreenSystem == ODExplorer.Models.Jumponium.Premium);

            var basicBody = new SystemBody { Landable = true, IsPlanet = true, Materials = [carbon, vanadium, germanium] };
            var basicSysVm = new ODExplorer.ViewModels.ModelVMs.StarSystemViewModel(new StarSystem(), settings, notifications)
            {
                Bodies = new System.Collections.ObjectModel.ObservableCollection<ODExplorer.ViewModels.ModelVMs.SystemBodyViewModel>
                {
                    new ODExplorer.ViewModels.ModelVMs.SystemBodyViewModel(basicBody, settings)
                }
            };
            Check("system with only basic materials reports Standard", basicSysVm.GreenSystem == ODExplorer.Models.Jumponium.Standard);

            var emptyBody = new SystemBody { Landable = true, IsPlanet = true, Materials = [new ShipMaterials { Name = "iron" }] };
            var emptySysVm = new ODExplorer.ViewModels.ModelVMs.StarSystemViewModel(new StarSystem(), settings, notifications)
            {
                Bodies = new System.Collections.ObjectModel.ObservableCollection<ODExplorer.ViewModels.ModelVMs.SystemBodyViewModel>
                {
                    new ODExplorer.ViewModels.ModelVMs.SystemBodyViewModel(emptyBody, settings)
                }
            };
            Check("system without jumponium materials reports None", emptySysVm.GreenSystem == ODExplorer.Models.Jumponium.None);

            Check("exo table has 90 species", exo.AllGenus.Sum(g => g.Species.Count) == 90);
            Check("exo Stratum Tectonicas value", exo.GetInfo("$Codex_Ent_Stratum_07_Name;") is { Value: 19_010_800 });
            Check("exo Electricae Radialem value", exo.GetInfo("$Codex_Ent_Electricae_02_Name;") is { Value: 6_284_600 });
            Check("exo Fungoida Setisis value", exo.GetInfo("$Codex_Ent_Fungoids_01_Name;") is { Value: 1_670_100 });
            Check("exo Bacterium Aurasus value", exo.GetInfo("$Codex_Ent_Bacterial_01_Name;") is { Value: 1_000_000 });
            Check("exo Bacterium colony range", exo.GetInfo("$Codex_Ent_Bacterial_01_Name;") is { ColonyRange: 2 });
            Check("exo unknown codex returns null", exo.GetInfo("$Codex_Ent_Bogus_01_Name;") is null);
            Check("exo species name from codex", ExoData.GetNamesFromSpecies("$Codex_Ent_Stratum_07_Name;") is { Species: "Stratum Tectonicas" });
            Check("exo variant name from codex", ExoData.GetNames("$Codex_Ent_Aleoids_01_A_Name;") is { Genus: "Aleoida", Species: "Aleoida Arcus", Variant: "Aleoida Arcus A" });
            Check("exo species available in all regions", exo.AllGenus.SelectMany(g => g.Species)
                .First(s => s.SpeciesName == "Stratum Tectonicas").IsAvailableIn(GalacticRegions.Bubble));

            // ── Prediction engine: cross-validated against the BioScan rules. The
            //    expected species lists below were produced by a Python mirror of
            //    the C# engine run over the generated rule data. At the default
            //    origin (0,0,0 = Sol, region id 18 = orion-cygnus) the region-gated
            //    rules now contribute (Fungoida Stabitis, Osseus Fractus/Pellebantus,
            //    Electricae Radialem), which the committed data previously omitted. ──
            SystemBody PredictBody(Action<SystemBody> setup)
            {
                var body = new SystemBody
                {
                    IsPlanet = true,
                    GoverningStar = StarType.G
                };
                setup(body);
                return body;
            }

            var gSystem = new StarSystem { StarType = StarType.G, Address = 1 };
            var aSystem = new StarSystem { StarType = StarType.A, Address = 1 };

            List<string> PredictNames(SystemBody body, StarSystem system)
                => ExoPredictionEngine.Predict(body, system).Select(p => p.Name).OrderBy(n => n).ToList();


            Check("HMC CO2 250K predicts Aurasus + Renibus + region species + Tectonicas",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.HighMetalContentBody;
                    b.Atmosphere = AtmosphereClass.CarbonDioxide;
                    b.SurfaceGravity = 1.0;
                    b.SurfaceTemp = 250;
                    b.DistanceFromArrivalLs = 500;
                }), gSystem)) == "Bacterium Aurasus,Concha Renibus,Fungoida Stabitis,Osseus Fractus,Osseus Pellebantus,Stratum Tectonicas");

            Check("Rocky no-atmosphere body predicts nothing (like Testes 1)",
                PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.RockyBody;
                    b.SurfaceGravity = 1.1;
                    b.SurfaceTemp = 280;
                    b.DistanceFromArrivalLs = 1800;
                }), gSystem).Count == 0);

            Check("Icy Argon around A-star predicts Vesicula + Pluma + Radialem + Campestris",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.IcyBody;
                    b.Atmosphere = AtmosphereClass.Argon;
                    b.SurfaceGravity = 0.4;
                    b.SurfaceTemp = 100;
                    b.DistanceFromArrivalLs = 50;
                }), aSystem)) == "Bacterium Vesicula,Electricae Pluma,Electricae Radialem,Fonticulua Campestris");

            Check("HMC CO2 with water geysers predicts Tela (any-volcanism rule)",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.HighMetalContentBody;
                    b.Atmosphere = AtmosphereClass.CarbonDioxide;
                    b.SurfaceGravity = 1.0;
                    b.SurfaceTemp = 250;
                    b.VolcanismName = "Water Geysers Volcanism";
                    b.DistanceFromArrivalLs = 500;
                }), gSystem)) == "Bacterium Aurasus,Bacterium Tela,Stratum Tectonicas");

            Check("Rocky ice methane 40K predicts nothing",
                PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.RockyIceBody;
                    b.Atmosphere = AtmosphereClass.Methane;
                    b.SurfaceGravity = 0.3;
                    b.SurfaceTemp = 40;
                    b.DistanceFromArrivalLs = 100;
                }), gSystem).Count == 0);

            Check("HMC CO2 rich 1.5% SO2 adds Recepta Umbrux",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.HighMetalContentBody;
                    b.Atmosphere = AtmosphereClass.CarbonDioxide;
                    b.SurfaceGravity = 1.0;
                    b.SurfaceTemp = 200;
                    b.AtmosphereComposition = [new ScanItemComponent { Name = "SulphurDioxide", Percent = 1.5 }];
                    b.DistanceFromArrivalLs = 500;
                }), gSystem)) == "Bacterium Aurasus,Concha Labiata,Concha Renibus,Fungoida Stabitis,Osseus Fractus,Osseus Pellebantus,Recepta Umbrux,Stratum Tectonicas");

            Check("HMC CO2 0.5% SO2 misses Recepta Umbrux (SO2 < 1.05)",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.HighMetalContentBody;
                    b.Atmosphere = AtmosphereClass.CarbonDioxide;
                    b.SurfaceGravity = 1.0;
                    b.SurfaceTemp = 200;
                    b.AtmosphereComposition = [new ScanItemComponent { Name = "SulphurDioxide", Percent = 0.5 }];
                    b.DistanceFromArrivalLs = 500;
                }), gSystem)) == "Bacterium Aurasus,Concha Labiata,Concha Renibus,Fungoida Stabitis,Osseus Fractus,Osseus Pellebantus,Stratum Tectonicas");

            // Galaxy-position gates: Cactoida Lapis carries regions=[sagittarius-carina]
            // and Electricae Radialem carries nebula=all, so they must appear only where
            // the system actually sits inside the matching region / nebula.
            var elysianShoreSystem = new StarSystem { StarType = StarType.G, Address = 1, Name = "Elysian Shore Probe", Position = new(-4499.4, 0, -5535.8) };

            Check("HMC Ammonia region rule: Cactoida Lapis present in Sagittarius-Carina (Sol)",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.HighMetalContentBody;
                    b.Atmosphere = AtmosphereClass.Ammonia;
                    b.SurfaceGravity = 1.0;
                    b.SurfaceTemp = 165;
                    b.DistanceFromArrivalLs = 500;
                }), gSystem)) == "Aleoida Laminiae,Bacterium Alcyoneum,Cactoida Lapis,Concha Aureolas,Frutexa Metallicum,Fungoida Setisis,Osseus Spiralis,Stratum Tectonicas,Tubus Sororibus,Tussock Cultro");

            Check("HMC Ammonia region rule: no Cactoida Lapis outside Sagittarius-Carina (Elysian Shore)",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.HighMetalContentBody;
                    b.Atmosphere = AtmosphereClass.Ammonia;
                    b.SurfaceGravity = 1.0;
                    b.SurfaceTemp = 165;
                    b.DistanceFromArrivalLs = 500;
                }), elysianShoreSystem)) == "Bacterium Alcyoneum,Concha Aureolas,Frutexa Metallicum,Fungoida Setisis,Osseus Spiralis,Stratum Tectonicas,Tubus Sororibus,Tussock Divisa");

            var pleiadesSystem = new StarSystem { StarType = StarType.G, Address = 1, Name = "Pleiades Sector HR-V b2-0", Position = new(70, 0, -880) };
            var farSystem = new StarSystem { StarType = StarType.G, Address = 1, Name = "Far Nebula Negative", Position = new(1000, 0, 1000) };

            Check("Icy Argon nebula rule: Electricae Radialem present in Pleiades Sector",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.IcyBody;
                    b.Atmosphere = AtmosphereClass.Argon;
                    b.SurfaceGravity = 2.0;
                    b.SurfaceTemp = 100;
                    b.DistanceFromArrivalLs = 50;
                }), pleiadesSystem)) == "Bacterium Vesicula,Electricae Radialem,Fonticulua Campestris");

            Check("Icy Argon nebula rule: no Electricae Radialem far from nebulae",
                string.Join(",", PredictNames(PredictBody(b =>
                {
                    b.PlanetClass = PlanetClass.IcyBody;
                    b.Atmosphere = AtmosphereClass.Argon;
                    b.SurfaceGravity = 2.0;
                    b.SurfaceTemp = 100;
                    b.DistanceFromArrivalLs = 50;
                }), farSystem)) == "Bacterium Vesicula,Fonticulua Campestris");

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
            settings.OnBoardingComplete = true;
            settings.SaveSettings();

            settings.DeveloperMode = false;
            settings.MinimiseToTray = false;
            settings.UiScale = 1.0;
            settings.NotificationSettings.DisplayTime = 7;
            settings.SystemGridSetting.ExoValuableBodyValue = 20_000_000;
            settings.OnBoardingComplete = false;
            settings.LoadSettings();

            Check("settings round-trip DeveloperMode", settings.DeveloperMode);
            Check("settings round-trip MinimiseToTray", settings.MinimiseToTray);
            Check("settings round-trip UiScale", Math.Abs(settings.UiScale - 1.35) < 0.001);
            Check("settings round-trip NotificationSettings", settings.NotificationSettings.DisplayTime == 42);
            Check("settings round-trip SystemGridSetting", settings.SystemGridSetting.ExoValuableBodyValue == 9_999_999);
            Check("settings round-trip OnBoardingComplete", settings.OnBoardingComplete);

            // ── EDSM system-details flow: a live FSDJump to a system with unknown
            //    star class / value / body count must fetch from the (fake) EDSM
            //    service and raise OnSystemUpdatedFromEDSM. ──
            var edsmJournalDir = Path.Combine(Path.GetTempPath(), "odex_edsm_smoke");
            Directory.CreateDirectory(edsmJournalDir);
            foreach (var f in Directory.GetFiles(edsmJournalDir, "*.log")) File.Delete(f);
            File.WriteAllLines(Path.Combine(edsmJournalDir, "Journal.240201000000.01.log"),
            [
                "{\"timestamp\":\"2024-02-01T00:00:00Z\",\"event\":\"Fileheader\",\"part\":1,\"language\":\"English\"}",
                "{\"timestamp\":\"2024-02-01T00:00:01Z\",\"event\":\"LoadGame\",\"Commander\":\"EdSmCMDR\",\"Ship\":\"CobraMkIII\",\"GameMode\":\"Solo\",\"Credits\":1000000}"
            ]);

            var edsmParser = new JournalParserStore(persistedProvider, settings);
            var edsmExploration = new ExplorationDataStore(edsmParser, new FakeEdsmApiService(), persistedProvider,
                notifications, settings, exo, organicChecklist);
            int edsmUpdated = 0;
            edsmExploration.OnSystemUpdatedFromEDSM += (_, _) => Interlocked.Increment(ref edsmUpdated);
            edsmParser.ReadNewDirectory(edsmJournalDir);
            deadline = DateTime.UtcNow.AddSeconds(30);
            while (!edsmParser.IsLive && DateTime.UtcNow < deadline) Thread.Sleep(50);

            File.AppendAllText(Path.Combine(edsmJournalDir, "Journal.240201000000.01.log"),
                "{\"timestamp\":\"2024-02-01T00:00:02Z\",\"event\":\"FSDJump\",\"StarSystem\":\"EdSmSys\",\"SystemAddress\":4100000000000,\"StarPos\":[10.0,20.0,30.0],\"Body\":7,\"Bodies\":1,\"JumpDist\":40.0}\n");

            deadline = DateTime.UtcNow.AddSeconds(15);
            while (edsmExploration.CurrentSystem is null || edsmExploration.CurrentSystem.EstimatedValue == 0)
            {
                if (DateTime.UtcNow > deadline) break;
                Thread.Sleep(100);
            }

            var edsmSystem = edsmExploration.CurrentSystem;
            Check("EDSM flow fills estimated value", edsmSystem is { EstimatedValue: 123456 });
            Check("EDSM flow fills star class", edsmSystem is { StarType: StarType.G });
            Check("EDSM flow fills body count", edsmSystem is { BodyCount: 4, EdsmScannedBodyCount: 2, IsKnownToEDSM: true });
            Check("EDSM flow fills edsm url", edsmSystem is { EdsmUrl.Length: > 0 });
            Check("EDSM flow adds valuable bodies",
                edsmSystem?.SystemBodies.Any(b => b.BodyName == "EdSmSys A 3") == true);
            Check("EDSM flow raises OnSystemUpdatedFromEDSM", edsmUpdated >= 1);

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
        "{\"timestamp\":\"2024-01-01T00:00:11Z\",\"event\":\"SellExplorationData\",\"Systems\":[\"Testes\"],\"Discovered\":[\"Testes\"],\"BaseValue\":50000,\"Bonus\":10000,\"TotalEarnings\":60000}",
        "{\"timestamp\":\"2024-01-01T00:00:12Z\",\"event\":\"FSDJump\",\"StarSystem\":\"NextSys\",\"SystemAddress\":3103895106049,\"StarPos\":[-100.0,200.0,300.0],\"StarType\":\"G\",\"Body\":7,\"Bodies\":1,\"JumpDist\":350.0}"
    ];

    // In-memory EDSM fake: returns canned system details without any network call.
    sealed class FakeEdsmApiService : EdsmApiService
    {
        public override Task<ODUtils.Models.StarType> GetPrimaryStarClassAsync(string systemName)
            => Task.FromResult(ODUtils.Models.StarType.G);

        public override Task<EdsmSystemValue?> GetSystemValueAsync(string systemName)
            => Task.FromResult<EdsmSystemValue?>(new EdsmSystemValue
            {
                Url = "https://www.edsm.net/en/system/id/4100000000000/name/EdSmSys",
                EstimatedValueMapped = 123456,
                ValuableBodies =
                [
                    new EdsmBody { BodyId = 3, BodyName = "EdSmSys A 3" },
                    new EdsmBody { BodyId = 4, BodyName = "EdSmSys A 4" }
                ]
            });

        public override Task<(int Count, int Scanned)> GetBodyCountAsync(long systemAddress)
            => Task.FromResult((4, 2));
    }
}
