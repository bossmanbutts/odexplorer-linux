// Stubs for commonly used ODUtils models/enums to allow core to compile without the full ODUtils dependency.
// TODO: Replace these stubs with adapter-based mappings or real library references in the UI/host.

namespace ODExplorer.Stubs
{
    public enum GalacticRegions
    {
        Unknown = 0,
        Core,
        Bubble,
        OuterRim
    }

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
}
