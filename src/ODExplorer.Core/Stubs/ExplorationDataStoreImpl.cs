// Functional in-memory ExplorationDataStore for the journal→carto/organic pipeline.
// Parses typed journal events into the ODUtils.Models models and raises the same
// events as the real store so the ViewModels work unchanged. EDSM lookups and
// exo predictions are approximated; the real store can replace this later.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODExplorer.Database;
using ODExplorer.Journal;
using ODExplorer.Models;
using ODUtils.APis;
using ODUtils.Database.Interfaces;
using ODUtils.EliteDangerousHelpers.GalacticRegions;
using ODUtils.EliteDangerousHelpers;
using ODUtils.Exobiology;
using ODUtils.Extensions;
using ODUtils.Journal;
using ODUtils.Models;
using Composition = ODUtils.Models.Composition;
using JournalEntry = ODUtils.Journal.JournalEntry;
using JournalTypeEnum = ODUtils.Journal.JournalTypeEnum;
using OrganicScanStage = ODUtils.Models.OrganicScanStage;
using PlanetClass = ODUtils.Models.PlanetClass;
using ScanItemComponent = ODUtils.Models.ScanItemComponent;
using ShipMaterials = ODUtils.Models.ShipMaterials;
using StarType = ODUtils.Models.StarType;
using SystemBody = ODUtils.Models.SystemBody;
using StatusFileEvent = ODUtils.Journal.StatusFileEvent;

namespace ODExplorer.Stores
{
    public sealed class ExplorationDataStore : IProcessJournalLogs
    {
        #region Ctor
        public ExplorationDataStore(JournalParserStore parserStore,
                                    EdsmApiService edsmApi,
                                    IOdToolsDatabaseProvider databaseProvider,
                                    NotificationStore notificationStore,
                                    SettingsStore settingsStore,
                                    ExoData exoData,
                                    OrganicCheckListDataStore organicCheckListData)
        {
            this.parserStore = parserStore;
            this.edsmApi = edsmApi;
            this.databaseProvider = databaseProvider;
            this.notificationStore = notificationStore;
            this.settingsStore = settingsStore;
            this.exoData = exoData;
            this.organicCheckListData = organicCheckListData;

            parserStore.RegisterParser(this);
            parserStore.OnParserStoreLive += ParserStore_OnParserStoreLive;
            parserStore.StatusUpdated += ParserStore_StatusUpdated;
        }
        #endregion

        public async Task RefreshEdAstroPois()
        {
            try
            {
                var pois = await new EdAstroApiService().GetPois();
                if (pois.Count == 0)
                    return;

                if (databaseProvider is OdExplorerDatabaseProvider explorerDatabaseProvider)
                    explorerDatabaseProvider.AddEdAstroPois(pois);

                EdAstroPois = pois;
                DispatcherHelper.Invoke(() => OnEdAstroPoisUpdated?.Invoke(this, EventArgs.Empty));
            }
            catch
            {
                // Network or parse failure; keep any data already loaded from the database.
            }
        }

        #region Private Fields
        private readonly JournalParserStore parserStore;
        private readonly EdsmApiService edsmApi;
        private readonly IOdToolsDatabaseProvider databaseProvider;
        private readonly NotificationStore notificationStore;
        private readonly SettingsStore settingsStore;
        private readonly ExoData exoData;
        private readonly OrganicCheckListDataStore organicCheckListData;

        private const string CartoDataSettingsKey = "CartoDataState";

        private static readonly JsonSerializerOptions CartoJsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        private readonly Dictionary<long, StarSystem> _cartoData = [];
        private readonly List<SystemBody> _organicData = [];
        private readonly Dictionary<long, string> _ignoredSystems = [];

        private readonly HashSet<SystemBody> _highValueExoNotified = [];
        private readonly HashSet<string> _newSpeciesNotified = [];
        private readonly HashSet<string> _newCodexNotified = [];

        private bool onFoot;
        private double longitude;
        private double latitude;
        private long currentBodyDestinationId;
        private int currentCmdrId;
        private int eventsSinceLastSave;
        #endregion

        #region Public Properties
        public StarSystem? CurrentSystem { get; private set; }
        public string? CurrentSystemName => CurrentSystem?.Name;
        public string? CurrentSystemRegion => CurrentSystem?.Region.Name;
        public SystemBody? CurrentBody { get; private set; }
        public long SelectedBodyId { get; set; }
        public List<StarSystem> Route { get; private set; } = [];
        public OrganicScanItem? CurrentBioItem { get; private set; }
        public List<SystemBody> OrganicScanItems => [.. _organicData];
        public List<EdAstroPoi>? EdAstroPois { get; private set; } = [];
        #endregion

        #region Event Declarations
        public event EventHandler<StarSystem?>? OnCurrentSystemUpdated;
        public event EventHandler<StarSystem?>? OnAllBodiesDiscovered;
        public event EventHandler<SystemBody?>? OnCurrentSystemBodyUpdated;
        public event EventHandler<StarSystem>? OnSystemUpdatedFromEDSM;
        public event EventHandler<List<StarSystem>>? OnRouteUpdated;
        public event EventHandler<SystemBody>? OnBodyUpdated;
        public event EventHandler? OnCartoDataSold;
        public event EventHandler? OnCartoDataLost;
        public event EventHandler<OrganicScanItem>? OnBioDataUpdated;
        public event EventHandler<SystemBody>? OnBodyBiosUpdated;
        public event EventHandler<SystemBody>? OnBodyTargeted;
        public event EventHandler<string>? OnFSDJump;
        public event EventHandler? OnBioDataSold;
        public event EventHandler? OnBioDataLost;
        public event EventHandler? OnExoMinValueChanged;
        public event EventHandler? OnEdAstroPoisUpdated;
        #endregion

        #region IProcessJournalLogs Implementation

        private readonly List<JournalTypeEnum> cartoHistoricEventsToParse =
        [
            JournalTypeEnum.Location,
            JournalTypeEnum.FSDJump,
            JournalTypeEnum.CarrierJump,
            JournalTypeEnum.FSSDiscoveryScan,
            JournalTypeEnum.FSSAllBodiesFound,
            JournalTypeEnum.FSSBodySignals,
            JournalTypeEnum.ScanBaryCentre,
            JournalTypeEnum.SupercruiseEntry,
            JournalTypeEnum.Scan,
            JournalTypeEnum.SAAScanComplete,
            JournalTypeEnum.SAASignalsFound,
            JournalTypeEnum.SellExplorationData,
            JournalTypeEnum.MultiSellExplorationData,
            JournalTypeEnum.Disembark,
            JournalTypeEnum.Embark,
            JournalTypeEnum.Died,
            JournalTypeEnum.ApproachBody,
            JournalTypeEnum.CodexEntry,
            JournalTypeEnum.ScanOrganic,
            JournalTypeEnum.SellOrganicData,
        ];

        public JournalHistoryArgs GetEventsToParse(DateTime defaultAge)
        {
            return new(cartoHistoricEventsToParse, defaultAge, this, ParseHistoryStream);
        }

        public void RunBeforeParsingLogs(int currentCmdrId)
        {
            this.currentCmdrId = currentCmdrId;
            PopulateIgnoredSystems(currentCmdrId);
            LoadCartoData();
        }

        public Task ParseHistoryStream(JournalEntry entry)
        {
            ParseJournalEvent(entry);
            return Task.CompletedTask;
        }

        public void ParseHistory(IEnumerable<JournalEntry> journalEntries, int currentCmdrId)
        {
            this.currentCmdrId = currentCmdrId;
            PopulateIgnoredSystems(currentCmdrId);
            LoadCartoData();

            foreach (var journalEntry in journalEntries)
            {
                ParseJournalEvent(journalEntry);
            }
        }

        public Task ParseHistoryStream(IEnumerable<JournalEntry> journalEntries, int currentCmdrId)
        {
            this.currentCmdrId = currentCmdrId;
            PopulateIgnoredSystems(currentCmdrId);
            LoadCartoData();

            foreach (var journalEntry in journalEntries)
            {
                ParseJournalEvent(journalEntry);
            }
            return Task.CompletedTask;
        }

        public void PopulateIgnoredSystems(int currentCmdrId)
        {
            if (databaseProvider is OdExplorerDatabaseProvider provider)
            {
                _ignoredSystems.Clear();

                foreach (var system in provider.GetIgnoredSystems(currentCmdrId))
                {
                    _ignoredSystems.TryAdd(system.Address, system.Name);
                }
            }
        }

