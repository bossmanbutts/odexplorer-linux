// Minimal DTO stubs to allow compilation. Real DTOs live in database project; hosts should supply real implementations.

namespace ODExplorer.Database.DTOs
{
    public sealed class JournalCommanderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string JournalDir { get; set; } = string.Empty;
        public string LastFile { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }

    public sealed class SettingsDTO { public string Id { get; set; } = string.Empty; }

    public sealed class SpanshCsvDTO { public int CsvType { get; set; } public int CommanderID { get; set; } public string Json { get; set; } = string.Empty; }

    public sealed class EdAstroPoiDTO { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string GalMapName { get; set; } = string.Empty; public long SystemAddress { get; set; } public double X { get; set; } public double Y { get; set; } public double Z { get; set; } public int Type { get; set; } public int Type2 { get; set; } public string Summary { get; set; } = string.Empty; public string MarkDown { get; set; } = string.Empty; public double DistanceFromSol { get; set; } public string PoiUrl { get; set; } = string.Empty; }

    public sealed class JournalEntryDTO { public string Filename { get; set; } = string.Empty; public long Offset { get; set; } public int CommanderID { get; set; } public int EventTypeId { get; set; } public string EventData { get; set; } = string.Empty; public System.DateTime TimeStamp { get; set; } }

    public sealed class CartoIgnoredSystemsDTO { public long Address { get; set; } public string Name { get; set; } = string.Empty; public System.Collections.Generic.List<JournalCommanderDTO> Commanders { get; set; } = new(); }
}
