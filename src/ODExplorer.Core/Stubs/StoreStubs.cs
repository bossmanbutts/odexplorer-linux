// Stub SpanshCsvStore (no network/print dependency). The journal pipeline stores
// (JournalParserStore, ExplorationDataStore, OrganicCheckListDataStore) now live
// in Stubs/JournalParserStoreImpl.cs, Stubs/ExplorationDataStoreImpl.cs and
// Stubs/OrganicCheckListDataStoreImpl.cs.

using System;
using ODExplorer.Models;
using ODUtils.Spansh;

namespace ODExplorer.Stores
{
    // ─── SpanshCsvStore ───────────────────────────────────────────────────────
    public sealed class SpanshCsvStore
    {
        public event EventHandler<ExplorationTarget?>? OnCurrentTargetChanged;
        public event EventHandler<SpanshCsvContainer?>? OnCurrentContainerChanged;
        public event EventHandler<bool>? OnCarrierTimerRunning;
        public event EventHandler<string>? OnCarrierTimeTick;

        public int CurrentIndex { get; set; } = 0;
        public bool CarrierTimerRunning { get; } = false;
        public SpanshCsvContainer? CurrentContainer { get; } = null;
        public ExplorationTarget? CurrentTarget { get; } = null;
        public ExplorationTarget? NextTarget { get; } = null;

        public SpanshCsvContainer? GetCurrentContainer(CsvType csvType) => null;
        public bool ParseCSV(string fileName) => false;
        public bool ForceParseCSV(string fileName, CsvType csvType) => false;
        public void SaveCSVs() { }
        public void StartFleetCarrierTimer() { }
        public void StopFleetCarrierTimer() { }
    }
}