        public void SaveCartoData()
        {
            try
            {
                if (databaseProvider is not OdExplorerDatabaseProvider provider)
                    return;

                var json = JsonSerializer.Serialize(_cartoData, CartoJsonOptions);

                provider.AddSettings(
                [
                    new ODUtils.Database.DTOs.SettingsDTO
                    {
                        Id = CartoDataSettingsKey,
                        StringValue = json,
                    }
                ]);
            }
            catch
            {
                // Serialization or DB failure — non-fatal, data is still in memory.
            }
        }

        public void LoadCartoData()
        {
            try
            {
                if (databaseProvider is not OdExplorerDatabaseProvider provider)
                    return;

                var saved = provider.GetAllSettings().FirstOrDefault(x => x.Id == CartoDataSettingsKey);

                if (saved?.StringValue is not { Length: > 0 } json)
                    return;

                var loaded = JsonSerializer.Deserialize<Dictionary<long, StarSystem>>(json, CartoJsonOptions);

                if (loaded is null || loaded.Count == 0)
                    return;

                _cartoData.Clear();
                _organicData.Clear();

                foreach (var (addr, system) in loaded)
                {
                    _cartoData[addr] = system;

                    // Reconstruct Owner.SystemBodies back-reference (null after
                    // ReferenceHandler.IgnoreCycles deserialization).
                    foreach (var body in system.SystemBodies)
                    {
                        if (body.Owner is not null)
                            body.Owner.SystemBodies = system.SystemBodies;

                        if (body.BiologicalSignals > 0)
                            _organicData.Add(body);
                    }
                }
            }
            catch
            {
                // Malformed data — keep empty state, user will repopulate from journals.
            }
        }

        public void ClearData()
        {
            SaveCartoData();
            CurrentSystem = null;
            CurrentBody = null;
            CurrentBioItem = null;
            Route.Clear();
            _cartoData.Clear();
            _ignoredSystems.Clear();
            _organicData.Clear();
        }

        public void Dispose()
        {
            SaveCartoData();
            parserStore.UnregisterParser(this);
            parserStore.OnParserStoreLive -= ParserStore_OnParserStoreLive;
            parserStore.StatusUpdated -= ParserStore_StatusUpdated;
        }

