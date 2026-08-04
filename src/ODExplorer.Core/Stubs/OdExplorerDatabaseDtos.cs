// ODUtils.Database.DTOs types used by the real EFCore layer.
// These mirror the real ODUtils DTOs so the database provider can persist
// commanders, journal entries and settings.

using System;
using System.Collections.Generic;

namespace ODUtils.Database.DTOs
{
    public sealed class JournalCommanderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string JournalDir { get; set; } = string.Empty;
        public string LastFile { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }

    public sealed class JournalEntryDTO
    {
        public string Filename { get; set; } = string.Empty;
        public long Offset { get; set; }
        public int CommanderID { get; set; }
        public int EventTypeId { get; set; }
        public string EventData { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
    }

    public sealed class SettingsDTO
    {
        public string Id { get; set; } = string.Empty;
        public int? IntValue { get; set; }
        public double? DoubleValue { get; set; }
        public string? StringValue { get; set; }
    }
}
