// Lightweight stubs to satisfy references to ODUtils.Models used in the core. These are temporary; UI/host should provide real models or map via adapters.

using System.Collections.Generic;
using System.Linq;
using ODUtils.Exobiology;

namespace ODUtils.Models
{
    // ── Regions ────────────────────────────────────────────────────────────────
    // Matches real ODUtils.Models.GalacticRegion (36 values)
    public enum GalacticRegion
    {
        All,
        GalacticCentre,
        EmpyreanStraits,
        RykersHope,
        OdinsHold,
        NormaArm,
        ArcadianStream,
        Izanami,
        InnerOrionPerseusConflux,
        InnerScutumCentaurusArm,
        NormaExpanse,
        TrojanBelt,
        TheVeils,
        NewtonsVault,
        TheConduit,
        OuterOrionPerseusConflux,
        OrionCygnusArm,
        Temple,
        InnerOrionSpur,
        HawkingsGap,
        DrymansPoint,
        SagittariusCarinaArm,
        MareSomnia,
        Acheron,
        FormorianFrontier,
        HieronymusDelta,
        OuterScutumCentaurusArm,
        OuterArm,
        AquilasHalo,
        ErrantMarches,
        PerseusArm,
        FormidineRift,
        VulcanGate,
        ElysianShore,
        SanguineousRim,
        OuterOrionSpur,
        AchillessAltar,
        Xibalba,
        LyrasSong,
        Tenebrae,
        TheAbyss,
        KeplersCrest,
        TheVoid
    }

    // Legacy alias used by some ViewModels
    public enum GalacticRegions { Unknown = 0, Core, Bubble, OuterRim }

    public sealed class SystemRegion
    {
        public string Name { get; set; } = string.Empty;
    }

    // ── Position ───────────────────────────────────────────────────────────────
    // Matches real ODUtils: sealed class with math operators.
    public sealed class Position
    {
        public const double KEpsilon = 9.999999747378752E-06;
        public const double KEpsilonNormalSqrt = 1.0000000036274937E-15;

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Position(double x, double y, double z) { X = x; Y = y; Z = z; }

        public Position FlipZ => new Position(X, Y, Z * -1.0);

        public double DistanceFrom(Position other) => Distance(this, other);

        public double GetMagnitude() => System.Math.Sqrt(X * X + Y * Y + Z * Z);
        public double GetSqrMagnitude() => X * X + Y * Y + Z * Z;

        public static double SqrMagnitude(Position p) => p.X * p.X + p.Y * p.Y + p.Z * p.Z;
        public static double Magnitude(Position p) => System.Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);