        #region Event Parsing
        public void ParseJournalEvent(JournalEntry e)
        {
            try
            {
                switch (e.EventData)
                {
                    case LocationEvent.LocationEventArgs locationEvt:
                        {
                            onFoot = locationEvt.OnFoot;
                            longitude = locationEvt.Longitude ?? 0;
                            latitude = locationEvt.Latitude ?? 0;

                            var currentSys = UpdateCurrentSystem(BuildSystem(locationEvt.StarSystem, locationEvt.SystemAddress, locationEvt.StarPos.ToArray(), StarType.Unknown));

                            if (locationEvt.BodyType == BodyType.Planet)
                            {
                                var body = currentSys.SystemBodies.FirstOrDefault(x => x.BodyID == locationEvt.BodyID);

                                body ??= CreateMinimalBody(locationEvt.BodyID, locationEvt.Body, currentSys);

                                UpdateCurrentBody(body);
                            }
                        }
                        break;
                    case CarrierJumpEvent.CarrierJumpEventArgs carrierJump:
                        {
                            var currentSys = UpdateCurrentSystem(BuildSystem(carrierJump.StarSystem, carrierJump.SystemAddress, carrierJump.StarPos.ToArray(), StarType.Unknown));

                            if (carrierJump.BodyType == BodyType.Planet)
                            {
                                var body = currentSys.SystemBodies.FirstOrDefault(x => x.BodyID == carrierJump.BodyID);

                                body ??= CreateMinimalBody(carrierJump.BodyID, carrierJump.Body, currentSys);

                                UpdateCurrentBody(body);
                            }
                        }
                        break;
                    case StartJumpEvent.StartJumpEventArgs startJumpEvt:
                        if (parserStore.IsLive && startJumpEvt.JumpType == JumpType.Hyperspace)
                        {
                            InvokeLive(() => OnFSDJump?.Invoke(this, startJumpEvt.StarSystem));
                        }
                        break;
                    case FSDJumpEvent.FSDJumpEventArgs fsdJumpEvent:
                        UpdateCurrentSystem(BuildSystem(fsdJumpEvent.StarSystem, fsdJumpEvent.SystemAddress, fsdJumpEvent.StarPos.ToArray(),
                            JournalEventMapper.GetStarType(fsdJumpEvent.OriginalEvent?["StarClass"]?.ToString())));
                        break;
                    case FSDTargetEvent.FSDTargetEventArgs fsdTargetEvent:
                        {
                            _cartoData.TryGetValue(fsdTargetEvent.SystemAddress, out var data);

                            data ??= BuildSystem(fsdTargetEvent.Name, fsdTargetEvent.SystemAddress, Array.Empty<double>(),
                                JournalEventMapper.GetStarType(fsdTargetEvent.StarClass));

                            if (Route.Contains(data))
                            {
                                break;
                            }
                            Route.Clear();
                            Route.Add(data);
                            _cartoData.TryAdd(data.Address, data);

                            var routeSnapshot = new List<StarSystem>(Route);
                            InvokeLive(() => OnRouteUpdated?.Invoke(this, routeSnapshot));

                            _ = Task.Run(async () =>
                            {
                                if (await GetSystemValue(data).ConfigureAwait(true))
                                {
                                    if (parserStore.IsLive)
                                    {
                                        var updatedRouteSnapshot = new List<StarSystem>(Route);
                                        InvokeLive(() => OnRouteUpdated?.Invoke(this, updatedRouteSnapshot));
                                        InvokeLive(() => OnSystemUpdatedFromEDSM?.Invoke(this, data));
                                    }
                                }
                            });
                        }
                        break;
                    case NavRouteEvent.NavRouteEventArgs:
                        {
                            var navRoute = parserStore.GetNavRoute();
                            if (navRoute?.Route is { Count: > 0 })
                            {
                                Route.Clear();
                                foreach (var sys in navRoute.Route)
                                {
                                    if (sys is null) continue;
                                    var system = CheckIfSystemKnown(BuildSystem(sys.StarSystem, sys.SystemAddress,
                                        sys.StarPos, JournalEventMapper.GetStarType(sys.StarClass)));
                                    if (!Route.Contains(system))
                                    {
                                        _cartoData.TryAdd(system.Address, system);
                                        Route.Add(system);
                                    }
                                }
                                var navRouteSnapshot = new List<StarSystem>(Route);
                                InvokeLive(() => OnRouteUpdated?.Invoke(this, navRouteSnapshot));
                            }
                        }
                        break;
                    case NavRouteClearEvent.NavRoutClearEventArgs:
                        Route.Clear();
                        var clearedSnapshot = new List<StarSystem>(Route);
                        InvokeLive(() => OnRouteUpdated?.Invoke(this, clearedSnapshot));
                        break;
                    case SupercruiseEntryEvent.SupercruiseEntryEventArgs:
                        UpdateCurrentBody(null);
                        break;
                    case FSSDiscoveryScanEvent.FSSDiscoveryScanEventArgs fssScan:
                        if (CurrentSystem?.Address == fssScan.SystemAddress)
                        {
                            CurrentSystem.DiscoveredBodyCount = fssScan.BodyCount;
                            CurrentSystem.AllBodiesFound = fssScan.Progress >= 1;
                            TriggerCurrentSystemEventIfLive();
                        }
                        break;
                    case FSSAllBodiesFoundEvent.FSSAllBodiesFoundEventArgs fssAll:
                        if (CurrentSystem?.Address == fssAll.SystemAddress)
                        {
                            CurrentSystem.DiscoveredBodyCount = fssAll.Count;
                            CurrentSystem.AllBodiesFound = true;
                            if (parserStore.IsLive)
                                InvokeLive(() => OnAllBodiesDiscovered?.Invoke(this, CurrentSystem));
                        }
                        break;
                    case FSSBodySignalsEvent.FSSBodySignalsEventArgs fssBodySignals:
                        {
                            if (_cartoData.TryGetValue(fssBodySignals.SystemAddress, out var system))
                            {
                                var body = GetOrCreateBody(system, fssBodySignals.BodyID, string.Empty);

                                UpdateSignalsFound(body, fssBodySignals.Signals.Select(s => (s.Type, s.Count)), false, fssBodySignals.Timestamp);

                                if (parserStore.IsLive && body.BiologicalSignals > 0 && _organicData.Contains(body) == false)
                                {
                                    _organicData.Add(body);
                                    TriggerBodyBiosUpdatedIfLive(body);
                                }
                                if (parserStore.IsLive && body.IsPlanet)
                                    notificationStore.CheckForNotableNotifications(body);
                                TriggerBodyEventIfLive(body);
                                break;
                            }
                        }
                        break;
                    case ScanEvent.ScanEventArgs scanEvt:
                        {
                            if (scanEvt.ScanType == ScanType.NavBeacon || scanEvt.ScanType == ScanType.NavBeaconDetail)
                                break;

                            if (_cartoData.TryGetValue(scanEvt.SystemAddress, out var system))
                            {
                                var body = AddOrUpdateBodyFromScan(system, scanEvt);                                if (body.BiologicalSignals > 0 && _organicData.Contains(body) == false)
                                {
                                    _organicData.Add(body);
                                }
                                TriggerBodyEventIfLive(body);
                                break;
                            }
                        }
                        break;
                    case SAAScanCompleteEvent.SAAScanCompleteEventArgs ssaScanComplete:
                        {
                            if (_cartoData.TryGetValue(ssaScanComplete.SystemAddress, out var system))
                            {
                                var body = GetOrCreateBody(system, ssaScanComplete.BodyID, string.Empty);

                                ApplyDssScan(body, ssaScanComplete.Timestamp);
                                RecalcSystemCounts(system);
                                TriggerBodyEventIfLive(body);
                                break;
                            }
                        }
                        break;
                    case SAASignalsFoundEvent.SAASignalsFoundEventArgs ssaSignalsFound:
                        {
                            if (_cartoData.TryGetValue(ssaSignalsFound.SystemAddress, out var system))
                            {
                                var body = GetOrCreateBody(system, ssaSignalsFound.BodyID, string.Empty);

                                UpdateSignalsFound(body, ssaSignalsFound.Signals.Select(s => (s.Type, s.Count)), true, ssaSignalsFound.Timestamp);

                                if (body.BiologicalSignals > 0)
                                {
                                    if (_organicData.Contains(body) == false)
                                        _organicData.Add(body);
                                    TriggerBodyBiosUpdatedIfLive(body);
                                }
                                TriggerBodyEventIfLive(body);
                                break;
                            }
                        }
                        break;
                    case ScanBaryCentreEvent.ScanBaryCentreEventArgs scanBarycentre:
                        break;
                    case SellExplorationDataEvent.SellExplorationDataEventArgs sellCarto:
                        {
                            foreach (string system in sellCarto.Systems)
                            {
                                var known = _cartoData.Values.FirstOrDefault(x => x.Name.Equals(system, StringComparison.OrdinalIgnoreCase));

                                if (known is null)
                                {
                                    continue;
                                }
                                MarkBodiesSold(known);
                            }

                            if (parserStore.IsLive)
                                InvokeLive(() => OnCartoDataSold?.Invoke(this, EventArgs.Empty));
                        }
                        break;
                    case MultiSellExplorationDataEvent.MultiSellExplorationDataEventArgs multiSellCarto:
                        {
                            foreach (var system in multiSellCarto.Discovered)
                            {
                                var known = _cartoData.Values.FirstOrDefault(x => x.Name.Equals(system.SystemName, StringComparison.OrdinalIgnoreCase));

                                if (known is null)
                                {
                                    continue;
                                }
                                MarkBodiesSold(known);
                            }

                            if (parserStore.IsLive)
                                InvokeLive(() => OnCartoDataSold?.Invoke(this, EventArgs.Empty));
                        }
                        break;
                    case DisembarkEvent.DisembarkEventArgs disembark:
                        {
                            onFoot = disembark.OnPlanet;
                            longitude = (double?)disembark.OriginalEvent?["Longitude"] ?? 0;
                            latitude = (double?)disembark.OriginalEvent?["Latitude"] ?? 0;

                            if (CurrentBody is not null && CurrentBody.BodyID == disembark.BodyID)
                            {
                                break;
                            }
                            if (CurrentSystem?.Address == disembark.SystemAddress)
                            {
                                var body = CurrentSystem?.SystemBodies.FirstOrDefault(x => x.BodyID == disembark.BodyID);

                                if (body is not null)
                                    UpdateCurrentBody(body);
                            }
                        }
                        break;
                    case EmbarkEvent.EmbarkEventArgs:
                        onFoot = false;
                        break;
                    case DiedEvent.DiedEventArgs:
                        {
                            if (onFoot == false)
                            {
                                foreach (var system in _cartoData.Values.Where(x => x.UnsoldCount > 0))
                                {
                                    MarkBodiesLost(system);
                                }
                                if (parserStore.IsLive)
                                    InvokeLive(() => OnCartoDataLost?.Invoke(this, EventArgs.Empty));
                            }

                            var dataLost = false;
                            foreach (var body in _organicData)
                            {
                                if (body.OrganicScanItems is null)
                                    continue;

                                foreach (var bio in body.OrganicScanItems)
                                {
                                    if (bio.DataState == DataState.Unsold && bio.ScanStage == OrganicScanStage.Analyse)
                                    {
                                        bio.DataState = DataState.Lost;
                                        dataLost = true;
                                    }
                                }
                            }
                            if (dataLost)
                                TriggerBioLostIfLive();
                        }
                        break;
                    case ApproachBodyEvent.ApproachBodyEventArgs approachBody:
                        {
                            CurrentSystem ??= BuildSystem(approachBody.StarSystem, approachBody.SystemAddress, Array.Empty<double>(), StarType.Unknown);

                            var knownBody = CurrentSystem.SystemBodies.FirstOrDefault(x => x.BodyID == approachBody.BodyID);

                            knownBody ??= CreateMinimalBody(approachBody.BodyID, approachBody.Body, CurrentSystem);

                            UpdateCurrentBody(knownBody);
                        }
                        break;
                    case CodexEntryEvent.CodexEntryEventArgs codexEntry:
                        ProcessCodex(codexEntry);
                        break;
                    case ScanOrganicEvent.ScanOrganicEventArgs scanOrganic:
                        ProcessScanOrganic(scanOrganic);
                        break;
                    case SellOrganicDataEvent.SellOrganicDataEventArgs sellOrganic:
                        {
                            foreach (var organic in sellOrganic.BioData)
                            {
                                foreach (var body in _organicData)
                                {
                                    if (body.OrganicScanItems is null)
                                        continue;

                                    var bio = body.OrganicScanItems.FirstOrDefault(x =>
                                        x.Variants.Exists(v => string.Equals(v.VariantCodex, organic.Variant, StringComparison.OrdinalIgnoreCase))
                                        && x.ScanStage == OrganicScanStage.Analyse);

                                    if (bio is not null)
                                    {
                                        bio.DataState = DataState.Sold;
                                        continue;
                                    }

                                    bio = body.OrganicScanItems.FirstOrDefault(x =>
                                        string.Equals(x.SpeciesCodex, organic.Species, StringComparison.OrdinalIgnoreCase)
                                        && x.ScanStage == OrganicScanStage.Analyse);

                                    if (bio is not null)
                                    {
                                        bio.DataState = DataState.Sold;
                                    }
                                }
                            }

                            TriggerBioSoldIfLive();
                        }
                        break;
                }
            }
            catch (NullReferenceException ex)
            {
                App.Logger.Error(ex, "Null Reference parsing journal logs");
            }
            catch (Exception ex)
            {
                App.Logger.Error(ex, "Exception parsing journal logs");
            }

            if (parserStore.IsLive && ++eventsSinceLastSave >= 50)
            {
                eventsSinceLastSave = 0;
                SaveCartoData();
            }
        }
        #endregion

        #endregion

        #region Public Data Methods
        public List<StarSystem> GetUnsoldCartoSystems()
        {
            return [.. _cartoData.Values.Where(x => x.UnsoldCount > 0).OrderBy(x => x.Name)];
        }

        public List<StarSystem> GetSoldCartoSystems()
        {
            return [.. _cartoData.Values.Where(x => x.SoldCount > 0).OrderBy(x => x.Name)];
        }

        public List<StarSystem> GetLostCartoSystems()
        {
            return [.. _cartoData.Values.Where(x => x.LostCount > 0).OrderBy(x => x.Name)];
        }

        internal string GetUnsoldCartoValueString()
        {
            var value = _cartoData.Values
                .SelectMany(x => x.SystemBodies)
                .Where(x => x.BodyDataState == DataState.Unsold)
                .Sum(x => x.UnsoldCommanderValue);

            return value == 0 ? "?" : value.ToString("N0");
        }

