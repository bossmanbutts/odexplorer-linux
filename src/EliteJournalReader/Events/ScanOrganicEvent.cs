using Newtonsoft.Json;

namespace EliteJournalReader.Events
{
    public class ScanOrganicEvent : JournalEvent<ScanOrganicEvent.ScanOrganicEventArgs>
    {
        public ScanOrganicEvent() : base("ScanOrganic") { }

        public class ScanOrganicEventArgs : JournalEventArgs
        {
            // Tolerant parse: the journal has historically emitted "Organic" here
            // (and the odd localized string), which StringEnumConverter would throw
            // on and drop the whole must-map event. Unknown values degrade to the
            // default stage instead of losing the scan.
            [JsonConverter(typeof(ExtendedStringEnumConverter<OrganicScanStage>))]
            public OrganicScanStage ScanType { get; set; }
            public string Genus { get; set; }
            public string Genus_Localised { get; set; }
            public string Species { get; set; }
            public string Species_Localised { get; set; }
            public string Variant { get; set; }
            public string Variant_Localised { get; set; }
            public long SystemAddress { get; set; }
            public long Body { get; set; }
        }
    }
}