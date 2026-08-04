namespace ODUtils.Spansh
{
    public enum CsvType
    {
        None = -1,
        RoadToRiches = 0,
        FleetCarrier,
        NeutronRoute,
        GalaxyPlotter,
        WorldTypeRoute,
        TouristRoute,
        Exobiology,
        ExobiologyOld,
        Colonisation,
        GalaxyPlotterOld,
    }

    public sealed class SpanshCsvDTO
    {
        public int CsvType { get; set; }
        public int CommanderID { get; set; }
        public string Json { get; set; } = string.Empty;
    }

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