        internal string GetUnsoldExoValueString()
        {
            var value = _organicData
                .Where(x => x.OrganicScanItems?.Any(b => b.DataState == DataState.Unsold && b.ScanStage == OrganicScanStage.Analyse) == true)
                .Sum(x => x.MaxExoValue);

            return value == 0 ? "?" : value.ToString("N0");
        }
        #endregion

        #region Private Event Triggers
        private void ParserStore_OnParserStoreLive(object? sender, bool e)
        {
            if (e)
            {
                InvokeLive(() =>
                {
                    ReprocessOrganicBodiesAtLive();
                    OnCurrentSystemUpdated?.Invoke(this, CurrentSystem);
                    OnRouteUpdated?.Invoke(this, new List<StarSystem>(Route));
                });
            }
        }

        // Go-live catch-up: during the history parse the FSSBodySignals handler only
        // tracks bodies while IsLive, so bodies that were signalled but never scanned
        // never entered _organicData. Re-run predictions for any body still holding
        // "Not Predicted" placeholders now that the full system context (position,
        // star types) is known, and surface the changes. Toasts are suppressed here:
        // these are historical discoveries, not new live ones.
        private void ReprocessOrganicBodiesAtLive()
        {
            foreach (var system in _cartoData.Values)
            {
                foreach (var body in system.SystemBodies)
                {
                    if (body.BiologicalSignals <= 0)
                        continue;

                    if (_organicData.Contains(body) == false)
                        _organicData.Add(body);

                    if (body.OrganicScanItems?.Any(x => x.GenusLocalised == "Not Predicted") == true)
                    {
                        if (UpdateBioPredictions(body, DateTime.UtcNow, notify: false))
                            TriggerBodyBiosUpdatedIfLive(body);
                    }
                }
            }
        }

        private void InvokeLive(Action action)
        {
            DispatcherHelper.Invoke(action);
        }

        private void TriggerCurrentSystemEventIfLive()
        {
            if (parserStore.IsLive)
                InvokeLive(() => OnCurrentSystemUpdated?.Invoke(this, CurrentSystem));
        }

        private void TriggerBodyEventIfLive(SystemBody body)
        {
            if (parserStore.IsLive)
            {
                if (settingsStore.NotificationOptions.HasFlag(NotificationOptions.WorthMapping)
                    && body.Status == DiscoveryStatus.WorthMapping)
                {
                    notificationStore.ShowWorthMappingNotification(body);
                }

                InvokeLive(() => OnBodyUpdated?.Invoke(this, body));
            }
        }

        private void TriggerBodyBiosUpdatedIfLive(SystemBody body)
        {
            if (parserStore.IsLive)
                InvokeLive(() => OnBodyBiosUpdated?.Invoke(this, body));
        }

        private void TriggerBioUpdatedIfLive(OrganicScanItem bio, bool fromCodex = false)
        {
            if (parserStore.IsLive)
            {
                if (!fromCodex)
                    SetCurrentBio(bio);

                if ((!fromCodex || !onFoot)
                    && bio.Info is not null
                    && settingsStore.NotificationOptions.HasFlag(NotificationOptions.NewBioScanned))
                {
                    notificationStore.ShowExoBioNotification(bio, "");
                }

                InvokeLive(() => OnBioDataUpdated?.Invoke(this, bio));
            }
        }

        private void ParserStore_StatusUpdated(object? sender, StatusFileEvent e)
        {
            CheckCurrentBody(e);

            latitude = e.Latitude;
            longitude = e.Longitude;

            if (latitude == 0 && longitude == 0)
                return;

            if (CurrentBioItem is null)
                return;

            if (string.IsNullOrEmpty(e.BodyName))
            {
                CurrentBioItem = null;
                return;
            }

            if (CurrentBioItem.Info is null || CurrentBioItem.ScanStage <= OrganicScanStage.Codex)
                return;

            var colonyRange = CurrentBioItem.Info.ColonyRange;

            foreach (var scan in CurrentBioItem.ScanLocations)
            {
                scan.Distance = BodyHelpers.DistanceBetweenLongLats(scan.Latitude, scan.Longitude, e.Latitude, e.Longitude, e.PlanetRadius);

                scan.DistanceState = scan.Distance > colonyRange ? ScanNotificationState.FarEnough : ScanNotificationState.TooClose;
            }

            if (settingsStore.NotificationOptions.HasFlag(NotificationOptions.DistanceFromBio) == false)
                return;

            var locations = CurrentBioItem.ScanLocations.Where(x => x.ScanStage > OrganicScanStage.Codex);

            if (locations.All(x => x.HasPos && x.DistanceState == ScanNotificationState.FarEnough)
                && CurrentBioItem.NotificationState == ScanNotificationState.TooClose)
            {
                CurrentBioItem.NotificationState = ScanNotificationState.FarEnough;
                notificationStore.ShowExoBioNotification(CurrentBioItem, "Minimum Distance Travelled");
                return;
            }

            if (locations.Any(x => x.HasPos && x.DistanceState == ScanNotificationState.TooClose)
                && CurrentBioItem.NotificationState == ScanNotificationState.FarEnough)
            {
                CurrentBioItem.NotificationState = ScanNotificationState.TooClose;
                notificationStore.ShowExoBioNotification(CurrentBioItem, "Moved Too Close To Scans");
            }
        }

        private void CheckCurrentBody(StatusFileEvent e)
        {
            if (currentBodyDestinationId != e.Destination.Body && string.IsNullOrEmpty(e.BodyName))
            {
                var known = CurrentSystem?.SystemBodies.FirstOrDefault(x => x.BodyID == e.Destination.Body);

                if (known is not null)
                {
                    currentBodyDestinationId = known.BodyID;
                    OnBodyTargeted?.Invoke(this, known);
                    return;
                }
            }

            if (string.IsNullOrEmpty(e.BodyName) == false)
            {
                var known = CurrentSystem?.SystemBodies.FirstOrDefault(x => x.BodyName.Equals(e.BodyName));

                if (known is not null && known.BodyID != currentBodyDestinationId)
                {
                    currentBodyDestinationId = known.BodyID;
                    OnBodyTargeted?.Invoke(this, known);
                }
            }
        }

        private void SetCurrentBio(OrganicScanItem scanItem)
        {
            if (scanItem.ScanStage == OrganicScanStage.Analyse)
            {
                CurrentBioItem = null;
                return;
            }

            // If we've scanned something new without finishing the last item
            // then clear the scan locations so a far walk isn't falsely rewarded.
            if (CurrentBioItem != scanItem && CurrentBioItem?.ScanStage < OrganicScanStage.Analyse)
            {
                CurrentBioItem.ScanLocations.RemoveAll(x => x.ScanStage >= OrganicScanStage.Codex);
                CurrentBioItem.NotificationState = ScanNotificationState.TooClose;
            }

            CurrentBioItem = scanItem;
        }

        private void TriggerBioSoldIfLive()
        {
            if (parserStore.IsLive)
                InvokeLive(() => OnBioDataSold?.Invoke(this, EventArgs.Empty));
        }

        private void TriggerBioLostIfLive()
        {
            if (parserStore.IsLive)
                InvokeLive(() => OnBioDataLost?.Invoke(this, EventArgs.Empty));
        }
        #endregion

        #region System/Body Construction
        private StarSystem BuildSystem(string name, long address, double[] starPos, StarType starType)
        {
            var pos = starPos.Length == 3
                ? new Position(starPos[0], starPos[1], starPos[2])
                : new Position(0, 0, 0);

            var region = RegionMap.FindRegion(pos.X, pos.Y, pos.Z);

            return new StarSystem
            {
                Name = name,
                Address = address,
                SystemAddress = address,
                Position = pos,
                StarType = starType,
                BodyCount = 0,
                Region = new SystemRegion { Name = region.Name },
                SystemBodies = []
            };
        }

        private StarSystem CheckIfSystemKnown(StarSystem system)
        {
            if (_cartoData.TryGetValue(system.Address, out var value))
            {
                value.Position = system.Position;
                value.Region = new SystemRegion { Name = RegionMap.FindRegion(system.Position.X, system.Position.Y, system.Position.Z).Name };
                if (value.StarType == StarType.Unknown && system.StarType != StarType.Unknown)
                    value.StarType = system.StarType;
                return value;
            }
            _cartoData.TryAdd(system.Address, system);
            return system;
        }

