using System.Collections.Generic;

namespace EliteJournalReader.Events
{
    public class BackpackChangeEvent : JournalEvent<BackpackChangeEvent.BackpackChangeEventArgs>
    {
        public BackpackChangeEvent() : base("BackpackChange") { }

        public class BackpackChangeEventArgs : JournalEventArgs
        {
            public List<BackPackChangeItem> Added { get; set; }
            public List<BackPackChangeItem> Removed { get; set; }
        }
    }
}
