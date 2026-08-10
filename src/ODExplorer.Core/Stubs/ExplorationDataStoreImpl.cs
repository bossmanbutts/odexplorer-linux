// Functional in-memory ExplorationDataStore for the journal→carto/organic pipeline.
// Parses typed journal events into the ODUtils.Models models and raises the same
// events as the real store so the ViewModels work unchanged. EDSM lookups and
// exo predictions are approximated; the real store can replace this later.

using System;
using System.Collections.Generic;
using System.Linq;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODExplorer.Database;
using ODExplorer.Journal;
using ODExplorer.Models;
using ODUtils.APis;
using ODUtils.Database.Interfaces;
using ODUtils.EliteDangerousHelpers.GalacticRegions;
using ODUtils.Exobiology;
using ODUtils.Extensions;
using ODUtils.Journal;
using ODUtils.Models;
using System.Threading.Tasks;
using JournalEntry = EliteJournalReader.JournalEntry;
using SystemBody = ODUtils.Models.SystemBody;

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

        private readonly Dictionary<long, StarSystem> _cartoData = [];
        private readonly List<SystemBody> _organicData = [];
        private readonly Dictionary<long, string> _ignoredSystems = [];

        private readonly HashSet<SystemBody> _highValueExoNotified = [];
        private readonly HashSet<string> _newSpeciesNotified = [];
        private readonly HashSet<string> _newCodexNotified = [];

        private bool onFoot;
        private double longitude;
        private double latitude;
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
            PopulateIgnoredSystems(currentCmdrId);
        }

        public Task ParseHistoryStream(JournalEntry entry)
        {
            ParseJournalEvent(entry);
            return Task.CompletedTask;
        }

        public void ParseHistory(IEnumerable<JournalEntry> journalEntries, int currentCmdrId)
        {
            PopulateIgnoredSystems(currentCmdrId);

            foreach (var journalEntry in journalEntries)
            {
                ParseJournalEvent(journalEntry);
            }
        }

        public Task ParseHistoryStream(IEnumerable<JournalEntry> journalEntries, int currentCmdrId)
        {
            PopulateIgnoredSystems(currentCmdrId);

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

        public void ClearData()
        {
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
            parserStore.UnregisterParser(this);
            parserStore.OnParserStoreLive -= ParserStore_OnParserStoreLive;
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
                            longitude = locationEvt.Longitude;
                            latitude = locationEvt.Latitude;

                            var currentSys = UpdateCurrentSystem(BuildSystem(locationEvt.StarSystem, locationEvt.SystemAddress, locationEvt.StarPos, StarType.Unknown));

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
                            var currentSys = UpdateCurrentSystem(BuildSystem(carrierJump.StarSystem, carrierJump.SystemAddress, carrierJump.StarPos, StarType.Unknown));

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
                        UpdateCurrentSystem(BuildSystem(fsdJumpEvent.StarSystem, fsdJumpEvent.SystemAddress, fsdJumpEvent.StarPos,
                            JournalEventMapper.GetStarType(fsdJumpEvent.StarType)));
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

                                UpdateSignalsFound(body, fssBodySignals.Signals, false, fssBodySignals.Timestamp);

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
                                var body = AddOrUpdateBodyFromScan(system, scanEvt);

                                if (body.BiologicalSignals > 0 && _organicData.Contains(body) == false)
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

                                UpdateSignalsFound(body, ssaSignalsFound.Signals, true, ssaSignalsFound.Timestamp);

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
                        {
                            if (_cartoData.TryGetValue(scanBarycentre.SystemAddress, out var system))
                            {
                                var known = system.SystemBodies.FirstOrDefault(x => x.BodyID == scanBarycentre.BodyID);

                                if (known is null)
                                {
                                    var body = CreateMinimalBody(scanBarycentre.BodyID, string.Empty, system);
                                    system.SystemBodies.Add(body);
                                }
                            }
                        }
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
                            longitude = disembark.Longitude;
                            latitude = disembark.Latitude;

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
                    OnCurrentSystemUpdated?.Invoke(this, CurrentSystem);
                    OnRouteUpdated?.Invoke(this, new List<StarSystem>(Route));
                });
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
                if ((!fromCodex || !onFoot)
                    && bio.Info is not null
                    && settingsStore.NotificationOptions.HasFlag(NotificationOptions.NewBioScanned))
                {
                    notificationStore.ShowExoBioNotification(bio, "");
                }

                InvokeLive(() => OnBioDataUpdated?.Invoke(this, bio));
            }
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
                }
                return CurrentSystem;
            }

            CurrentSystem = system;
            CurrentSystem.VisitedByCommander = true;
            CurrentSystem.AllBodiesFound = false;
            CurrentSystem.EdsmUrl = string.Empty;

            TriggerCurrentSystemEventIfLive();
            return CurrentSystem;
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

            var isStar = string.IsNullOrEmpty(scanEvt.PlanetClass);
            body.IsStar = isStar;
            body.IsPlanet = !isStar;
            body.PlanetClass = isStar ? PlanetClass.Unknown : JournalEventMapper.GetPlanetClass(scanEvt.PlanetClass);
            body.StarType = JournalEventMapper.GetStarType(scanEvt.StarType);
            body.StarLuminosity = JournalEventMapper.GetStarLuminosity(scanEvt.StarClass);
            body.StellarMass = scanEvt.StellarMass > 0 ? scanEvt.StellarMass : null;
            body.Age_MY = scanEvt.Age_MY;
            body.AbsoluteMagnitude = scanEvt.AbsoluteMagnitude;
            body.DistanceFromArrivalLs = scanEvt.DistanceFromArrivalLS;
            body.OrbitalPeriod = scanEvt.OrbitalPeriod;
            body.RotationPeriod = scanEvt.RotationPeriod;
            body.AxialTilt = scanEvt.AxialTilt;
            body.Eccentricity = scanEvt.Eccentricity;
            body.SemiMajorAxis = scanEvt.SemiMajorAxis;
            body.Radius = scanEvt.Radius;
            body.MassEM = scanEvt.MassEM;
            body.SurfaceGravity = scanEvt.SurfaceGravity;
            body.Landable = scanEvt.Landable;
            body.Terraformable = JournalEventMapper.IsTerraformable(scanEvt.TerraformState);
            body.TerraformState = scanEvt.TerraformState;
            body.Atmosphere = JournalEventMapper.GetAtmosphereClass(scanEvt.Atmosphere);
            body.AtmosphereType = JournalEventMapper.GetAtmosphereType(scanEvt.AtmosphereType);
            body.Volcanism = JournalEventMapper.GetVolcanism(scanEvt.Volcanism);
            body.SurfaceTemp = scanEvt.SurfaceTemperature;
            body.SurfacePressure = scanEvt.SurfacePressure;
            body.WasDiscovered = scanEvt.WasDiscovered;
            body.WasMapped = scanEvt.WasMapped;
            body.ScanDate = scanEvt.Timestamp;
            body.ScanState = BodyScanState.FssScanned;
            body.Rings = scanEvt.Rings.Count > 0 ? scanEvt.Rings : null;
            body.Materials = scanEvt.Materials?.Select(m => new ShipMaterials { Name = m.Name, Percent = m.Percent }).ToList();
            body.AtmosphereComposition = scanEvt.AtmosphereComposition?.Select(a => new ScanItemComponent { Name = a.Name, Percent = a.Percent }).ToList();

            if (scanEvt.Composition is not null)
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

            RecalcSystemCounts(system);
            return body;
        }

        private StarType GetGoverningStar(StarSystem system, List<JournalParent>? parents)
        {
            if (parents is not null)
            {
                foreach (var parent in parents)
                {
                    if (parent.Star > 0)
                    {
                        var star = system.SystemBodies.FirstOrDefault(x => x.BodyID == parent.Star);
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

        private void UpdateSignalsFound(SystemBody body, List<SignalFound> signals, bool dssScanned, DateTime timeStamp)
        {
            var bioCount = 0;
            var geoCount = 0;

            foreach (var signal in signals)
            {
                if (signal.IsBiological)
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

            if (bioCount > 0 && (body.OrganicScanItems is null || body.OrganicScanItems.Count == 0))
            {
                body.OrganicScanItems = new OrganicScanItemList();
                for (int i = 0; i < bioCount; i++)
                {
                    body.OrganicScanItems.Add(NewNotPredictedItem(body, timeStamp));
                }
            }

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

                body ??= CreateMinimalBody(codexEntry.BodyID, codexEntry.Body, CurrentSystem);
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
            bio.ScanStage = scanOrganic.ScanType;
            bio.ScanTime = scanOrganic.Timestamp;
            bio.DataState = DataState.Unsold;
            bio.Info = exoData.GetInfo(scanOrganic.Species);
            bio.IsNewSpecies = organicCheckListData.IsNewSpecies(scanOrganic.Species);
            bio.BodyDssScanned = body.DssScanned;
            bio.WasLogged = true;
            bio.ScanLocations.Add(new Position(scanOrganic.Longitude, scanOrganic.Latitude, 0));
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
            bio.Info = exoData.GetInfo(bio.SpeciesCodex);
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
                ScanStage = scanOrganic.ScanType,
                DataState = DataState.Unsold,
                ScanTime = scanOrganic.Timestamp,
                IsNewSpecies = organicCheckListData.IsNewSpecies(scanOrganic.Species),
                BodyDssScanned = body.DssScanned,
                WasLogged = true,
                Info = exoData.GetInfo(scanOrganic.Species),
                ScanLocations = [new Position(scanOrganic.Longitude, scanOrganic.Latitude, 0)],
                Variants = BuildVariants(scanOrganic)
            };

            bio.TotalValue = ComputeTotalValue(bio, body);
            return bio;
        }

        private OrganicScanItem NewBioFromCodex(CodexEntryEvent.CodexEntryEventArgs codexEntry, SystemBody body)
        {
            var names = ExoData.GetNames(codexEntry.Name);
            var info = names is null ? null : exoData.GetInfo(names.SpeciesCodex);

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
                ScanLocations = [new Position(codexEntry.Longitude, codexEntry.Latitude, 0)],
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

        private void UpdateBioMinMaxValue(SystemBody body)
        {
            var min = 0L;
            var max = 0L;

            if (body.OrganicScanItems is not null)
            {
                var valued = body.OrganicScanItems.Where(x => x.Info is not null && x.ScanStage >= OrganicScanStage.Codex);

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
