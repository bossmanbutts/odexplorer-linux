namespace ODUtils.Spansh
{
    public enum CsvType { RoadToRiches = 0, GalaxyPlotter }

    public sealed class ExplorationTarget
    {
        public string SystemName { get; set; } = string.Empty;
        public int Property3 { get; set; }
        public string Property3Text => Property3.ToString();
    }
}
