namespace ODUtils.Spansh
{
    public enum CsvType { RoadToRiches = 0, GalaxyPlotter }

    public sealed class ExplorationTarget
    {
        public string SystemName { get; set; } = string.Empty;
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
        public string? Property3 { get; set; }
        public string? Property4 { get; set; }
        public System.Collections.Generic.List<BodiesInfo>? BodiesInfo { get; set; }
    }

    public sealed class BodiesInfo
    {
        public string? Body { get; set; }
        public string? Distance { get; set; }
        public string? Property1 { get; set; }
    }
}
