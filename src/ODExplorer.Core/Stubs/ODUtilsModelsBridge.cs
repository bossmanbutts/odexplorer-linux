// Lightweight stubs to satisfy references to ODUtils.Models used in the core. These are temporary; UI/host should provide real models or map via adapters.

namespace ODUtils.Models
{
    public enum GalacticRegions { Unknown = 0, Core, Bubble, OuterRim }

    public sealed class StarSystem
    {
        public string Name { get; set; } = string.Empty;
        public long SystemAddress { get; set; }
    }

    public sealed class SystemBody
    {
        public string BodyName { get; set; } = string.Empty;
        public int BiologicalSignals { get; set; }
        public double Radius { get; set; }
        public double RotationPeriod { get; set; }
        public bool TidalLock { get; set; }
        public double OrbitalPeriod { get; set; }
        public double SurfaceGravity { get; set; }
        public System.Collections.Generic.List<Parent> Parents { get; set; } = new();
        public System.Collections.Generic.List<Ring> Rings { get; set; } = new();
        public Owner Owner { get; set; } = new();
        public bool Landable { get; set; }
        public bool Terraformable { get; set; }
    }

    public sealed class Parent { public ParentType Type { get; set; } = ParentType.Null; public long BodyID { get; set; } }
    public enum ParentType { Null, Planet }
    public sealed class Ring { public string Name { get; set; } = string.Empty; public double OuterRad { get; set; } public double InnerRad { get; set; } }
    public sealed class Owner { public System.Collections.Generic.List<SystemBody> SystemBodies { get; set; } = new(); }

    public readonly record struct Position(double X, double Y, double Z);

    namespace EdAstro
    {
        public enum EDAstroType { Unknown = 0 }
    }
}
