// Journal pipeline types for EliteJournalReader namespaces.
// These are functional in-memory stand-ins for the real (excluded) EliteJournalReader
// library. They mirror the event-argument shape of the real library closely enough that
// the real stores can be swapped in later for full parity.

using System;
using System.Collections.Generic;

namespace EliteJournalReader
{
    public sealed class JournalEntry
    {
        public JournalEntry() { }

        public JournalEntry(string filename, long offset, int commanderID,
            ODUtils.Journal.JournalTypeEnum type, object eventData,
            Newtonsoft.Json.Linq.JToken? originalEvent)
        {
            Filename = filename;
            Offset = offset;
            CommanderID = commanderID;
            EventType = type;
            EventData = eventData;
            OriginalEvent = originalEvent;
        }

        public string Event { get; set; } = string.Empty;
        public object EventData { get; set; } = new object();
        public Newtonsoft.Json.Linq.JToken? OriginalEvent { get; set; }
        public ODUtils.Journal.JournalTypeEnum EventType { get; set; }
        public int CommanderID { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Filename { get; set; } = string.Empty;
        public long Offset { get; set; }
    }
}

namespace EliteJournalReader.Events
{
    public enum BodyType { Unknown = 0, Star, Planet, Station, Taxi, Fighter, SRV, FleetCarrier }
    public enum ScanType { Detailed = 0, AutoScan, NavBeacon, NavBeaconDetail }
    public enum JumpType { Hyperspace, Supercruise }
    public enum ParentType { Null, Planet, Star }

    public struct PlanetRing
    {
        public string Name { get; set; }
        public string RingClass { get; set; }
        public double MassMT { get; set; }
        public double InnerRad { get; set; }
        public double OuterRad { get; set; }
    }

    public sealed class SignalFound
    {
        public string Type { get; set; } = string.Empty;
        public string Type_Localised { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsBiological => Type == "$SAA_SignalType_Biological;";
    }

    public sealed class JournalParent
    {
        public long Star { get; set; }
        public long Planet { get; set; }
    }

    public sealed class ScanBarycentreBody
    {
        public long BodyID { get; set; }
        public double SemiMajorAxis { get; set; }
        public double Eccentricity { get; set; }
        public double OrbitalInclination { get; set; }
        public double Periapsis { get; set; }
        public double OrbitalPeriod { get; set; }
        public double AscendingNode { get; set; }
        public double MeanAnomaly { get; set; }
    }