        private StarSystem UpdateCurrentSystem(StarSystem newSystem)
        {
            var system = CheckIfSystemKnown(newSystem);

            var known = Route.FirstOrDefault(x => x.Address == system.Address);

            if (known != null)
            {
                var index = Route.IndexOf(known);
                Route.RemoveRange(0, index + 1);
                CurrentSystem = known;
                CurrentSystem.VisitedByCommander = true;
                TriggerCurrentSystemEventIfLive();
                if (parserStore.IsLive)
                {
                    var updatedRouteSnapshot = new List<StarSystem>(Route);
                    InvokeLive(() => OnRouteUpdated?.Invoke(this, updatedRouteSnapshot));
                    var captured = CurrentSystem;
                    _ = Task.Run(async () =>
                    {
                        if (await UpdateKnownBodyCount(captured).ConfigureAwait(true))
                            InvokeLive(() => OnSystemUpdatedFromEDSM?.Invoke(this, captured));
                    });
                }
                return CurrentSystem;
            }

            CurrentSystem = system;
            CurrentSystem.VisitedByCommander = true;
            CurrentSystem.AllBodiesFound = false;
            CurrentSystem.EdsmUrl = string.Empty;

            TriggerCurrentSystemEventIfLive();

            if (parserStore.IsLive)
            {
                var captured = CurrentSystem;
                _ = Task.Run(async () =>
                {
                    var starUpdate = captured.StarType == StarType.Unknown && await UpdateSystemStarClass(captured).ConfigureAwait(true);
                    var valueUpdate = captured.EstimatedValue == 0 && await GetSystemValue(captured).ConfigureAwait(true);
                    var countUpdate = captured.BodyCount == 0 && await UpdateKnownBodyCount(captured).ConfigureAwait(true);
                    if ((starUpdate || valueUpdate || countUpdate) && CurrentSystem == captured)
                        InvokeLive(() => OnSystemUpdatedFromEDSM?.Invoke(this, captured));
                });
            }

            return CurrentSystem;
        }

        private async Task<bool> UpdateSystemStarClass(StarSystem system)
        {
            if (parserStore.IsLive == false || system is null)
                return false;

            var starClass = await edsmApi.GetPrimaryStarClassAsync(system.Name).ConfigureAwait(true);

            if (starClass != StarType.Unknown)
            {
                var ret = system.StarType != starClass;
                system.StarType = starClass;
                return ret;
            }

            return false;
        }

        private async Task<bool> UpdateKnownBodyCount(StarSystem system)
        {
            var count = await edsmApi.GetBodyCountAsync(system.Address).ConfigureAwait(true);

            var ret = system.BodyCount != count.Count;
            system.BodyCount = count.Count;
            system.EdsmScannedBodyCount = count.Scanned;
            if (count.Count > 0)
            {
                system.IsKnownToEDSM = true;
            }
            return ret;
        }

        private async Task<bool> GetSystemValue(StarSystem system)
        {
            var value = await edsmApi.GetSystemValueAsync(system.Name).ConfigureAwait(true);
            bool ret = false;

            if (value is not null)
            {
                if (value.ValuableBodies is not null)
                {
                    foreach (var body in value.ValuableBodies)
                    {
                        bool bodyKnown = system.SystemBodies.FirstOrDefault(x => x.EdsmBodyID == body.BodyId || string.Equals(x.BodyName, body.BodyName, StringComparison.OrdinalIgnoreCase)) != default;

                        if (bodyKnown)
                        {
                            continue;
                        }

                        var planet = CreateMinimalBody(body.BodyId, body.BodyName, system);
                        planet.EdsmBodyID = (int)body.BodyId;
                    }
                }

                system.EdsmUrl = value.Url ?? string.Empty;
                system.IsKnownToEDSM = true;
                system.EstimatedValue = value.EstimatedValueMapped;
                ret = true;
            }

            return ret;
        }

        private void UpdateCurrentBody(SystemBody? body)
        {
            CurrentBody = body;

            if (parserStore.IsLive)
                InvokeLive(() => OnCurrentSystemBodyUpdated?.Invoke(this, CurrentBody));
        }

        private SystemBody CreateMinimalBody(long bodyId, string bodyName, StarSystem system)
        {
            var body = new SystemBody
            {
                BodyID = bodyId,
                BodyName = string.IsNullOrEmpty(bodyName) ? $"{system.Name} {bodyId}" : bodyName,
                IsPlanet = true,
                Owner = new Owner
                {
                    Address = system.Address,
                    Name = system.Name,
                    Region = system.Region,
                    SystemBodies = system.SystemBodies
                },
                Parents = [],
                ScanState = BodyScanState.None,
                PlanetClass = PlanetClass.Unknown
            };
            system.SystemBodies.Add(body);
            return body;
        }

        private SystemBody GetOrCreateBody(StarSystem system, long bodyId, string bodyName)
        {
            var body = system.SystemBodies.FirstOrDefault(x => x.BodyID == bodyId);

            if (body is not null)
                return body;

            return CreateMinimalBody(bodyId, bodyName, system);
        }

        private void SetOwner(SystemBody body, StarSystem system)
        {
            if (body.Owner is null || body.Owner.Address != system.Address)
            {
                body.Owner = new Owner
                {
                    Address = system.Address,
                    Name = system.Name,
                    Region = system.Region,
                    SystemBodies = system.SystemBodies
                };
            }
        }

        private SystemBody AddOrUpdateBodyFromScan(StarSystem system, ScanEvent.ScanEventArgs scanEvt)
        {
            var body = GetOrCreateBody(system, scanEvt.BodyID, scanEvt.BodyName);
            SetOwner(body, system);

            var isStar = scanEvt.PlanetClass == EliteJournalReader.PlanetClass.Unknown;
            body.IsStar = isStar;
            body.IsPlanet = !isStar;
            body.PlanetClass = isStar ? PlanetClass.Unknown : JournalEventMapper.GetPlanetClass(scanEvt.PlanetClass);
            body.StarType = JournalEventMapper.GetStarType(scanEvt.StarType);
            body.StarLuminosity = JournalEventMapper.GetStarLuminosity(scanEvt.Luminosity);
            body.StellarMass = scanEvt.StellarMass > 0 ? scanEvt.StellarMass : null;
            body.Age_MY = scanEvt.Age_MY ?? 0;
            body.AbsoluteMagnitude = scanEvt.AbsoluteMagnitude ?? 0;
            body.DistanceFromArrivalLs = scanEvt.DistanceFromArrivalLs;
            // Journal emits orbital/rotation periods in seconds; the model and UI
            // display them in days. Radius is in metres; the model/UI use km.
            // SurfaceGravity is m/s^2; the model compares against g-based thresholds.
            body.OrbitalPeriod = (scanEvt.OrbitalPeriod ?? 0) / 86400;
            body.RotationPeriod = (scanEvt.RotationPeriod ?? 0) / 86400;
            body.AxialTilt = scanEvt.AxialTilt ?? 0;
            body.Eccentricity = scanEvt.Eccentricity ?? 0;
            body.SemiMajorAxis = scanEvt.SemiMajorAxis ?? 0;
            body.Radius = (scanEvt.Radius ?? 0) / 1000;
            body.MassEM = scanEvt.MassEM ?? 0;
            body.SurfaceGravity = (scanEvt.SurfaceGravity ?? 0) / 9.80665;
            body.Landable = scanEvt.Landable ?? false;
            body.Terraformable = JournalEventMapper.IsTerraformable(scanEvt.TerraformState);
            body.TerraformState = scanEvt.TerraformState.ToString();
            body.Atmosphere = JournalEventMapper.GetAtmosphereClass(scanEvt.Atmosphere);
            body.AtmosphereDescription = scanEvt.Atmosphere;
            body.AtmosphereType = JournalEventMapper.GetAtmosphereType(scanEvt.AtmosphereType);
            body.Volcanism = JournalEventMapper.GetVolcanism(scanEvt.Volcanism);
            body.VolcanismName = JournalEventMapper.GetVolcanismName(scanEvt.Volcanism);
            body.SurfaceTemp = scanEvt.SurfaceTemperature ?? 0;
            body.SurfacePressure = scanEvt.SurfacePressure ?? 0;
            body.WasDiscovered = scanEvt.WasDiscovered ?? false;
            body.WasMapped = scanEvt.WasMapped ?? false;
            body.ScanDate = scanEvt.Timestamp;
            body.ScanState = BodyScanState.FssScanned;
            body.Rings = scanEvt.Rings is { Count: > 0 } ? [.. scanEvt.Rings] : null;
            body.Materials = scanEvt.Materials?.Select(m => new ShipMaterials { Name = m.Name.ToString(), Percent = m.Percent }).ToList();
            body.AtmosphereComposition = scanEvt.AtmosphereComposition?.Select(a => new ScanItemComponent { Name = a.Name, Percent = a.Percent }).ToList();

            if (scanEvt.Composition.HasValue)
            {
                body.Composition = new Composition
                {
                    Ice = scanEvt.Composition.Ice,
                    Rock = scanEvt.Composition.Rock,
                    Metal = scanEvt.Composition.Metal
                };
            }

            body.FssValue = JournalEventMapper.GetFssValue(body.PlanetClass);
            body.MappedValue = body.FssValue;
            body.UnsoldCommanderValue = body.FssValue;
            body.BodyDataState = DataState.Unsold;

            if (isStar && system.StarType == StarType.Unknown)
            {
                system.StarType = body.StarType;
            }

            body.GoverningStar = isStar ? body.StarType : GetGoverningStar(system, scanEvt.Parents);

            UpdateBioPredictions(body, scanEvt.Timestamp);

            RecalcSystemCounts(system);
            return body;
        }

