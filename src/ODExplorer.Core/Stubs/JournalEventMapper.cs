// Maps journal JSON lines to typed JournalEntry instances for the in-memory
// pipeline. Mirrors the event-args surface of EliteJournalReader so the real
// parser can replace it later.

using System;
using System.Collections.Generic;
using System.Globalization;
using EliteJournalReader;
using EliteJournalReader.Events;
using Newtonsoft.Json.Linq;
using ODUtils.Journal;
using ODUtils.Models;
using JournalEntry = EliteJournalReader.JournalEntry;

namespace ODExplorer.Journal
{
    public static class JournalEventMapper
    {
        // Tracks the most recently seen system position (FSDJump/Location/CarrierJump)
        // so CodexEntry events (which carry no coordinates) can be given a region.
        internal static double[] LastSystemPosition = Array.Empty<double>();

        public static JournalEntry? Map(string line, string filename, int commanderId)
        {
            JObject obj;
            try
            {
                obj = JObject.Parse(line);
            }
            catch
            {
                return null;
            }

            var eventName = obj["event"]?.ToString();
            if (string.IsNullOrEmpty(eventName))
                return null;

            var type = JournalTypeHelpers.FromString(eventName);
            var entry = new JournalEntry
            {
                Event = eventName,
                EventType = type,
                CommanderID = commanderId,
                TimeStamp = GetTimestamp(obj),
                Filename = filename,
                Offset = 0,
                OriginalEvent = obj
            };

            entry.EventData = MapEvent(obj, type);
            return entry;
        }