    // ── Ship / carrier location events ──────────────────────────────────────────
    public sealed class CarrierLocationEvent
    {
        public sealed class CarrierLocationEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public string CarrierType { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class CarrierStatsEvent
    {
        public sealed class CarrierStatsEventArgs
        {
            public string Name { get; set; } = string.Empty;
            public string Callsign { get; set; } = string.Empty;
            public string CarrierType { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class CarrierJumpRequestEvent
    {
        public sealed class CarrierJumpRequestEventArgs
        {
            public DateTime DepartureTime { get; set; } = DateTime.UtcNow;
            public string CarrierType { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class CarrierJumpCancelledEvent
    {
        public sealed class CarrierJumpCancelledEventArgs
        {
            public string CarrierType { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }
    }

    // ── System / location events ────────────────────────────────────────────────
    public sealed class FSDJumpEvent
    {
        public sealed class FSDJumpEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public double[] StarPos { get; set; } = Array.Empty<double>();
            public string StarType { get; set; } = string.Empty;
            public long Body { get; set; }
            public int Bodies { get; set; }
            public double JumpDist { get; set; }
            public double FuelUsed { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class LocationEvent
    {
        public sealed class LocationEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public double[] StarPos { get; set; } = Array.Empty<double>();
            public string Body { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public BodyType BodyType { get; set; }
            public bool Docked { get; set; }
            public bool OnFoot { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double DistanceFromArrivalLS { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class CarrierJumpEvent
    {
        public sealed class CarrierJumpEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public double[] StarPos { get; set; } = Array.Empty<double>();
            public string Body { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public BodyType BodyType { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class StartJumpEvent
    {
        public sealed class StartJumpEventArgs
        {
            public JumpType JumpType { get; set; }
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public string StarClass { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class FSDTargetEvent
    {
        public sealed class FSDTargetEventArgs
        {
            public long SystemAddress { get; set; }
            public string Name { get; set; } = string.Empty;
            public string StarClass { get; set; } = string.Empty;
            public int RemainingJumpsInRoute { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class NavRouteEvent
    {
        public sealed class NavRouteEventArgs { public DateTime Timestamp { get; set; } }
    }

    public sealed class NavRouteClearEvent
    {
        public sealed class NavRoutClearEventArgs { public DateTime Timestamp { get; set; } }
    }

    public sealed class SupercruiseEntryEvent
    {
        public sealed class SupercruiseEntryEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    // ── FSS events ──────────────────────────────────────────────────────────────
    public sealed class FSSDiscoveryScanEvent
    {
        public sealed class FSSDiscoveryScanEventArgs
        {
            public string SystemName { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public double Progress { get; set; }
            public int BodyCount { get; set; }
            public int NonBodyCount { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class FSSAllBodiesFoundEvent
    {
        public sealed class FSSAllBodiesFoundEventArgs
        {
            public string SystemName { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public int Count { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class FSSBodySignalsEvent
    {
        public sealed class FSSBodySignalsEventArgs
        {
            public long SystemAddress { get; set; }
            public long BodyID { get; set; }
            public List<SignalFound> Signals { get; set; } = [];
            public List<string> Genuses { get; set; } = [];
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class ScanBaryCentreEvent
    {
        public sealed class ScanBaryCentreEventArgs
        {
            public string SystemName { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public long BodyID { get; set; }
            public List<ScanBarycentreBody> Barycentre { get; set; } = [];
            public DateTime Timestamp { get; set; }
        }
    }

    // ── Body scan events ────────────────────────────────────────────────────────
    public sealed class ScanEvent
    {
        public sealed class ScanEventArgs
        {
            public ScanType ScanType { get; set; }
            public string BodyName { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public List<JournalParent> Parents { get; set; } = [];
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public double DistanceFromArrivalLS { get; set; }
            public string StarType { get; set; } = string.Empty;
            public string StarClass { get; set; } = string.Empty;
            public double StellarMass { get; set; }
            public double Radius { get; set; }
            public double AbsoluteMagnitude { get; set; }
            public double Age_MY { get; set; }
            public double MassEM { get; set; }
            public double SurfaceGravity { get; set; }
            public double RotationPeriod { get; set; }
            public double AxialTilt { get; set; }
            public double OrbitalPeriod { get; set; }
            public double SemiMajorAxis { get; set; }
            public double Eccentricity { get; set; }
            public double OrbitalInclination { get; set; }
            public double Periapsis { get; set; }
            public double MeanAnomaly { get; set; }
            public double AscendingNode { get; set; }
            public string PlanetClass { get; set; } = string.Empty;
            public bool Landable { get; set; }
            public string TerraformState { get; set; } = string.Empty;
            public string Atmosphere { get; set; } = string.Empty;
            public string AtmosphereType { get; set; } = string.Empty;
            public string Volcanism { get; set; } = string.Empty;
            public double SurfaceTemperature { get; set; }
            public double SurfacePressure { get; set; }
            public bool WasDiscovered { get; set; }
            public bool WasMapped { get; set; }
            public List<PlanetRing> Rings { get; set; } = [];
            public List<SignalFound> Signals { get; set; } = [];
            public List<ShipMaterialsEntry> Materials { get; set; } = [];
            public CompositionEntry? Composition { get; set; }
            public List<ScanItemComponentEntry> AtmosphereComposition { get; set; } = [];
            public string ReserveLevel { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        public sealed class ShipMaterialsEntry { public string Name { get; set; } = string.Empty; public double Percent { get; set; } }
        public sealed class CompositionEntry { public double Ice { get; set; } public double Rock { get; set; } public double Metal { get; set; } }
        public sealed class ScanItemComponentEntry { public string Name { get; set; } = string.Empty; public double Percent { get; set; } }
    }

    public sealed class SAAScanCompleteEvent
    {
        public sealed class SAAScanCompleteEventArgs
        {
            public string SystemName { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public long BodyID { get; set; }
            public int ProbesUsed { get; set; }
            public bool EfficiencyTargetMet { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class SAASignalsFoundEvent
    {
        public sealed class SAASignalsFoundEventArgs
        {
            public string SystemName { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public long BodyID { get; set; }
            public List<SignalFound> Signals { get; set; } = [];
            public List<string> Genuses { get; set; } = [];
            public DateTime Timestamp { get; set; }
        }
    }

    // ── Sell / death events ─────────────────────────────────────────────────────
    public sealed class SellExplorationDataEvent
    {
        public sealed class SellExplorationDataEventArgs
        {
            public List<string> Systems { get; set; } = [];
            public List<SystemDiscoveredEntry> Discovered { get; set; } = [];
            public long BaseValue { get; set; }
            public long TotalEarnings { get; set; }
            public long Bonus { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public sealed class SystemDiscoveredEntry { public string SystemName { get; set; } = string.Empty; public int NumBodies { get; set; } }
    }

    public sealed class MultiSellExplorationDataEvent
    {
        public sealed class MultiSellExplorationDataEventArgs
        {
            public List<SystemDiscoveredEntry> Discovered { get; set; } = [];
            public long BaseValue { get; set; }
            public long TotalEarnings { get; set; }
            public long Bonus { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public sealed class SystemDiscoveredEntry { public string SystemName { get; set; } = string.Empty; public int NumBodies { get; set; } }
    }

    public sealed class DiedEvent
    {
        public sealed class DiedEventArgs
        {
            public string KillerName { get; set; } = string.Empty;
            public string KillerShip { get; set; } = string.Empty;
            public string KillerRank { get; set; } = string.Empty;
            public bool KillingBlow { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    // ── On-foot / approach events ───────────────────────────────────────────────
    public sealed class DisembarkEvent
    {
        public sealed class DisembarkEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public string Body { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public bool OnStation { get; set; }
            public bool OnPlanet { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class EmbarkEvent
    {
        public sealed class EmbarkEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public string Body { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public bool OnStation { get; set; }
            public bool OnPlanet { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class ApproachBodyEvent
    {
        public sealed class ApproachBodyEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public string Body { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    // ── Exobiology / codex events ───────────────────────────────────────────────
    public sealed class CodexEntryEvent
    {
        public sealed class CodexEntryEventArgs
        {
            public string System { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public string Body { get; set; } = string.Empty;
            public long BodyID { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Name_Localised { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string SubCategory { get; set; } = string.Empty;
            public ODUtils.Models.GalacticRegions Region { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public bool IsNewEntry { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class ScanOrganicEvent
    {
        public sealed class ScanOrganicEventArgs
        {
            public ODUtils.Models.OrganicScanStage ScanType { get; set; }
            public string Genus { get; set; } = string.Empty;
            public string Genus_Localised { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Species_Localised { get; set; } = string.Empty;
            public string Variant { get; set; } = string.Empty;
            public string Variant_Localised { get; set; } = string.Empty;
            public long SystemAddress { get; set; }
            public long Body { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }

    public sealed class SellOrganicDataEvent
    {
        public sealed class SellOrganicDataEventArgs
        {
            public long MarketID { get; set; }
            public List<OrganicSoldEntry> BioData { get; set; } = [];
            public DateTime Timestamp { get; set; }
        }

        public sealed class OrganicSoldEntry
        {
            public string Name { get; set; } = string.Empty;
            public string Name_Localised { get; set; } = string.Empty;
            public string Genus { get; set; } = string.Empty;
            public string Species { get; set; } = string.Empty;
            public string Variant { get; set; } = string.Empty;
            public long Value { get; set; }
            public long Bonus { get; set; }
            public long TotalValue { get; set; }
        }
    }
}