        private StarType GetGoverningStar(StarSystem system, IReadOnlyList<BodyParent>? parents)
        {
            if (parents is not null)
            {
                foreach (var parent in parents)
                {
                    if (parent.Type == EliteJournalReader.Events.ParentType.Star)
                    {
                        var star = system.SystemBodies.FirstOrDefault(x => x.BodyID == parent.BodyID);
                        if (star is not null && star.StarType != StarType.Unknown)
                            return star.StarType;
                    }
                }
            }

            return system.SystemBodies.FirstOrDefault(x => x.IsStar)?.StarType ?? StarType.Unknown;
        }

        private void ApplyDssScan(SystemBody body, DateTime timeStamp)
        {
            body.DssScanned = true;
            body.WasMapped = true;
            body.MappedValue = body.FssValue > 0 ? (long)(body.FssValue * 1.5) : 0;
            body.UnsoldCommanderValue = body.MappedValue;
            body.ScanState = BodyScanState.DssScanned;
            body.ScanDate = timeStamp;

            UpdateBioMinMaxValue(body);
        }

        private void UpdateSignalsFound(SystemBody body, IEnumerable<(string Type, int Count)> signals, bool dssScanned, DateTime timeStamp)
        {
            var bioCount = 0;
            var geoCount = 0;

            foreach (var signal in signals)
            {
                if (signal.Type.StartsWith("$SAA_SignalType_Biological;", StringComparison.Ordinal))
                    bioCount += signal.Count;
                else
                    geoCount += signal.Count;
            }

            body.BiologicalSignals = bioCount;
            body.GeologicalSignals = geoCount;

            if (dssScanned)
            {
                body.DssScanned = true;
                body.ScanState = BodyScanState.DssScanned;
            }
            else if (body.ScanState == BodyScanState.None)
            {
                body.ScanState = BodyScanState.FssScanned;
            }

            if (bioCount > 0)
                EnsurePredictionPlaceholders(body, timeStamp);

            UpdateBioPredictions(body, timeStamp);

            UpdateBioMinMaxValue(body);
        }

        private void MarkBodiesSold(StarSystem system)
        {
            foreach (var body in system.SystemBodies)
            {
                if (body.BodyDataState == DataState.Unsold)
                {
                    body.BodyDataState = DataState.Sold;
                    body.SoldCommanderValue = body.MappedValue;
                    body.UnsoldCommanderValue = 0;
                }
            }

            RecalcSystemCounts(system);
        }

        private void MarkBodiesLost(StarSystem system)
        {
            foreach (var body in system.SystemBodies)
            {
                if (body.BodyDataState == DataState.Unsold)
                {
                    body.BodyDataState = DataState.Lost;
                    body.LostCommanderValue = body.MappedValue;
                    body.UnsoldCommanderValue = 0;
                }
            }

            RecalcSystemCounts(system);
        }

        private void RecalcSystemCounts(StarSystem system)
        {
            system.SoldCount = system.SystemBodies.Count(x => x.BodyDataState == DataState.Sold && x.MappedValue > 0);
            system.UnsoldCount = system.SystemBodies.Count(x => x.BodyDataState == DataState.Unsold && x.MappedValue > 0);
            system.LostCount = system.SystemBodies.Count(x => x.BodyDataState == DataState.Lost && x.MappedValue > 0);
            system.EstimatedValue = system.SystemBodies.Sum(x => x.MappedValue);
        }

        private DiscoveryStatus GetWorthMapping(SystemBody body)
        {
            if (body.PlanetClass is PlanetClass.EarthlikeBody or PlanetClass.WaterWorld
                or PlanetClass.AmmoniaWorld or PlanetClass.WaterGiant or PlanetClass.WaterGiantWithLife
                or PlanetClass.HeliumRichGasGiant or PlanetClass.HeliumGasGiant)
            {
                return DiscoveryStatus.WorthMapping;
            }

            return body.FssValue >= settingsStore.SystemGridSetting.ValuableBodyValue
                ? DiscoveryStatus.WorthMapping
                : DiscoveryStatus.Discovered;
        }
        #endregion

        #region Organic Handling
        private void ProcessScanOrganic(ScanOrganicEvent.ScanOrganicEventArgs scanOrganic)
        {
            if (string.IsNullOrEmpty(scanOrganic.Genus) || string.IsNullOrEmpty(scanOrganic.Species) || string.IsNullOrEmpty(scanOrganic.Variant))
                return;

            var body = CurrentBody;

            if (body is null || body.BodyID != scanOrganic.Body)
            {
                CurrentSystem ??= BuildSystem("Unknown", scanOrganic.SystemAddress, Array.Empty<double>(), StarType.Unknown);

                body = CurrentSystem.SystemBodies.FirstOrDefault(x => x.BodyID == scanOrganic.Body);

                body ??= CreateMinimalBody(scanOrganic.Body, string.Empty, CurrentSystem);
            }

            SetOwner(body, CurrentSystem);
            body.OrganicScanItems ??= new OrganicScanItemList();

            var known = body.OrganicScanItems.FirstOrDefault(x =>
                string.IsNullOrEmpty(x.SpeciesCodex) == false &&
                (string.Equals(x.SpeciesCodex, scanOrganic.Species, StringComparison.OrdinalIgnoreCase) ||
                 x.Variants.Exists(v => string.Equals(v.VariantCodex, scanOrganic.Variant, StringComparison.OrdinalIgnoreCase))));

            if (known is not null)
            {
                UpdateFromScan(known, scanOrganic, body);
                UpdateBioMinMaxValue(body);
                TriggerBioUpdatedIfLive(known);
                NotifyBioDiscoveries(body);
                return;
            }

            var notPredicted = body.OrganicScanItems.FirstOrDefault(x => x.GenusLocalised == "Not Predicted");

            if (notPredicted is not null)
            {
                UpdateFromScan(notPredicted, scanOrganic, body);
                UpdateBioMinMaxValue(body);
                TriggerBioUpdatedIfLive(notPredicted);
                NotifyBioDiscoveries(body);
                return;
            }

            var newBio = NewBioFromScan(scanOrganic, body);
            body.OrganicScanItems.Add(newBio);
            if (_organicData.Contains(body) == false)
                _organicData.Add(body);
            UpdateBioMinMaxValue(body);
            TriggerBioUpdatedIfLive(newBio);
            NotifyBioDiscoveries(body);
        }