        public static double Distance(Position a, Position b)
        {
            var dx = a.X - b.X; var dy = a.Y - b.Y; var dz = a.Z - b.Z;
            return System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static double Angle(Position from, Position to)
        {
            double mag = System.Math.Sqrt(SqrMagnitude(from) * SqrMagnitude(to));
            if (mag < KEpsilonNormalSqrt) return 0.0;
            double d = System.Math.Clamp(Dot(from, to) / mag, -1.0, 1.0);
            return System.Math.Acos(d) * 360.0 / (System.Math.PI * 2.0);
        }

        public static double SignedAngle(Position from, Position to, Position axis)
        {
            double angle = Angle(from, to);
            double crossX = from.Y * to.Z - from.Z * to.Y;
            double crossY = from.Z * to.X - from.X * to.Z;
            double crossZ = from.X * to.Y - from.Y * to.X;
            double sign = System.Math.Sign(axis.X * crossX + axis.Y * crossY + axis.Z * crossZ);
            return angle * sign;
        }

        public static Position Cross(Position lhs, Position rhs) =>
            new Position(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);

        public Position Cross(Position other) => Cross(this, other);

        public static double Dot(Position lhs, Position rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;

        public double Dot(Position other) => Dot(this, other);

        public static Position ProjectOnPlane(Position position, Position planeNormal)
        {
            double magSq = Dot(planeNormal, planeNormal);
            if (magSq < KEpsilon) return position;
            double dot = Dot(position, planeNormal);
            return new Position(position.X - planeNormal.X * dot / magSq, position.Y - planeNormal.Y * dot / magSq, position.Z - planeNormal.Z * dot / magSq);
        }

        public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Position operator -(Position a, Position b) => new Position(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Position operator -(Position a) => new Position(-a.X, -a.Y, -a.Z);
        public static Position operator *(Position a, float d) => new Position(a.X * d, a.Y * d, a.Z * d);
        public static Position operator *(double d, Position a) => new Position(a.X * d, a.Y * d, a.Z * d);
        public static Position operator /(Position a, float d) => new Position(a.X / d, a.Y / d, a.Z / d);

        public override string ToString() => $"({X:N6}, {Y:N6}, {Z:N6})";

        public override bool Equals(object? obj) =>
            obj is Position other && X == other.X && Y == other.Y && Z == other.Z;

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static bool operator ==(Position? left, Position? right) =>
            ReferenceEquals(left, right) || (left?.Equals(right) == true);

        public static bool operator !=(Position? left, Position? right) => !(left == right);
    }

    // ── DataState ──────────────────────────────────────────────────────────────
    public enum DataState { Unsold = 0, Sold, Lost }

    // ── DiscoveryStatus ────────────────────────────────────────────────────────
    // Matches real ODUtils: { Discovered=0, UnDiscovered=1, WorthMapping=2, MappedByUser=3, Noteable=4 }
    public enum DiscoveryStatus { Discovered = 0, UnDiscovered, WorthMapping, MappedByUser, Noteable }

    // ── PlanetClass ────────────────────────────────────────────────────────────
    public enum PlanetClass
    {
        [System.ComponentModel.Description("Unknown")] Unknown = 0,
        [System.ComponentModel.Description("EDSM Valuable Body")] EdsmValuableBody,
        [System.ComponentModel.Description("Icy Body")] IcyBody,
        [System.ComponentModel.Description("Rocky Body")] RockyBody,
        [System.ComponentModel.Description("Rocky Ice Body")] RockyIceBody,
        [System.ComponentModel.Description("Metal Rich Body")] MetalRichBody,
        [System.ComponentModel.Description("High Metal Content Body")] HighMetalContentBody,
        [System.ComponentModel.Description("Earth-like World")] EarthlikeBody,
        [System.ComponentModel.Description("Water World")] WaterWorld,
        [System.ComponentModel.Description("Water Giant")] WaterGiant,
        [System.ComponentModel.Description("Water Giant with Life")] WaterGiantWithLife,
        [System.ComponentModel.Description("Ammonia World")] AmmoniaWorld,
        [System.ComponentModel.Description("Gas Giant with Water-based Life")] GasGiantWithWaterBasedLife,
        [System.ComponentModel.Description("Gas Giant with Ammonia-based Life")] GasGiantWithAmmoniaBasedLife,
        [System.ComponentModel.Description("Sudarsky Class I Gas Giant")] SudarskyClassIGasGiant,
        [System.ComponentModel.Description("Sudarsky Class II Gas Giant")] SudarskyClassIIGasGiant,
        [System.ComponentModel.Description("Sudarsky Class III Gas Giant")] SudarskyClassIIIGasGiant,
        [System.ComponentModel.Description("Sudarsky Class IV Gas Giant")] SudarskyClassIVGasGiant,
        [System.ComponentModel.Description("Sudarsky Class V Gas Giant")] SudarskyClassVGasGiant,
        [System.ComponentModel.Description("Helium Rich Gas Giant")] HeliumRichGasGiant,
        [System.ComponentModel.Description("Helium Gas Giant")] HeliumGasGiant
    }

    // ── StarType ───────────────────────────────────────────────────────────────
    public enum StarType
    {
        [System.ComponentModel.Description("Unknown")] Unknown = 0,
        [System.ComponentModel.Description("O")] O,
        [System.ComponentModel.Description("B")] B,
        [System.ComponentModel.Description("A")] A,
        [System.ComponentModel.Description("F")] F,
        [System.ComponentModel.Description("G")] G,
        [System.ComponentModel.Description("K")] K,
        [System.ComponentModel.Description("M")] M,
        [System.ComponentModel.Description("L")] L,
        [System.ComponentModel.Description("T")] T,
        [System.ComponentModel.Description("Y")] Y,
        [System.ComponentModel.Description("TTS Proto Star")] TTS,
        [System.ComponentModel.Description("AeBe Proto Star")] AeBe,
        [System.ComponentModel.Description("Wolf-Rayet")] W,
        [System.ComponentModel.Description("Wolf-Rayet N")] WN,
        [System.ComponentModel.Description("Wolf-Rayet NC")] WNC,
        [System.ComponentModel.Description("Wolf-Rayet C")] WC,
        [System.ComponentModel.Description("Wolf-Rayet O")] WO,
        [System.ComponentModel.Description("Carbon Sequence")] CS,
        [System.ComponentModel.Description("Carbon Star")] C,
        [System.ComponentModel.Description("Carbon Star N")] CN,
        [System.ComponentModel.Description("Carbon Star J")] CJ,
        [System.ComponentModel.Description("Carbon Star H")] CH,
        [System.ComponentModel.Description("Carbon Star Hd")] CHd,
        [System.ComponentModel.Description("MS-Type Star")] MS,
        [System.ComponentModel.Description("S-Type Star")] S,
        [System.ComponentModel.Description("White Dwarf")] D,
        [System.ComponentModel.Description("DA")]
        DA,
        [System.ComponentModel.Description("DAB")]
        DAB,
        [System.ComponentModel.Description("DAO")]
        DAO,
        [System.ComponentModel.Description("DAZ")]
        DAZ,
        [System.ComponentModel.Description("DAV")]
        DAV,
        [System.ComponentModel.Description("DB")]
        DB,
        [System.ComponentModel.Description("DBZ")]
        DBZ,
        [System.ComponentModel.Description("DBV")]
        DBV,
        [System.ComponentModel.Description("DO")]
        DO,
        [System.ComponentModel.Description("DOV")]
        DOV,
        [System.ComponentModel.Description("DQ")]
        DQ,
        [System.ComponentModel.Description("DC")]
        DC,
        [System.ComponentModel.Description("DCV")]
        DCV,
        [System.ComponentModel.Description("DX")]
        DX,
        [System.ComponentModel.Description("Neutron Star")] N,
        [System.ComponentModel.Description("Black Hole")] BH,
        [System.ComponentModel.Description("Rogue Planet")] H,
        [System.ComponentModel.Description("X")]
        X
    }

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

    // ── ScanState ──────────────────────────────────────────────────────────────
    // Matches real ODUtils: { None=0, Fss=1, Dss=2 }
    public enum ScanState
    {
        [System.ComponentModel.Description("")] None = 0,
        [System.ComponentModel.Description("FSS")] Fss = 1,
        [System.ComponentModel.Description("DSS")] Dss = 2
    }

    // ── StarLuminosityClass ────────────────────────────────────────────────────
    public enum StarLuminosityClass { Unknown = 0, O, Ia, Ib, II, III, IIIa, IIIb, IV, V, Va, Vb, Vz, VI, VII }

    // ── OrganicScanState ───────────────────────────────────────────────────────
    // Matches real ODUtils: Unavailable=-1, None=0, Discovered=2, Analysed=3, Sold=4
    public enum OrganicScanState { Unavailable = -1, None = 0, Discovered = 2, Analysed = 3, Sold = 4 }

    // ── OrganicScanStage ───────────────────────────────────────────────────────
    // Mirrors the journal values (in scan-progression order) so the progression
    // comparisons used by the distance-to-sample feature keep their meaning:
    // samples logged/sampled/analysed are all "past Codex".
    public enum OrganicScanStage
    {
        MultiChoice = -1,
        Prediction = 0,
        DSS = 1,
        Codex = 2,
        Log = 3,
        Sample = 4,
        Analyse = 5
    }

    // ── ExoBiologyViewState ────────────────────────────────────────────────────
    public enum ExoBiologyViewState { None = -1, CheckList = 0, UnSoldList, Sold, Lost }

    // ── CodexEntryHistory ──────────────────────────────────────────────────────
    public enum CodexEntryHistory { Regional = 0, Global }

    // ── JournalLogAge ──────────────────────────────────────────────────────────
    public enum JournalLogAge
    {
        [System.ComponentModel.Description("Load All")] All,
        [System.ComponentModel.Description("< 7 Days")] SevenDays,
        [System.ComponentModel.Description("< 30 Days")] ThirtyDays,
        [System.ComponentModel.Description("< 60 Days")] SixtyDays,
        [System.ComponentModel.Description("< 180 Days")] OneHundredEightyDays,
        [System.ComponentModel.Description("< One Year")] Oneyear,
        [System.ComponentModel.Description("< Two Years")] Twoyears,
        [System.ComponentModel.Description("< Three Years")] Threeyears,
        [System.ComponentModel.Description("< Four Years")] Fouryears,
        [System.ComponentModel.Description("< Five Years")] Fiveyears,
        [System.ComponentModel.Description("< Six Years")] Sixyears,
        [System.ComponentModel.Description("< Seven Years")] Sevenyears,
        [System.ComponentModel.Description("< Eight Years")] Eightyears
    }

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
        public PlanetMaterial Name_AsMaterial => Name?.ToLowerInvariant() switch
        {
            "carbon" => PlanetMaterial.carbon,
            "vanadium" => PlanetMaterial.vanadium,
            "germanium" => PlanetMaterial.germanium,
            "cadmium" => PlanetMaterial.cadmium,
            "niobium" => PlanetMaterial.niobium,
            "yttrium" => PlanetMaterial.yttrium,
            "polonium" => PlanetMaterial.polonium,
            _ => PlanetMaterial.None,
        };
    }

    // ── ShipMaterials ─────────────────────────────────────────────────────────
    public sealed class ShipMaterials
    {
        public string Name { get; set; } = string.Empty;
        public string Name_Localised { get; set; } = string.Empty;
        public double Percent { get; set; }

        // Maps the journal material name ("carbon", "vanadium", ...) to its
        // jumponium-relevant PlanetMaterial flag; non-jumponium materials map to
        // None so the CheckSystemMaterials bitmask only accumulates the seven
        // synthesis materials.
        public PlanetMaterial Name_AsMaterial => Name?.ToLowerInvariant() switch
        {
            "carbon" => PlanetMaterial.carbon,
            "vanadium" => PlanetMaterial.vanadium,
            "germanium" => PlanetMaterial.germanium,
            "cadmium" => PlanetMaterial.cadmium,
            "niobium" => PlanetMaterial.niobium,
            "yttrium" => PlanetMaterial.yttrium,
            "polonium" => PlanetMaterial.polonium,
            _ => PlanetMaterial.None,
        };
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
        public bool IsNewSpecies { get; set; }
        public bool BodyDssScanned { get; set; }
        public bool WasLogged { get; set; }
        public ODUtils.Exobiology.OrganicInfo? Info { get; set; }
        public List<OrganicScanDetails> ScanLocations { get; set; } = new();
        public ScanNotificationState NotificationState { get; set; } = ScanNotificationState.TooClose;

        // Computed properties matching real ODUtils
        public long TotalValue => (Info == null) ? 0 : Info.Value + Bonus;
        public long Value => Info?.Value ?? StoredValue;
        public long StoredValue { get; set; }
        public long Bonus
        {
            get
            {
                if (Info == null) return 0;
                if (ScanTime <= ODExplorer.Models.PatchDates.Type11PatchDate)
                    return Body.WasMapped ? 0 : (ScanTime < ODUtils.Exobiology.OrganicValues.NewPriceDate ? Info.Value : Info.Value * 4);
                return WasLogged ? 0 : (ScanTime < ODUtils.Exobiology.OrganicValues.NewPriceDate ? Info.Value : Info.Value * 4);
            }
        }
    }

    public enum ScanNotificationState
    {
        TooClose = 0,
        FarEnough = 1
    }

    public sealed class OrganicScanDetails
    {
        public OrganicScanStage ScanStage { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public ScanNotificationState DistanceState { get; set; }
        public bool HasPos => System.Math.Abs(Longitude) > 0.01 && System.Math.Abs(Latitude) > 0.01;
    }

    // ── OrganicScanItemList ───────────────────────────────────────────────────
    public sealed class OrganicScanItemList : List<OrganicScanItem>
    {
        public bool IsEmpty => Count == 0;
    }

    // ── Owner ─────────────────────────────────────────────────────────────────
    // Legacy alias: real ODUtils uses StarSystem as SystemBody.Owner
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
        public ScanState ScanState { get; set; }
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
        public double Eccentricity { get; set; }
        public double SemiMajorAxis { get; set; }
        public bool TidalLock { get; set; }

        // Planet
        public PlanetClass PlanetClass { get; set; }
        public double SurfaceTemp { get; set; }
        public double SurfacePressure { get; set; }
        public double SurfaceGravity { get; set; }
        public double MassEM { get; set; }
        public bool Landable { get; set; }
        public bool Terraformable { get; set; }
        public string TerraformState { get; set; } = string.Empty;
        public AtmosphereClass Atmosphere { get; set; }
        public AtmosphereClass AtmosphereType { get; set; }
        // Full journal atmosphere description (e.g. "thin carbon dioxide-rich");
        // preserved so the exo prediction engine can distinguish thin/thick/hot.
        public EliteJournalReader.AtmosphereDescription AtmosphereDescription { get; set; }
        public VolcanismType Volcanism { get; set; }
        // Raw journal volcanism string (e.g. "Water Geysers Volcanism"); used by the
        // exo prediction engine which mirrors BioScan's substring matching.
        public string VolcanismName { get; set; } = string.Empty;
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
        public long ParentStarBodyId { get; set; } = -1;
        public List<long> ParentStarBodyIds { get; set; } = [];
        public StarLuminosityClass StarLuminosity { get; set; }
        public double? StellarMass { get; set; }
        public double Age_MY { get; set; }
        public double AbsoluteMagnitude { get; set; }

        // EDSM
        public List<SystemBody> GetGoverningStar() => new();
    }

    // ── StarSystem ────────────────────────────────────────────────────────────
    public sealed class StarSystem : System.IComparable, System.IEquatable<object>
    {
        public string Name { get; set; } = string.Empty;
        public long SystemAddress { get; set; }
        public long Address { get; set; }
        public Position Position { get; set; } = new(0, 0, 0);
        public SystemRegion Region { get; set; } = new();
        public StarType StarType { get; set; }
        public long EstimatedValue { get; set; }
        public bool IsKnownToEDSM { get; set; }
        public bool VisitedByCommander { get; set; }
        public int DiscoveredBodyCount { get; set; }
        public int BodyCount { get; set; }
        public int EdsmScannedBodyCount { get; set; } = -1;
        public string EdsmUrl { get; set; } = string.Empty;
        public bool AllBodiesFound { get; set; }
        public List<SystemBody> SystemBodies { get; set; } = new();

        // Computed properties matching real ODUtils
        public long CommanderValue => SystemBodies.Sum(x => x.UnsoldCommanderValue);
        public int SoldCount { get; set; }
        public int LostCount { get; set; }
        public int UnsoldCount { get; set; }

        // IComparable
        public int CompareTo(object? obj)
        {
            if (obj != null && obj is StarSystem other && Name != null)
                return string.Compare(Name, other.Name, System.StringComparison.OrdinalIgnoreCase);
            return 1;
        }

        // IEquatable / Equality
        bool System.IEquatable<object>.Equals(object? obj) => CompareTo(obj) == 0;
        public override bool Equals(object? obj) => CompareTo(obj) == 0;
        public override int GetHashCode() => HashCode.Combine(Name?.GetHashCode(), Position?.GetHashCode());
        public static bool operator ==(StarSystem? a, StarSystem? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Name == b.Name;
        }
        public static bool operator !=(StarSystem? a, StarSystem? b) => !(a == b);
    }

    namespace EdAstro
    {
        public enum EDAstroType
        {
            Community = 0,
            DeepSpaceOutpost,
            Empty,
            Glitches,
            GreenGasGiants,
            Historical,
            InhabitedSystem,
            Memorials,
            MysteryAndXenology,
            Nebulae,
            NotableStellarPhenomena,
            Organic,
            PlanetaryCircumnavigation,
            PlanetaryFeatures,
            SightsAndScenery,
            StellarFeatures,
            TouristBeacons
        }
    }
}
