using System.Collections.Generic;

namespace EliteJournalReader.Events
{
    public class BackpackEvent : JournalEvent<BackpackEvent.BackpackEventArgs>
    {
        public BackpackEvent() : base("Backpack") { }

        public class BackpackEventArgs : JournalEventArgs
        {
            public List<BackPackItem> Items { get; set; }
            public List<BackPackItem> Components { get; set; }
            public List<BackPackItem> Consumables { get; set; }
            public List<BackPackItem> Data { get; set; }
        }
    }
}