        private void ProcessCodex(CodexEntryEvent.CodexEntryEventArgs codexEntry)
        {
            if (string.Equals("$Codex_SubCategory_Organic_Structures;", codexEntry.SubCategory, StringComparison.OrdinalIgnoreCase) == false)
                return;

            var body = CurrentBody;

            if (body is null || body.BodyID != codexEntry.BodyID)
            {
                CurrentSystem ??= BuildSystem(codexEntry.System, codexEntry.SystemAddress, Array.Empty<double>(), StarType.Unknown);

                body = CurrentSystem.SystemBodies.FirstOrDefault(x => x.BodyID == codexEntry.BodyID);

                body ??= CreateMinimalBody(codexEntry.BodyID, string.Empty, CurrentSystem);
            }

            SetOwner(body, CurrentSystem);
            body.OrganicScanItems ??= new OrganicScanItemList();

            var genus = string.Join('_', codexEntry.Name.Split('_').Take(3));

            if (_organicData.Contains(body))
            {
                var knownBios = body.OrganicScanItems.Where(x => x.GenusCodex.StartsWith(genus, StringComparison.OrdinalIgnoreCase)).ToList();

                if (knownBios.Count > 1)
                {
                    var species = string.Join('_', codexEntry.Name.Split('_').Take(4));

                    foreach (var bio in knownBios)
                    {
                        if (bio.SpeciesCodex.StartsWith(species, StringComparison.OrdinalIgnoreCase))
                        {
                            UpdateFromCodex(bio, codexEntry, body);
                            TriggerBioUpdatedIfLive(bio, true);
                            NotifyBioDiscoveries(body);
                        }
                        else
                        {
                            bio.ScanStage = OrganicScanStage.Prediction;
                            if (parserStore.IsLive)
                                InvokeLive(() => OnBioDataUpdated?.Invoke(this, bio));
                        }
                    }
                    UpdateBioMinMaxValue(body);
                    return;
                }

                if (knownBios.Count == 1)
                {
                    var bio = knownBios[0];
                    if (bio.GenusCodex.StartsWith(genus, StringComparison.OrdinalIgnoreCase))
                    {
                        UpdateFromCodex(bio, codexEntry, body);
                        TriggerBioUpdatedIfLive(bio, true);
                        NotifyBioDiscoveries(body);
                        return;
                    }
                }

                var fallback = body.OrganicScanItems.FirstOrDefault(x =>
                    codexEntry.Name_Localised.StartsWith(x.GenusLocalised, StringComparison.OrdinalIgnoreCase));

                if (fallback is not null)
                {
                    UpdateFromCodex(fallback, codexEntry, body);
                    TriggerBioUpdatedIfLive(fallback, true);
                    NotifyBioDiscoveries(body);
                    return;
                }

                var notPredicted = body.OrganicScanItems.FirstOrDefault(x => x.GenusLocalised == "Not Predicted");

                if (notPredicted is not null)
                {
                    UpdateFromCodex(notPredicted, codexEntry, body);
                    TriggerBioUpdatedIfLive(notPredicted, true);
                    NotifyBioDiscoveries(body);
                    return;
                }
            }

            var newBio = NewBioFromCodex(codexEntry, body);
            body.OrganicScanItems = [newBio];
            if (_organicData.Contains(body) == false)
                _organicData.Add(body);
            TriggerBioUpdatedIfLive(newBio, true);
            NotifyBioDiscoveries(body);
        }

        private void NotifyBioDiscoveries(SystemBody body)
        {
            if (parserStore.IsLive == false)
                return;

            // Valuable exo body: fired once per body once its value crosses the threshold.
            if (settingsStore.NotificationOptions.HasFlag(NotificationOptions.ValuableBioPlanet)
                && body.MinExoValue >= settingsStore.SystemGridSetting.ExoValuableBodyValue
                && _highValueExoNotified.Add(body))
            {
                var min = body.MinExoValue;
                var max = body.MaxExoValue;
                var valueString = min == max ? $"{max.FormatNumber()}" : $"{min.FormatNumber()} - {max.FormatNumber()}";
                var countString = body.BiologicalSignals > 1 ? $"{body.BiologicalSignals} Signals" : $"{body.BiologicalSignals} Signal";
                notificationStore.ShowHighValueExoBodyNotification(body.BodyName, valueString, countString);
            }

            if (body.OrganicScanItems is null)
                return;

            if (settingsStore.NotificationOptions.HasFlag(NotificationOptions.NewBioSpecies))
            {
                var newSpecies = new Dictionary<string, bool>();
                foreach (var item in body.OrganicScanItems.Where(x => x.IsNewSpecies && string.IsNullOrEmpty(x.SpeciesCodex) == false))
                {
                    if (_newSpeciesNotified.Add(item.SpeciesCodex))
                    {
                        var name = string.IsNullOrEmpty(item.SpeciesLocalised) ? item.SpeciesEnglish : item.SpeciesLocalised;
                        newSpecies.TryAdd(string.IsNullOrEmpty(name) ? item.SpeciesCodex : name, true);
                    }
                }

                if (newSpecies.Count > 0)
                    notificationStore.ShowNewSpeciesEntriesNotification(body.BodyName, newSpecies, CurrentSystemRegion);
            }

            if (settingsStore.NotificationOptions.HasFlag(NotificationOptions.NewBioCodexEntry))
            {
                var newCodex = new Dictionary<string, bool>();
                foreach (var variant in body.OrganicScanItems.SelectMany(x => x.Variants)
                             .Where(v => v.NewCodexEntry && string.IsNullOrEmpty(v.VariantCodex) == false))
                {
                    if (_newCodexNotified.Add(variant.VariantCodex))
                    {
                        var name = string.IsNullOrEmpty(variant.EnglishName) ? variant.VariantCodex : variant.EnglishName;
                        newCodex.TryAdd(name, true);
                    }
                }

                if (newCodex.Count > 0)
                    notificationStore.ShowNewCodexEntriesNotification(body.BodyName, newCodex, CurrentSystemRegion);
            }
        }

        private void UpdateFromScan(OrganicScanItem bio, ScanOrganicEvent.ScanOrganicEventArgs scanOrganic, SystemBody body)
        {
            bio.GenusCodex = scanOrganic.Genus;
            bio.GenusEnglish = scanOrganic.Genus_Localised;
            bio.GenusLocalised = scanOrganic.Genus_Localised;
            bio.SpeciesCodex = scanOrganic.Species;
            bio.SpeciesEnglish = scanOrganic.Species_Localised;
            bio.SpeciesLocalised = scanOrganic.Species_Localised;
            bio.VariantCodex = scanOrganic.Variant;
            bio.ScanStage = JournalEventMapper.GetOrganicScanStage(scanOrganic.ScanType);
            bio.ScanTime = scanOrganic.Timestamp;
            bio.DataState = DataState.Unsold;
            bio.Info = OrganicValues.GetOrganicInfo(scanOrganic.Species, scanOrganic.Species_Localised, scanOrganic.Timestamp);
            bio.IsNewSpecies = organicCheckListData.IsNewSpecies(scanOrganic.Species);
            bio.BodyDssScanned = body.DssScanned;
            bio.WasLogged = true;
            bio.ScanLocations.Add(new ScanLocation
            {
                Latitude = (double?)scanOrganic.OriginalEvent?["Latitude"] ?? 0,
                Longitude = (double?)scanOrganic.OriginalEvent?["Longitude"] ?? 0,
                ScanStage = bio.ScanStage
            });
            bio.Variants = BuildVariants(scanOrganic);
            bio.TotalValue = ComputeTotalValue(bio, body);
        }

        private void UpdateFromCodex(OrganicScanItem bio, CodexEntryEvent.CodexEntryEventArgs codexEntry, SystemBody body)
        {
            var names = ExoData.GetNames(codexEntry.Name);

            if (names is not null)
            {
                bio.GenusCodex = names.GenusCodex;
                bio.GenusEnglish = names.Genus;
                bio.GenusLocalised = names.Genus;
                bio.SpeciesCodex = names.SpeciesCodex;
                bio.SpeciesEnglish = names.Species;
                bio.SpeciesLocalised = names.Species;
            }

            bio.VariantCodex = codexEntry.Name;
            bio.ScanStage = OrganicScanStage.Codex;
            bio.ScanTime = codexEntry.Timestamp;
            bio.DataState = DataState.Unsold;
            bio.Info = OrganicValues.GetOrganicInfo(bio.SpeciesCodex, bio.SpeciesLocalised, bio.ScanTime);
            bio.IsNewSpecies = false;
            bio.BodyDssScanned = body.DssScanned;
            bio.WasLogged = true;

            var variantColour = GetVariantColour(codexEntry.Name);

            bio.Variants =
            [
                new OrganicVariant
                {
                    VariantCodex = codexEntry.Name,
                    EnglishName = codexEntry.Name_Localised,
                    LocalName = codexEntry.Name_Localised,
                    Colour = variantColour,
                    Confirmed = true,
                    NewCodexEntry = true
                }
            ];

            bio.TotalValue = ComputeTotalValue(bio, body);
        }

        private OrganicScanItem NewBioFromScan(ScanOrganicEvent.ScanOrganicEventArgs scanOrganic, SystemBody body)
        {
            var scanLon = (double?)scanOrganic.OriginalEvent?["Longitude"] ?? 0;
            var scanLat = (double?)scanOrganic.OriginalEvent?["Latitude"] ?? 0;

            var bio = new OrganicScanItem
            {
                GenusCodex = scanOrganic.Genus,
                GenusEnglish = scanOrganic.Genus_Localised,
                GenusLocalised = scanOrganic.Genus_Localised,
                SpeciesCodex = scanOrganic.Species,
                SpeciesEnglish = scanOrganic.Species_Localised,
                SpeciesLocalised = scanOrganic.Species_Localised,
                VariantCodex = scanOrganic.Variant,
                Body = body,
                ScanStage = JournalEventMapper.GetOrganicScanStage(scanOrganic.ScanType),
                DataState = DataState.Unsold,
                ScanTime = scanOrganic.Timestamp,
                IsNewSpecies = organicCheckListData.IsNewSpecies(scanOrganic.Species),
                BodyDssScanned = body.DssScanned,
                WasLogged = true,
                Info = OrganicValues.GetOrganicInfo(scanOrganic.Species, scanOrganic.Species_Localised, scanOrganic.Timestamp),
                ScanLocations =
                [
                    new ScanLocation
                    {
                        Latitude = scanLat,
                        Longitude = scanLon,
                        ScanStage = JournalEventMapper.GetOrganicScanStage(scanOrganic.ScanType)
                    }
                ],
                Variants = BuildVariants(scanOrganic)
            };

            bio.TotalValue = ComputeTotalValue(bio, body);
            return bio;
        }