        private static object MapEvent(JObject obj, JournalTypeEnum type)
        {
            switch (type)
            {
                case JournalTypeEnum.Location:
                    return new LocationEvent.LocationEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        StarPos = GetStarPos(obj),
                        Body = Str(obj, "Body"),
                        BodyID = Long(obj, "BodyID"),
                        BodyType = GetBodyType(obj),
                        Docked = Bool(obj, "Docked"),
                        OnFoot = Bool(obj, "OnFoot"),
                        Latitude = Dbl(obj, "Latitude"),
                        Longitude = Dbl(obj, "Longitude"),
                        DistanceFromArrivalLS = Dbl(obj, "DistanceFromArrivalLS"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.FSDJump:
                    LastSystemPosition = GetStarPos(obj);
                    return new FSDJumpEvent.FSDJumpEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        StarPos = LastSystemPosition,
                        StarType = Str(obj, "StarType"),
                        Body = Long(obj, "Body"),
                        Bodies = Int(obj, "Bodies"),
                        JumpDist = Dbl(obj, "JumpDist"),
                        FuelUsed = Dbl(obj, "FuelUsed"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.CarrierJump:
                    LastSystemPosition = GetStarPos(obj);
                    return new CarrierJumpEvent.CarrierJumpEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        StarPos = LastSystemPosition,
                        Body = Str(obj, "Body"),
                        BodyID = Long(obj, "BodyID"),
                        BodyType = GetBodyType(obj),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.CarrierStats:
                    return new CarrierStatsEvent.CarrierStatsEventArgs
                    {
                        Name = Str(obj, "Name"),
                        Callsign = Str(obj, "Callsign"),
                        CarrierType = Str(obj, "CarrierType"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.CarrierLocation:
                    return new CarrierLocationEvent.CarrierLocationEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        CarrierType = Str(obj, "CarrierType"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.CarrierJumpRequest:
                    return new CarrierJumpRequestEvent.CarrierJumpRequestEventArgs
                    {
                        DepartureTime = DateTime.TryParse(Str(obj, "DepartureTime"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var d) ? d : GetTimestamp(obj),
                        CarrierType = Str(obj, "CarrierType"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.CarrierJumpCancelled:
                    return new CarrierJumpCancelledEvent.CarrierJumpCancelledEventArgs
                    {
                        CarrierType = Str(obj, "CarrierType"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.StartJump:
                    return new StartJumpEvent.StartJumpEventArgs
                    {
                        JumpType = Str(obj, "JumpType") == "Supercruise" ? JumpType.Supercruise : JumpType.Hyperspace,
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        StarClass = Str(obj, "StarClass"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.FSDTarget:
                    return new FSDTargetEvent.FSDTargetEventArgs
                    {
                        SystemAddress = Long(obj, "SystemAddress"),
                        Name = Str(obj, "Name"),
                        StarClass = Str(obj, "StarClass"),
                        RemainingJumpsInRoute = Int(obj, "RemainingJumpsInRoute"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.NavRoute:
                    return new NavRouteEvent.NavRouteEventArgs { Timestamp = GetTimestamp(obj) };
                case JournalTypeEnum.NavRouteClear:
                    return new NavRouteClearEvent.NavRoutClearEventArgs { Timestamp = GetTimestamp(obj) };
                case JournalTypeEnum.SupercruiseEntry:
                    return new SupercruiseEntryEvent.SupercruiseEntryEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.FSSDiscoveryScan:
                    return new FSSDiscoveryScanEvent.FSSDiscoveryScanEventArgs
                    {
                        SystemName = Str(obj, "SystemName"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Progress = Dbl(obj, "Progress"),
                        BodyCount = Int(obj, "BodyCount"),
                        NonBodyCount = Int(obj, "NonBodyCount"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.FSSAllBodiesFound:
                    return new FSSAllBodiesFoundEvent.FSSAllBodiesFoundEventArgs
                    {
                        SystemName = Str(obj, "SystemName"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Count = Int(obj, "Count"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.FSSBodySignals:
                    return new FSSBodySignalsEvent.FSSBodySignalsEventArgs
                    {
                        SystemAddress = Long(obj, "SystemAddress"),
                        BodyID = Long(obj, "BodyID"),
                        Signals = GetSignals(obj),
                        Genuses = GetGenuses(obj),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.ScanBaryCentre:
                    return new ScanBaryCentreEvent.ScanBaryCentreEventArgs
                    {
                        SystemName = Str(obj, "SystemName"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        BodyID = Long(obj, "BodyID"),
                        Barycentre = GetBarycentre(obj),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.Scan:
                    return new ScanEvent.ScanEventArgs
                    {
                        ScanType = GetScanType(obj),
                        BodyName = Str(obj, "BodyName"),
                        BodyID = Long(obj, "BodyID"),
                        Parents = GetParents(obj),
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        DistanceFromArrivalLS = Dbl(obj, "DistanceFromArrivalLS"),
                        StarType = Str(obj, "StarType"),
                        StarClass = Str(obj, "StarClass"),
                        StellarMass = Dbl(obj, "StellarMass"),
                        Radius = Dbl(obj, "Radius"),
                        AbsoluteMagnitude = Dbl(obj, "AbsoluteMagnitude"),
                        Age_MY = Dbl(obj, "Age_MY"),
                        MassEM = Dbl(obj, "MassEM"),
                        SurfaceGravity = Dbl(obj, "SurfaceGravity"),
                        RotationPeriod = Dbl(obj, "RotationPeriod"),
                        AxialTilt = Dbl(obj, "AxialTilt"),
                        OrbitalPeriod = Dbl(obj, "OrbitalPeriod"),
                        SemiMajorAxis = Dbl(obj, "SemiMajorAxis"),
                        Eccentricity = Dbl(obj, "Eccentricity"),
                        OrbitalInclination = Dbl(obj, "OrbitalInclination"),
                        Periapsis = Dbl(obj, "Periapsis"),
                        MeanAnomaly = Dbl(obj, "MeanAnomaly"),
                        AscendingNode = Dbl(obj, "AscendingNode"),
                        PlanetClass = Str(obj, "PlanetClass"),
                        Landable = Bool(obj, "Landable"),
                        TerraformState = Str(obj, "TerraformState"),
                        Atmosphere = Str(obj, "Atmosphere"),
                        AtmosphereType = Str(obj, "AtmosphereType"),
                        Volcanism = Str(obj, "Volcanism"),
                        SurfaceTemperature = Dbl(obj, "SurfaceTemperature"),
                        SurfacePressure = Dbl(obj, "SurfacePressure"),
                        WasDiscovered = Bool(obj, "WasDiscovered"),
                        WasMapped = Bool(obj, "WasMapped"),
                        Rings = GetRings(obj),
                        Signals = GetSignals(obj),
                        Materials = GetMaterials(obj),
                        Composition = GetComposition(obj),
                        AtmosphereComposition = GetAtmosphereComposition(obj),
                        ReserveLevel = Str(obj, "ReserveLevel"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.SAAScanComplete:
                    return new SAAScanCompleteEvent.SAAScanCompleteEventArgs
                    {
                        SystemName = Str(obj, "SystemName"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        BodyID = Long(obj, "BodyID"),
                        ProbesUsed = Int(obj, "ProbesUsed"),
                        EfficiencyTargetMet = Bool(obj, "EfficiencyTargetMet"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.SAASignalsFound:
                    return new SAASignalsFoundEvent.SAASignalsFoundEventArgs
                    {
                        SystemName = Str(obj, "SystemName"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        BodyID = Long(obj, "BodyID"),
                        Signals = GetSignals(obj),
                        Genuses = GetGenuses(obj),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.SellExplorationData:
                    return new SellExplorationDataEvent.SellExplorationDataEventArgs
                    {
                        Systems = GetStringArray(obj, "Systems"),
                        Discovered = GetDiscovered(obj),
                        BaseValue = Long(obj, "BaseValue"),
                        TotalEarnings = Long(obj, "TotalEarnings"),
                        Bonus = Long(obj, "Bonus"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.MultiSellExplorationData:
                    return new MultiSellExplorationDataEvent.MultiSellExplorationDataEventArgs
                    {
                        Discovered = GetDiscoveredMulti(obj),
                        BaseValue = Long(obj, "BaseValue"),
                        TotalEarnings = Long(obj, "TotalEarnings"),
                        Bonus = Long(obj, "Bonus"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.Disembark:
                    return new DisembarkEvent.DisembarkEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Body = Str(obj, "Body"),
                        BodyID = Long(obj, "BodyID"),
                        OnStation = Bool(obj, "OnStation"),
                        OnPlanet = Bool(obj, "OnPlanet"),
                        Latitude = Dbl(obj, "Latitude"),
                        Longitude = Dbl(obj, "Longitude"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.Embark:
                    return new EmbarkEvent.EmbarkEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Body = Str(obj, "Body"),
                        BodyID = Long(obj, "BodyID"),
                        OnStation = Bool(obj, "OnStation"),
                        OnPlanet = Bool(obj, "OnPlanet"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.Died:
                    return new DiedEvent.DiedEventArgs
                    {
                        KillerName = Str(obj, "KillerName"),
                        KillerShip = Str(obj, "KillerShip"),
                        KillerRank = Str(obj, "KillerRank"),
                        KillingBlow = Bool(obj, "KillingBlow"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.ApproachBody:
                    return new ApproachBodyEvent.ApproachBodyEventArgs
                    {
                        StarSystem = Str(obj, "StarSystem"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Body = Str(obj, "Body"),
                        BodyID = Long(obj, "BodyID"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.CodexEntry:
                    return new CodexEntryEvent.CodexEntryEventArgs
                    {
                        System = Str(obj, "System"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Body = Str(obj, "Body"),
                        BodyID = Long(obj, "BodyID"),
                        Name = Str(obj, "Name"),
                        Name_Localised = Str(obj, "Name_Localised"),
                        Category = Str(obj, "Category"),
                        SubCategory = Str(obj, "SubCategory"),
                        Region = GetCodexRegion(obj),
                        Latitude = Dbl(obj, "Latitude"),
                        Longitude = Dbl(obj, "Longitude"),
                        IsNewEntry = Bool(obj, "IsNewEntry"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.ScanOrganic:
                    return new ScanOrganicEvent.ScanOrganicEventArgs
                    {
                        ScanType = Str(obj, "ScanType") == "Analyse" ? OrganicScanStage.Analyse : OrganicScanStage.Log,
                        Genus = Str(obj, "Genus"),
                        Genus_Localised = Str(obj, "Genus_Localised"),
                        Species = Str(obj, "Species"),
                        Species_Localised = Str(obj, "Species_Localised"),
                        Variant = Str(obj, "Variant"),
                        Variant_Localised = Str(obj, "Variant_Localised"),
                        SystemAddress = Long(obj, "SystemAddress"),
                        Body = Long(obj, "Body"),
                        Latitude = Dbl(obj, "Latitude"),
                        Longitude = Dbl(obj, "Longitude"),
                        Timestamp = GetTimestamp(obj)
                    };
                case JournalTypeEnum.SellOrganicData:
                    return new SellOrganicDataEvent.SellOrganicDataEventArgs
                    {
                        MarketID = Long(obj, "MarketID"),
                        BioData = GetBioData(obj),
                        Timestamp = GetTimestamp(obj)
                    };
                default:
                    return new object();
            }
        }

        private static DateTime GetTimestamp(JObject obj)
        {
            if (DateTime.TryParse(Str(obj, "timestamp"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var ts))
            {
                return ts;
            }
            return DateTime.UtcNow;
        }

        private static string Str(JObject obj, string key)
        {
            return obj[key]?.ToString() ?? string.Empty;
        }

        private static bool Bool(JObject obj, string key)
        {
            return obj[key]?.Value<bool>() ?? false;
        }

        private static int Int(JObject obj, string key)
        {
            return obj[key]?.Value<int>() ?? 0;
        }

        private static long Long(JObject obj, string key)
        {
            return obj[key]?.Value<long>() ?? 0;
        }

        private static double Dbl(JObject obj, string key)
        {
            return obj[key]?.Value<double>() ?? 0;
        }

        private static double[] GetStarPos(JObject obj)
        {
            var starPos = obj["StarPos"];
            if (starPos is not JArray array || array.Count < 3)
                return Array.Empty<double>();

            return
            [
                array[0].Value<double>(),
                array[1].Value<double>(),
                array[2].Value<double>()
            ];
        }

        private static BodyType GetBodyType(JObject obj)
        {
            return Str(obj, "BodyType") switch
            {
                "Star" => BodyType.Star,
                "Planet" => BodyType.Planet,
                "Station" => BodyType.Station,
                "Taxi" => BodyType.Taxi,
                "Fighter" => BodyType.Fighter,
                "SRV" => BodyType.SRV,
                "FleetCarrier" => BodyType.FleetCarrier,
                _ => BodyType.Unknown
            };
        }

        private static ScanType GetScanType(JObject obj)
        {
            return Str(obj, "ScanType") switch
            {
                "AutoScan" => ScanType.AutoScan,
                "NavBeacon" => ScanType.NavBeacon,
                "NavBeaconDetail" => ScanType.NavBeaconDetail,
                _ => ScanType.Detailed
            };
        }

        private static List<SignalFound> GetSignals(JObject obj)
        {
            var ret = new List<SignalFound>();
            if (obj["Signals"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new SignalFound
                    {
                        Type = token["Type"]?.ToString() ?? string.Empty,
                        Type_Localised = token["Type_Localised"]?.ToString() ?? string.Empty,
                        Count = token["Count"]?.Value<int>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static List<string> GetGenuses(JObject obj)
        {
            var ret = new List<string>();
            if (obj["Genuses"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(token.ToString());
                }
            }
            return ret;
        }

        private static List<JournalParent> GetParents(JObject obj)
        {
            var ret = new List<JournalParent>();
            if (obj["Parents"] is JArray array)
            {
                foreach (var token in array)
                {
                    var parent = new JournalParent();
                    if (token["Star"] != null)
                    {
                        parent.Star = token["Star"].Value<long>();
                    }
                    if (token["Planet"] != null)
                    {
                        parent.Planet = token["Planet"].Value<long>();
                    }
                    ret.Add(parent);
                }
            }
            return ret;
        }

        private static List<PlanetRing> GetRings(JObject obj)
        {
            var ret = new List<PlanetRing>();
            if (obj["Rings"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new PlanetRing
                    {
                        Name = token["Name"]?.ToString() ?? string.Empty,
                        RingClass = token["RingClass"]?.ToString() ?? string.Empty,
                        MassMT = token["MassMT"]?.Value<double>() ?? 0,
                        InnerRad = token["InnerRad"]?.Value<double>() ?? 0,
                        OuterRad = token["OuterRad"]?.Value<double>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static List<ScanEvent.ShipMaterialsEntry> GetMaterials(JObject obj)
        {
            var ret = new List<ScanEvent.ShipMaterialsEntry>();
            if (obj["Materials"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new ScanEvent.ShipMaterialsEntry
                    {
                        Name = token["Name"]?.ToString() ?? string.Empty,
                        Percent = token["Percent"]?.Value<double>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static ScanEvent.CompositionEntry? GetComposition(JObject obj)
        {
            if (obj["Composition"] is not JObject comp)
                return null;

            return new ScanEvent.CompositionEntry
            {
                Ice = comp["Ice"]?.Value<double>() ?? 0,
                Rock = comp["Rock"]?.Value<double>() ?? 0,
                Metal = comp["Metal"]?.Value<double>() ?? 0
            };
        }

        private static List<ScanEvent.ScanItemComponentEntry> GetAtmosphereComposition(JObject obj)
        {
            var ret = new List<ScanEvent.ScanItemComponentEntry>();
            if (obj["AtmosphereComposition"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new ScanEvent.ScanItemComponentEntry
                    {
                        Name = token["Name"]?.ToString() ?? string.Empty,
                        Percent = token["Percent"]?.Value<double>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static List<ScanBarycentreBody> GetBarycentre(JObject obj)
        {
            var ret = new List<ScanBarycentreBody>();
            if (obj["Barycentre"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new ScanBarycentreBody
                    {
                        BodyID = token["BodyID"]?.Value<long>() ?? 0,
                        SemiMajorAxis = token["SemiMajorAxis"]?.Value<double>() ?? 0,
                        Eccentricity = token["Eccentricity"]?.Value<double>() ?? 0,
                        OrbitalInclination = token["OrbitalInclination"]?.Value<double>() ?? 0,
                        Periapsis = token["Periapsis"]?.Value<double>() ?? 0,
                        OrbitalPeriod = token["OrbitalPeriod"]?.Value<double>() ?? 0,
                        AscendingNode = token["AscendingNode"]?.Value<double>() ?? 0,
                        MeanAnomaly = token["MeanAnomaly"]?.Value<double>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static List<string> GetStringArray(JObject obj, string key)
        {
            var ret = new List<string>();
            if (obj[key] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(token.ToString());
                }
            }
            return ret;
        }

        private static List<SellExplorationDataEvent.SystemDiscoveredEntry> GetDiscovered(JObject obj)
        {
            var ret = new List<SellExplorationDataEvent.SystemDiscoveredEntry>();
            if (obj["Discovered"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new SellExplorationDataEvent.SystemDiscoveredEntry
                    {
                        SystemName = token["SystemName"]?.ToString() ?? string.Empty,
                        NumBodies = token["NumBodies"]?.Value<int>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static List<MultiSellExplorationDataEvent.SystemDiscoveredEntry> GetDiscoveredMulti(JObject obj)
        {
            var ret = new List<MultiSellExplorationDataEvent.SystemDiscoveredEntry>();
            if (obj["Discovered"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new MultiSellExplorationDataEvent.SystemDiscoveredEntry
                    {
                        SystemName = token["SystemName"]?.ToString() ?? string.Empty,
                        NumBodies = token["NumBodies"]?.Value<int>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static List<SellOrganicDataEvent.OrganicSoldEntry> GetBioData(JObject obj)
        {
            var ret = new List<SellOrganicDataEvent.OrganicSoldEntry>();
            if (obj["BioData"] is JArray array)
            {
                foreach (var token in array)
                {
                    ret.Add(new SellOrganicDataEvent.OrganicSoldEntry
                    {
                        Name = token["Name"]?.ToString() ?? string.Empty,
                        Name_Localised = token["Name_Localised"]?.ToString() ?? string.Empty,
                        Genus = token["Genus"]?.ToString() ?? string.Empty,
                        Species = token["Species"]?.ToString() ?? string.Empty,
                        Variant = token["Variant"]?.ToString() ?? string.Empty,
                        Value = token["Value"]?.Value<long>() ?? 0,
                        Bonus = token["Bonus"]?.Value<long>() ?? 0,
                        TotalValue = token["TotalValue"]?.Value<long>() ?? 0
                    });
                }
            }
            return ret;
        }

        private static GalacticRegions GetCodexRegion(JObject obj)
        {
            var raw = obj["Region"];
            if (raw is null && LastSystemPosition.Length == 3)
            {
                return (GalacticRegions)ODUtils.EliteDangerousHelpers.GalacticRegions.RegionMap.FindRegion(
                    LastSystemPosition[0], LastSystemPosition[1], LastSystemPosition[2]).Id;
            }

            // Journal "Region" is sometimes a numeric id, sometimes a name string.
            var text = raw?.ToString() ?? string.Empty;
            if (int.TryParse(text, out var id))
            {
                if (id >= 0 && id <= 3)
                    return (GalacticRegions)id;
            }

            if (Enum.TryParse<GalacticRegions>(text, true, out var parsed))
                return parsed;

            if (LastSystemPosition.Length == 3)
            {
                return (GalacticRegions)ODUtils.EliteDangerousHelpers.GalacticRegions.RegionMap.FindRegion(
                    LastSystemPosition[0], LastSystemPosition[1], LastSystemPosition[2]).Id;
            }

            return GalacticRegions.Unknown;
        }

        // ── StarSystem / SystemBody value helpers (shared with the stores) ────────

        public static StarType GetStarType(string? starType)
        {
            if (string.IsNullOrEmpty(starType))
                return StarType.Unknown;

            return starType switch
            {
                "T Tauri" => StarType.TTS,
                "Herbig Ae/Be" => StarType.AeBe,
                "Wolf-Rayet" => StarType.W,
                "Wolf-Rayet N" => StarType.WN,
                "Wolf-Rayet NC" => StarType.WNC,
                "Wolf-Rayet C" => StarType.WC,
                "Wolf-Rayet O" => StarType.WO,
                "Carbon Star" => StarType.C,
                "C-N" => StarType.CN,
                "C-J" => StarType.CJ,
                "C-H" => StarType.CH,
                "C-Hd" => StarType.CHd,
                "MS-type" => StarType.MS,
                "S-type" => StarType.S,
                "Neutron Star" => StarType.N,
                "Black Hole" => StarType.BH,
                "Supergiant" => StarType.X,
                _ => Enum.TryParse<StarType>(starType, true, out var st) ? st : StarType.Unknown
            };
        }

        public static StarLuminosityClass GetStarLuminosity(string? luminosity)
        {
            if (string.IsNullOrEmpty(luminosity))
                return StarLuminosityClass.Unknown;

            return luminosity switch
            {
                "Ia" => StarLuminosityClass.Ia,
                "Ib" => StarLuminosityClass.Ib,
                "Iab" => StarLuminosityClass.Ib,
                "I" => StarLuminosityClass.Ia,
                "II" => StarLuminosityClass.II,
                "III" => StarLuminosityClass.III,
                "IIIa" => StarLuminosityClass.IIIa,
                "IIIb" => StarLuminosityClass.IIIb,
                "IV" => StarLuminosityClass.IV,
                "V" => StarLuminosityClass.V,
                "Va" => StarLuminosityClass.Va,
                "Vb" => StarLuminosityClass.Vb,
                "Vz" => StarLuminosityClass.Vz,
                "VI" => StarLuminosityClass.VI,
                "VII" => StarLuminosityClass.VII,
                _ => Enum.TryParse<StarLuminosityClass>(luminosity, true, out var lc) ? lc : StarLuminosityClass.Unknown
            };
        }

        public static PlanetClass GetPlanetClass(string? planetClass)
        {
            if (string.IsNullOrEmpty(planetClass))
                return PlanetClass.Unknown;

            return planetClass switch
            {
                "Earthlike body" => PlanetClass.EarthlikeBody,
                "Water world" => PlanetClass.WaterWorld,
                "Ammonia world" => PlanetClass.AmmoniaWorld,
                "High metal content body" => PlanetClass.HighMetalContentBody,
                "Metal-rich body" => PlanetClass.MetalRichBody,
                "Rocky body" => PlanetClass.RockyBody,
                "Rocky ice world" => PlanetClass.RockyIceBody,
                "Icy body" => PlanetClass.IcyBody,
                "Sudarsky class I gas giant" => PlanetClass.SudarskyClassIGasGiant,
                "Sudarsky class II gas giant" => PlanetClass.SudarskyClassIIGasGiant,
                "Sudarsky class III gas giant" => PlanetClass.SudarskyClassIIIGasGiant,
                "Sudarsky class IV gas giant" => PlanetClass.SudarskyClassIVGasGiant,
                "Sudarsky class V gas giant" => PlanetClass.SudarskyClassVGasGiant,
                "Gas giant with water-based life" => PlanetClass.GasGiantWithWaterBasedLife,
                "Gas giant with ammonia-based life" => PlanetClass.GasGiantWithAmmoniaBasedLife,
                "Water giant" => PlanetClass.WaterGiant,
                "Water giant with life" => PlanetClass.WaterGiantWithLife,
                "Helium-rich gas giant" => PlanetClass.HeliumRichGasGiant,
                "Helium gas giant" => PlanetClass.HeliumGasGiant,
                _ => Enum.TryParse<PlanetClass>(planetClass, true, out var pc) ? pc : PlanetClass.Unknown
            };
        }

        public static AtmosphereClass GetAtmosphereClass(string? atmosphere)
        {
            if (string.IsNullOrEmpty(atmosphere))
                return AtmosphereClass.None;

            var a = atmosphere.ToLowerInvariant();

            if (a.Contains("suitable for water-based life")) return AtmosphereClass.SuitableForWaterBasedLife;
            if (a.Contains("ammonia") && a.Contains("oxygen")) return AtmosphereClass.AmmoniaOxygen;
            if (a.Contains("ammonia-rich")) return AtmosphereClass.AmmoniaRich;
            if (a.Contains("ammonia")) return AtmosphereClass.Ammonia;
            if (a.Contains("earthlike")) return AtmosphereClass.EarthLike;
            if (a.Contains("water-rich")) return AtmosphereClass.WaterRich;
            if (a.Contains("water")) return AtmosphereClass.Water;
            if (a.Contains("carbon dioxide-rich")) return AtmosphereClass.CarbonDioxideRich;
            if (a.Contains("carbon dioxide")) return AtmosphereClass.CarbonDioxide;
            if (a.Contains("sulphur dioxide")) return AtmosphereClass.SulphurDioxide;
            if (a.Contains("methane-rich")) return AtmosphereClass.MethaneRich;
            if (a.Contains("methane")) return AtmosphereClass.Methane;
            if (a.Contains("nitrogen")) return AtmosphereClass.Nitrogen;
            if (a.Contains("neon-rich")) return AtmosphereClass.NeonRich;
            if (a.Contains("neon")) return AtmosphereClass.Neon;
            if (a.Contains("argon-rich")) return AtmosphereClass.ArgonRich;
            if (a.Contains("argon")) return AtmosphereClass.Argon;
            if (a.Contains("oxygen")) return AtmosphereClass.Oxygen;
            if (a.Contains("helium")) return AtmosphereClass.Helium;
            if (a.Contains("silicate vapour")) return AtmosphereClass.SilicateVapour;
            if (a.Contains("metallic vapour")) return AtmosphereClass.MetallicVapour;
            if (a.Contains("no atmosphere")) return AtmosphereClass.NoAtmosphere;
            return AtmosphereClass.Unknown;
        }

        public static AtmosphereClass GetAtmosphereType(string? atmosphereType)
        {
            if (string.IsNullOrEmpty(atmosphereType))
                return AtmosphereClass.None;

            return Enum.TryParse<AtmosphereClass>(atmosphereType, true, out var at) ? at : AtmosphereClass.Unknown;
        }

        public static VolcanismType GetVolcanism(string? volcanism)
        {
            if (string.IsNullOrEmpty(volcanism) || volcanism.Equals("None", StringComparison.OrdinalIgnoreCase))
                return VolcanismType.None;

            var v = volcanism.ToLowerInvariant();
            bool major = v.Contains("major");
            bool minor = v.Contains("minor");

            if (v.Contains("rocky")) return major ? VolcanismType.MajorRocky : minor ? VolcanismType.MinorRocky : VolcanismType.Rocky;
            if (v.Contains("metallic")) return major ? VolcanismType.MajorMetallic : minor ? VolcanismType.MinorMetallic : VolcanismType.Metallic;
            if (v.Contains("carbon")) return major ? VolcanismType.MajorCarbon : minor ? VolcanismType.MinorCarbon : VolcanismType.Carbon;
            if (v.Contains("water")) return VolcanismType.Water;
            if (v.Contains("ammonia")) return VolcanismType.Ammonia;
            if (v.Contains("nitrogen")) return VolcanismType.Nitrogen;
            if (v.Contains("silicate")) return VolcanismType.Silicate;
            if (v.Contains("iron")) return VolcanismType.Iron;
            return VolcanismType.None;
        }

        public static bool IsTerraformable(string? terraformState)
        {
            if (string.IsNullOrEmpty(terraformState))
                return false;

            return terraformState.Equals("Terraformable", StringComparison.OrdinalIgnoreCase) ||
                   terraformState.Contains("Candidate for terraforming", StringComparison.OrdinalIgnoreCase);
        }

        // Approximate first-discovery FSS value per body type.
        public static long GetFssValue(PlanetClass planetClass)
        {
            return planetClass switch
            {
                PlanetClass.EarthlikeBody => 1_172_950,
                PlanetClass.WaterWorld => 780_250,
                PlanetClass.AmmoniaWorld => 780_250,
                PlanetClass.WaterGiantWithLife => 103_800,
                PlanetClass.WaterGiant => 77_850,
                PlanetClass.GasGiantWithWaterBasedLife or PlanetClass.GasGiantWithAmmoniaBasedLife => 77_850,
                PlanetClass.SudarskyClassIVGasGiant or PlanetClass.SudarskyClassVGasGiant => 77_850,
                PlanetClass.SudarskyClassIGasGiant or PlanetClass.SudarskyClassIIIGasGiant => 51_900,
                PlanetClass.HeliumRichGasGiant or PlanetClass.HeliumGasGiant => 155_700,
                PlanetClass.SudarskyClassIIGasGiant => 38_900,
                PlanetClass.MetalRichBody => 38_900,
                PlanetClass.HighMetalContentBody => 25_950,
                PlanetClass.RockyIceBody => 5_190,
                PlanetClass.RockyBody => 6_490,
                PlanetClass.IcyBody => 3_890,
                _ => 0
            };
        }
    }
}
