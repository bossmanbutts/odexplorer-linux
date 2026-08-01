// Stub implementations for ODExplorer store classes that are excluded from the core build.
// These allow ViewModels to compile without the full store implementations.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODExplorer.Models;
using ODExplorer.ViewModels.ModelVMs;
using ODUtils.Database.Interfaces;
using ODUtils.Journal;
using ODUtils.Models;
using ODUtils.Spansh;

namespace ODExplorer.Stores
{
    // ─── JournalParserStore ────────────────────────────────────────────────────
    public sealed class JournalParserStore
    {
        public event EventHandler<string?>? OnJournalStoreStatusChange;
        public event EventHandler<bool>? OnParserStoreLive;
        public event EventHandler? OnCommandersUpdated;

        public bool IsLive { get; } = false;
        public List<JournalCommander> JournalCommanders { get; } = new();

        public Task ReadNewCommander(int id) => Task.CompletedTask;
        public Task UpdateCommanders() => Task.CompletedTask;
        public Task ResetDataBase(IOdToolsDatabaseProvider provider) => Task.CompletedTask;
        public void ReadNewDirectory(string path) { }
    }

    // ─── ExplorationDataStore ──────────────────────────────────────────────────
    public sealed class ExplorationDataStore
    {
        public event EventHandler<StarSystem?>? OnCurrentSystemUpdated;
        public event EventHandler<string>? OnFSDJump;
        public event EventHandler? OnCartoDataSold;
        public event EventHandler? OnCartoDataLost;
        public event EventHandler? OnBioDataSold;
        public event EventHandler<OrganicScanItem>? OnBioDataUpdated;
        public event EventHandler? OnBioDataLost;
        public event EventHandler<SystemBody>? OnBodyUpdated;
        public event EventHandler<List<StarSystem>>? OnRouteUpdated;
        public event EventHandler<StarSystem>? OnSystemUpdatedFromEDSM;
        public event EventHandler<StarSystem?>? OnAllBodiesDiscovered;
        public event EventHandler<SystemBody>? OnBodyBiosUpdated;
        public event EventHandler<SystemBody>? OnBodyTargeted;

        public StarSystem? CurrentSystem { get; } = null;
        public string? CurrentSystemName { get; } = null;
        public string CurrentSystemRegion { get; } = string.Empty;
        public List<ODExplorer.Models.EdAstroPoi> EdAstroPois { get; } = new();
        public ObservableCollection<OrganicScanItemViewModel> OrganicScanItems { get; } = new();
        public List<StarSystem> Route { get; } = new();
        public long SelectedBodyId { get; set; }

        public string GetUnsoldCartoValueString() => string.Empty;
        public string GetUnsoldExoValueString() => string.Empty;
        public void PopulateIgnoredSystems(int commanderId) { }
        public IEnumerable<StarSystem> GetUnsoldCartoSystems() => Array.Empty<StarSystem>();
        public IEnumerable<StarSystem> GetSoldCartoSystems() => Array.Empty<StarSystem>();
        public IEnumerable<StarSystem> GetLostCartoSystems() => Array.Empty<StarSystem>();
    }

    // ─── OrganicCheckListDataStore ─────────────────────────────────────────────
    public sealed class OrganicCheckListDataStore
    {
        private static readonly System.Collections.Generic.List<OrganicCheckListItemViewModel> _empty = new();
        public event EventHandler? OnOrganicScanDetailsUpdated;
        public event EventHandler<string>? OnSpeciesUpdated;

        // Indexer returning list of checklist items by genus key
        public System.Collections.Generic.IReadOnlyList<OrganicCheckListItemViewModel> this[string key] => _empty;

        // Named property with indexer for OrganicScanItems
        public OrganicScanItemsCollection OrganicScanItems { get; } = new();

        public OrganicCheckListDataStore(JournalParserStore parserStore,
                                          ODUtils.Exobiology.ExoData exoData,
                                          SettingsStore settings) { }
    }

    public sealed class OrganicScanItemsCollection
    {
        private static readonly System.Collections.Generic.List<OrganicCheckListItemViewModel> _empty = new();
        public System.Collections.Generic.IReadOnlyList<OrganicCheckListItemViewModel> this[string key] => _empty;
    }

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
        public SpanshCsvContainer? ParseCSV(string fileName) => null;
        public SpanshCsvContainer? ForceParseCSV(string fileName, CsvType csvType) => null;
        public void SaveCSVs() { }
        public void StartFleetCarrierTimer() { }
        public void StopFleetCarrierTimer() { }
    }
}