        private OrganicScanItem NewBioFromCodex(CodexEntryEvent.CodexEntryEventArgs codexEntry, SystemBody body)
        {
            var names = ExoData.GetNames(codexEntry.Name);
            var info = names is null ? null : OrganicValues.GetOrganicInfo(names.SpeciesCodex, names.Species ?? string.Empty, codexEntry.Timestamp);

            var bio = new OrganicScanItem
            {
                GenusCodex = names?.GenusCodex ?? string.Empty,
                GenusEnglish = names?.Genus ?? string.Empty,
                GenusLocalised = names?.Genus ?? codexEntry.Name_Localised,
                SpeciesCodex = names?.SpeciesCodex ?? string.Empty,
                SpeciesEnglish = names?.Species ?? string.Empty,
                SpeciesLocalised = names?.Species ?? codexEntry.Name_Localised,
                VariantCodex = codexEntry.Name,
                Body = body,
                ScanStage = OrganicScanStage.Codex,
                DataState = DataState.Unsold,
                ScanTime = codexEntry.Timestamp,
                IsNewSpecies = false,
                BodyDssScanned = body.DssScanned,
                WasLogged = true,
                Info = info,
                ScanLocations =
                [
                    new ScanLocation
                    {
                        Latitude = codexEntry.Latitude,
                        Longitude = codexEntry.Longitude,
                        ScanStage = OrganicScanStage.Codex
                    }
                ],
                Variants =
                [
                    new OrganicVariant
                    {
                        VariantCodex = codexEntry.Name,
                        EnglishName = codexEntry.Name_Localised,
                        LocalName = codexEntry.Name_Localised,
                        Colour = GetVariantColour(codexEntry.Name),
                        Confirmed = true,
                        NewCodexEntry = true
                    }
                ]
            };

            bio.TotalValue = ComputeTotalValue(bio, body);
            return bio;
        }

        private OrganicScanItem NewNotPredictedItem(SystemBody body, DateTime timeStamp)
        {
            return new OrganicScanItem
            {
                GenusCodex = string.Empty,
                GenusEnglish = string.Empty,
                GenusLocalised = "Not Predicted",
                SpeciesCodex = string.Empty,
                SpeciesEnglish = string.Empty,
                SpeciesLocalised = string.Empty,
                Body = body,
                ScanStage = OrganicScanStage.Prediction,
                DataState = DataState.Unsold,
                ScanTime = timeStamp,
                IsNewSpecies = false,
                BodyDssScanned = body.DssScanned,
                WasLogged = false,
                Variants = [],
                Info = null,
                ScanLocations = []
            };
        }

        private List<OrganicVariant> BuildVariants(ScanOrganicEvent.ScanOrganicEventArgs scanOrganic)
        {
            var colour = GetVariantColour(scanOrganic.Variant);

            return
            [
                new OrganicVariant
                {
                    VariantCodex = scanOrganic.Variant,
                    EnglishName = scanOrganic.Variant_Localised,
                    LocalName = scanOrganic.Variant_Localised,
                    Colour = colour,
                    Confirmed = true,
                    NewCodexEntry = organicCheckListData.IsNewCodex(scanOrganic.Variant)
                }
            ];
        }

        private VariantColours GetVariantColour(string variantCodex)
        {
            var genus = exoData.AllGenus.FirstOrDefault(g => g.Species.Any(s => s.Variants.Any(v => v.Codex == variantCodex)));
            var species = genus?.Species.FirstOrDefault(s => s.Variants.Any(v => v.Codex == variantCodex));
            var variant = species?.Variants.FirstOrDefault(v => v.Codex == variantCodex);

            return variant?.Colour ?? VariantColours.Unknown;
        }

        private long ComputeTotalValue(OrganicScanItem bio, SystemBody body)
        {
            var value = bio.Info?.Value ?? 0;
            if (value == 0)
                return 0;

            var bonus = body.WasMapped ? 0 : bio.ScanTime < OrganicValues.NewPriceDate ? value : value * 4;
            return value + bonus;
        }

        // Replaces the "Not Predicted" placeholders on a body with the species that
        // match its scan data, using the ODUtils exobiology prediction engine. Runs
        // as soon as the body has scan data (Scan event) or bio signals
        // (FSSBodySignals / SAASignalsFound); a no-op otherwise. A body with no
        // matching rules legitimately stays "Not Predicted". When a prediction is
        // filled while the parser is live, valuable-exo toasts are raised; the
        // go-live history catch-up passes notify: false to avoid a burst of toasts
        // for every historical body.
        private bool UpdateBioPredictions(SystemBody body, DateTime timeStamp, bool notify = true)
        {
            if (body.BiologicalSignals <= 0 || body.PlanetClass == PlanetClass.Unknown)
                return false;

            if (_cartoData.TryGetValue(body.Owner.Address, out var system) == false)
                return false;

            var predicted = exoData.GetPredictions(ExoPlanetBuilder.Build(body, system, timeStamp))
                .Values.SelectMany(x => x)
                .DistinctBy(x => x.SpeciesCodex)
                .ToList();
            if (predicted.Count == 0)
                return false;

            EnsurePredictionPlaceholders(body, timeStamp);

            var placeholders = body.OrganicScanItems.Where(x => x.GenusLocalised == "Not Predicted").ToList();
            if (placeholders.Count == 0)
                return false;

            var filled = false;
            for (int i = 0; i < Math.Min(placeholders.Count, predicted.Count); i++)
            {
                var pred = predicted[i];
                var bio = placeholders[i];

                bio.GenusCodex = pred.GenusCodex;
                bio.GenusEnglish = pred.GenusEnglishName;
                bio.GenusLocalised = pred.GenusEnglishName;
                bio.SpeciesCodex = pred.SpeciesCodex;
                bio.SpeciesEnglish = pred.SpeciesEnglishName;
                bio.SpeciesLocalised = pred.SpeciesEnglishName;
                bio.ScanStage = OrganicScanStage.Prediction;
                bio.ScanTime = timeStamp;
                bio.Info = OrganicValues.GetOrganicInfo(pred.SpeciesCodex, pred.SpeciesEnglishName, timeStamp);
                bio.BodyDssScanned = body.DssScanned;
                bio.TotalValue = ComputeTotalValue(bio, body);
                filled = true;
            }

            if (filled)
            {
                UpdateBioMinMaxValue(body);
                if (notify)
                    NotifyBioDiscoveries(body);
            }

            return filled;
        }

        // Tops the body's organic items up to BiologicalSignals with "Not Predicted"
        // placeholders, creating the list on demand. Called from the signals handlers
        // and again before a re-prediction so a late Scan never loses a slot.
        private void EnsurePredictionPlaceholders(SystemBody body, DateTime timeStamp)
        {
            body.OrganicScanItems ??= new OrganicScanItemList();

            for (int i = body.OrganicScanItems.Count; i < body.BiologicalSignals; i++)
                body.OrganicScanItems.Add(NewNotPredictedItem(body, timeStamp));
        }

        private void UpdateBioMinMaxValue(SystemBody body)
        {
            var min = 0L;
            var max = 0L;

            if (body.OrganicScanItems is not null)
            {
                var valued = body.OrganicScanItems.Where(x => x.Info is not null && x.ScanStage >= OrganicScanStage.Prediction);

                foreach (var bio in valued)
                {
                    min += bio.Info!.Value;
                    max += bio.Info!.Value;
                }
            }

            if (body.MinExoValue != min || body.MaxExoValue != max)
            {
                body.MinExoValue = min;
                body.MaxExoValue = max;

                if (parserStore.IsLive)
                    InvokeLive(() => OnExoMinValueChanged?.Invoke(this, EventArgs.Empty));
            }
        }
        #endregion
    }
}
