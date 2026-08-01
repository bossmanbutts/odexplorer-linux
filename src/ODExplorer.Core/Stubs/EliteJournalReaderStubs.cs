// Minimal stubs for EliteJournalReader namespaces referenced by the core copy.
// These are temporary and only exist to allow building the core without the original library.

namespace EliteJournalReader
{
    public sealed class JournalEntry
    {
        public string Event { get; set; } = string.Empty;
        public object EventData { get; set; } = new object();
    }
}

namespace EliteJournalReader.Events
{
    public enum ParentType { Null, Planet }

    public sealed class CarrierLocationEvent
    {
        public sealed class CarrierLocationEventArgs
        {
            public string StarSystem { get; set; } = string.Empty;
            public string CarrierType { get; set; } = string.Empty;
            public System.DateTime Timestamp { get; set; }
        }
    }

    public sealed class FSDJumpEvent
    {
        public sealed class FSDJumpEventArgs { public string StarSystem { get; set; } = string.Empty; }
    }

    public sealed class CarrierJumpEvent
    {
        public sealed class CarrierJumpEventArgs { public string StarSystem { get; set; } = string.Empty; }
    }

    public sealed class CarrierStatsEvent
    {
        public sealed class CarrierStatsEventArgs { public string Name { get; set; } = string.Empty; public string Callsign { get; set; } = string.Empty; public string CarrierType { get; set; } = string.Empty; public System.DateTime Timestamp { get; set; } }
    }

    public sealed class CarrierJumpRequestEvent
    {
        public sealed class CarrierJumpRequestEventArgs { public System.DateTime DepartureTime { get; set; } = System.DateTime.UtcNow; public string CarrierType { get; set; } = string.Empty; public System.DateTime Timestamp { get; set; } }
    }

    public sealed class CarrierJumpCancelledEvent
    {
        public sealed class CarrierJumpCancelledEventArgs { public string CarrierType { get; set; } = string.Empty; public System.DateTime Timestamp { get; set; } }
    }
}
