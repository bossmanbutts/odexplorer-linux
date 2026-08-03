// Lightweight stubs to satisfy references to ODUtils.Models used in the core. These are temporary; UI/host should provide real models or map via adapters.

using System.Collections.Generic;

namespace ODUtils.Models
{
    // ── Regions ────────────────────────────────────────────────────────────────
    public enum GalacticRegions { Unknown = 0, Core, Bubble, OuterRim }

    public sealed class SystemRegion
    {
        public string Name { get; set; } = string.Empty;
    }

    // ── Position ───────────────────────────────────────────────────────────────
    public readonly record struct Position(double X, double Y, double Z)
    {
        public double DistanceFrom(Position other)
        {
            var dx = X - other.X; var dy = Y - other.Y; var dz = Z - other.Z;
            return System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }

    // ── DataState ──────────────────────────────────────────────────────────────
    public enum DataState { Unsold = 0, Sold, Lost }

    // ── DiscoveryStatus ────────────────────────────────────────────────────────
    public enum DiscoveryStatus { Unknown = 0, WorthMapping, Discovered }

    // ── PlanetClass ────────────────────────────────────────────────────────────
    public enum PlanetClass
    {
        Unknown = 0, EdsmValuableBody, IcyBody, RockyBody, RockyIceBody,
        MetalRichBody, HighMetalContentBody, EarthlikeBody, WaterWorld,
        WaterGiant, WaterGiantWithLife, AmmoniaWorld, GasGiantWithWaterBasedLife,
        GasGiantWithAmmoniaBasedLife, SudarskyClassIGasGiant, SudarskyClassIIGasGiant,
        SudarskyClassIIIGasGiant, SudarskyClassIVGasGiant, SudarskyClassVGasGiant,
        HeliumRichGasGiant, HeliumGasGiant
    }

    // ── StarType ───────────────────────────────────────────────────────────────
    public enum StarType { Unknown = 0, O, B, A, F, G, K, M, L, T, Y, TTS, AeBe, W, WN, WNC, WC, WO, CS, C, CN, CJ, CH, CHd, MS, S, D, DA, DAB, DAO, DAZ, DAV, DB, DBZ, DBV, DO, DOV, DQ, DC, DCV, DX, N, BH, H, X }

    // ── PlanetMaterial ─────────────────────────────────────────────────────────
    [System.Flags]
    public enum PlanetMaterial
    {
        None = 0, carbon = 1, vanadium = 2, germanium = 4,
        cadmium = 8, niobium = 16, yttrium = 32, polonium = 64
    }

    // ── AtmosphereClass ────────────────────────────────────────────────────────
    public enum AtmosphereClass
    {
        Unknown = 0, None, NoAtmosphere, SuitableForWaterBasedLife, AmmoniaOxygen,
        Ammonia, EarthLike, Water, CarbonDioxide, SulphurDioxide, Nitrogen,
        WaterRich, MethaneRich, AmmoniaRich, CarbonDioxideRich, Methane,
        Helium, SilicateVapour, MetallicVapour, NeonRich, ArgonRich, Neon, Argon, Oxygen
    }

    // ── VolcanismType ──────────────────────────────────────────────────────────
    public enum VolcanismType { None = 0, MinorRocky, MinorMetallic, MinorCarbon, Rocky, Metallic, Carbon, MajorRocky, MajorMetallic, MajorCarbon, Nitrogen, Silicate, Iron, Water, Ammonia }

    // ── BodyScanState ──────────────────────────────────────────────────────────
    public enum BodyScanState { None = 0, NavBeacon, HonkScanned, FssScanned, Predicted, DssScanned, Analysed }

    // ── StarLuminosityClass ────────────────────────────────────────────────────
    public enum StarLuminosityClass { Unknown = 0, O, Ia, Ib, II, III, IIIa, IIIb, IV, V, Va, Vb, Vz, VI, VII }

    // ── OrganicScanState ───────────────────────────────────────────────────────
    public enum OrganicScanState { None = 0, Unavailable = 1, Discovered = 2, Analysed = 3 }

    // ── OrganicScanStage ───────────────────────────────────────────────────────
    public enum OrganicScanStage { Log = 0, Codex = 1, Prediction = 2, Analyse = 3 }

    // ── ExoBiologyViewState ────────────────────────────────────────────────────
    public enum ExoBiologyViewState { None = 0, CheckList, UnSoldList, Sold, Lost }

    // ── CodexEntryHistory ──────────────────────────────────────────────────────
    public enum CodexEntryHistory { Global = 0, Regional, Commander }

    // ── JournalLogAge ──────────────────────────────────────────────────────────
    public enum JournalLogAge { AllLogs = 0, LastYear, LastSixMonths, LastThreeMonths, LastMonth, LastWeek }

    // ── VariantColours ─────────────────────────────────────────────────────────
    public enum VariantColours { Unknown = 0, Amethyst, Aquamarine, Blue, Cobalt, Cyan, Emerald, Gold, Green, Grey, Indigo, Lime, Magenta, Mauve, Mulberry, Ocher, Orange, Peach, Red, Sage, Teal, Turquoise, White, Yellow }

    // ── VariantChance ──────────────────────────────────────────────────────────
    public enum VariantChance { Unknown = 0, VeryLow, Low, Common, High, VeryHigh }

    // ── Composition ───────────────────────────────────────────────────────────
    public sealed class Composition
    {
        public double Ice { get; set; }
        public double Rock { get; set; }
        public double Metal { get; set; }
    }

    // ── ScanItemComponent ─────────────────────────────────────────────────────
    public sealed class ScanItemComponent
    {
        public string Name { get; set; } = string.Empty;
        public string Name_Localised { get; set; } = string.Empty;
        public double Percent { get; set; }
        // Used as flags bitmask via '|=' on PlanetMaterial
        public PlanetMaterial Name_AsMaterial => PlanetMaterial.None;
    }

    // ── ShipMaterials ─────────────────────────────────────────────────────────
    public sealed class ShipMaterials
    {
        public string Name { get; set; } = string.Empty;
        public string Name_Localised { get; set; } = string.Empty;
        public double Percent { get; set; }
        public PlanetMaterial Name_AsMaterial => PlanetMaterial.None;
        // Implicit conversion so material |= item.Name works in StarSystemViewModel
        public static implicit operator PlanetMaterial(ShipMaterials m) => m.Name_AsMaterial;
    }

    // ── OrganicVariant ────────────────────────────────────────────────────────
    public sealed class OrganicVariant
    {
        public string VariantCodex { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string LocalName { get; set; } = string.Empty;
        public VariantColours Colour { get; set; }
        public PlanetMaterial Material { get; set; }
        public StarType StarType { get; set; }
        public VariantChance Chance { get; set; }
        public bool NewCodexEntry { get; set; }
        public bool Confirmed { get; set; }
        public bool ContainsVariant(OrganicVariant other) => ReferenceEquals(this, other);
    }

    // ── OrganicScanItem ───────────────────────────────────────────────────────
    public sealed class OrganicScanItem
    {
        public string GenusCodex { get; set; } = string.Empty;
        public string SpeciesCodex { get; set; } = string.Empty;
        public string GenusEnglish { get; set; } = string.Empty;
        public string GenusLocalised { get; set; } = string.Empty;
        public string SpeciesEnglish { get; set; } = string.Empty;
        public string SpeciesLocalised { get; set; } = string.Empty;
        public string VariantCodex { get; set; } = string.Empty;
        public List<OrganicVariant> Variants { get; set; } = new();
        public SystemBody Body { get; set; } = new();
        public OrganicScanStage ScanStage { get; set; }
        public DataState DataState { get; set; }
        public System.DateTime ScanTime { get; set; }
        public long TotalValue { get; set; }
        public bool IsNewSpecies { get; set; }
        public bool BodyDssScanned { get; set; }
        public bool WasLogged { get; set; }
        public ODUtils.Exobiology.OrganicInfo? Info { get; set; }
        public List<Position> ScanLocations { get; set; } = new();
    }

    // ── OrganicScanItemList ───────────────────────────────────────────────────
    public sealed class OrganicScanItemList : List<OrganicScanItem>
    {
        public bool IsEmpty => Count == 0;
    }

    // ── Owner ─────────────────────────────────────────────────────────────────
    public sealed class Owner
    {
        public long Address { get; set; }
        public string Name { get; set; } = string.Empty;
        public SystemRegion Region { get; set; } = new();
        public List<SystemBody> SystemBodies { get; set; } = new();
    }

    // ── Parent / Ring / ParentType ─────────────────────────────────────────────
    public sealed class Parent { public ParentType Type { get; set; } = ParentType.Null; public long BodyID { get; set; } }
    public enum ParentType { Null, Planet }
    public sealed class Ring { public string Name { get; set; } = string.Empty; public double OuterRad { get; set; } public double InnerRad { get; set; } }

    // ── SystemBody ────────────────────────────────────────────────────────────
    public sealed class SystemBody
    {
        // Identification
        public string BodyName { get; set; } = string.Empty;
        public long BodyID { get; set; }
        public int EdsmBodyID { get; set; }

        // Ownership / Hierarchy
        public Owner Owner { get; set; } = new();
        public List<Parent> Parents { get; set; } = new();

        // Body type flags
        public bool IsStar { get; set; }
        public bool IsPlanet { get; set; }
        public bool IsNonBody => !IsStar && !IsPlanet;

        // Scan/Discovery
        public BodyScanState ScanState { get; set; }
        public DiscoveryStatus Status { get; set; }
        public DataState BodyDataState { get; set; }
        public bool WasDiscovered { get; set; }
        public bool WasMapped { get; set; }
        public bool WasFootfalled { get; set; }
        public bool DssScanned { get; set; }
        public System.DateTime? ScanDate { get; set; }

        // Values
        public long MappedValue { get; set; }
        public long FssValue { get; set; }
        public long UnsoldCommanderValue { get; set; }
        public long SoldCommanderValue { get; set; }
        public long LostCommanderValue { get; set; }
        public long MinExoValue { get; set; }
        public long MaxExoValue { get; set; }

        // Orbit / Physics
        public double DistanceFromArrivalLs { get; set; }
        public double OrbitalPeriod { get; set; }
        public double RotationPeriod { get; set; }
        public double Radius { get; set; }
        public double AxialTilt { get; set; }
        public bool TidalLock { get; set; }

        // Planet
        public PlanetClass PlanetClass { get; set; }
        public double SurfaceTemp { get; set; }
        public double SurfacePressure { get; set; }
        public double SurfaceGravity { get; set; }
        public double MassEM { get; set; }
        public bool Landable { get; set; }
        public bool Terraformable { get; set; }
        public AtmosphereClass Atmosphere { get; set; }
        public AtmosphereClass AtmosphereType { get; set; }
        public VolcanismType Volcanism { get; set; }
        public int BiologicalSignals { get; set; }
        public int GeologicalSignals { get; set; }
        public List<EliteJournalReader.Events.PlanetRing>? Rings { get; set; }
        public List<ScanItemComponent>? AtmosphereComposition { get; set; }
        public List<ShipMaterials>? Materials { get; set; }
        public Composition Composition { get; set; } = new();
        public OrganicScanItemList? OrganicScanItems { get; set; }

        // Star
        public StarType StarType { get; set; }
        public StarType GoverningStar { get; set; }
        public StarLuminosityClass StarLuminosity { get; set; }
        public double? StellarMass { get; set; }
        public double Age_MY { get; set; }
        public double AbsoluteMagnitude { get; set; }

        // EDSM
        public List<SystemBody> GetGoverningStar() => new();
    }

    // ── StarSystem ────────────────────────────────────────────────────────────
    public sealed class StarSystem
    {
        public string Name { get; set; } = string.Empty;
        public long SystemAddress { get; set; }
        public long Address { get; set; }
        public Position Position { get; set; }
        public SystemRegion Region { get; set; } = new();
        public StarType StarType { get; set; }
        public long EstimatedValue { get; set; }
        public bool IsKnownToEDSM { get; set; }
        public bool VisitedByCommander { get; set; }
        public int DiscoveredBodyCount { get; set; }
        public int BodyCount { get; set; }
        public int EdsmScannedBodyCount { get; set; }
        public string EdsmUrl { get; set; } = string.Empty;
        public int SoldCount { get; set; }
        public int LostCount { get; set; }
        public int UnsoldCount { get; set; }
        public bool AllBodiesFound { get; set; }
        public List<SystemBody> SystemBodies { get; set; } = new();
    }

    namespace EdAstro
    {
        public enum EDAstroType { Unknown = 0 }
    }
}
