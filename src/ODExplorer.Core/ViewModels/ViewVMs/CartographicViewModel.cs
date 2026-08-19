using ODExplorer.Models;
using ODExplorer.Stores;
using ODExplorer.ViewModels.ModelVMs;
using ODUtils.Commands;
using ODUtils.Dialogs.ViewModels;
using ODUtils.Spansh;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ODExplorer.ViewModels.ViewVMs
{
    public sealed class CartographicViewModel : OdViewModelBase
    {
        public CartographicViewModel(ExplorationDataStore explorationData,
                             JournalParserStore parserStore,
                             SettingsStore settingsStore,
                             MainViewModel mainView)
        {
            this.explorationData = explorationData;
            this.parserStore = parserStore;
            this.settingsStore = settingsStore;
            this.mainView = mainView;

            this.mainView.OnCurrentSystemUpdatedEvent += MainView_OnCurrentSystemUpdatedEvent;
            this.mainView.OnRouteUpdated += MainView_OnRouteUpdated;
            this.mainView.OnBodyUpdated += MainView_OnBodyUpdated;
            this.mainView.OnBioUpdated += MainView_OnBioUpdated;
            this.parserStore.OnParserStoreLive += (_, _) => DispatcherHelper.Invoke(() => OnPropertyChanged(nameof(CurrentState)));

            this.mainView.OnSelectedBodyUpdated += MainView_OnSelectedBodyUpdated;
            this.explorationData.OnFSDJump += ExplorationData_OnFSDJump;
            explorationData.OnCartoDataSold += ExplorationData_OnCartoValueChanged;
            explorationData.OnCartoDataLost += ExplorationData_OnCartoValueChanged;
            explorationData.OnBioDataSold += ExplorationData_OnBioDataSold;

            this.spanshStore = mainView.SpanshCsvStore;
            this.spanshStore.OnCurrentTargetChanged += Spansh_OnCurrentTargetChanged;
            this.spanshStore.OnCurrentContainerChanged += Spansh_OnCurrentContainerChanged;

            SwitchView = new RelayCommand<CartoViewState>(OnSwitchView);
            CopySystemName = new RelayCommand(OnCopySystemName);
            CycleSortDirection = new RelayCommand(OnCycleSortDirection);
            SetSortCategory = new RelayCommand<BodySortCategory>(OnSetSortCategory);

            ODExplorer.Models.DispatcherHelper.Invoke(() =>
            {
                if (CurrentSystem != null)
                {
                    RefreshBodiesView();
                }

                _ = Task.Factory.StartNew(() =>
                {
                    MainView_OnCurrentSystemUpdatedEvent(null, this.mainView.CurrentSystem);
                    MainView_OnRouteUpdated("Start up", this.mainView.Route);
                }).ConfigureAwait(true);
            });
        }

        private readonly ExplorationDataStore explorationData;
        private readonly JournalParserStore parserStore;
        private readonly SettingsStore settingsStore;
        private readonly MainViewModel mainView;
        private readonly SpanshCsvStore spanshStore;

        public StarSystemViewModel? CurrentSystem => mainView.CurrentSystem;
        public ObservableCollection<StarSystemViewModel> Route => mainView.Route;
        public ObservableCollection<SystemBodyViewModel> OrganicSignals => mainView.OrganicSignals;

        public bool HasSpanshRoute => spanshStore.CurrentContainer is { Targets.Count: > 0 };
        public string? SpanshTargetSystem => spanshStore.CurrentTarget?.SystemName;
        public string? SpanshTargetBodies => spanshStore.CurrentTarget?.BodiesInfo is { Count: > 0 }
            ? string.Join(", ", spanshStore.CurrentTarget.BodiesInfo.Select(x => x.Body).Where(x => string.IsNullOrWhiteSpace(x) == false))
            : null;
        public bool IsInSpanshTargetSystem => CurrentSystem != null
            && HasSpanshRoute
            && string.Equals(CurrentSystem.Name, SpanshTargetSystem, StringComparison.OrdinalIgnoreCase);

        public SystemBodyViewModel? SelectedBody
        {
            get
            {
                return mainView.SelectedBody;
            }
            set
            {
                mainView.SelectedBody = value;
                OnPropertyChanged(nameof(SelectedBody));
            }
        }

        private ObservableCollection<SystemBodyViewModel>? currentSystemBodies;
        public ObservableCollection<SystemBodyViewModel>? CurrentSystemBodies
        {
            get => currentSystemBodies;
            set
            {
                currentSystemBodies = value;
                OnPropertyChanged(nameof(CurrentSystemBodies));
            }
        }

        public CartoViewState CurrentState
        {
            get
            {
                if (parserStore.IsLive == false)
                {
                    return CartoViewState.None;
                }
                // Unified view — always DetailedExo
                return CartoViewState.DetailedExo;
            }
        }

        private bool inHyperSpace;
        public bool InHyperSpace
        {
            get => inHyperSpace;
            set
            {
                inHyperSpace = value;
                OnPropertyChanged(nameof(InHyperSpace));
            }
        }

        private string hyperSpaceText = string.Empty;
        public string HyperSpaceText
        {
            get => hyperSpaceText;
            set
            {
                hyperSpaceText = value;
                OnPropertyChanged(nameof(HyperSpaceText));
            }
        }

        public string CartoValue => explorationData.GetUnsoldCartoValueString();
        public string ExoValue => explorationData.GetUnsoldExoValueString();
        public bool FilterUnconfirmedBios => settingsStore.SystemGridSetting.FilterUnconfirmedBios;
        public SystemGridSettings CurrentSystemGridSettings => settingsStore.SystemGridSetting;
        public GridSize HorizontalViewGridSize => settingsStore.CartoHorizontalGridSize;
        public GridSize DetailedViewGridSize => settingsStore.CartoDetailedGridSize;
        public GridSize ExtendedBodyInfoGridSize => settingsStore.ExtendedBodyInfoGridSize;
        public GridSize CurrentExoGridSize => settingsStore.CurrentExoGridSize;

        // ── Sort options ────────────────────────────────────────────────────
        public BodySortCategory SelectedSortCategory => settingsStore.SystemGridSetting.BodySortingOptions;
        public bool SortAscending => settingsStore.SystemGridSetting.SortDirection == System.ComponentModel.ListSortDirection.Ascending;
        public string SortDirectionGlyph => SortAscending ? "▲" : "▼";

        private void MainView_OnSelectedBodyUpdated(object? sender, SystemBodyViewModel? e)
        {
            OnPropertyChanged(nameof(SelectedBody));
        }

        private void MainView_OnBioUpdated(object? sender, SystemBodyViewModel e)
        {
            OnPropertyChanged(nameof(ExoValue));
        }

        private void MainView_OnBodyUpdated(object? sender, SystemBodyViewModel e)
        {
            if (settingsStore.SystemGridSetting.IgnoreNonBodies && e.IsNonBody)
            {
                OnPropertyChanged(nameof(CartoValue));
                RefreshBodiesView();
                return;
            }
            OnPropertyChanged(nameof(SelectedBody));
            RefreshBodiesView();
            OnPropertyChanged(nameof(CartoValue));
            if (e.BiologicalSignals > 0)
                OnPropertyChanged(nameof(ExoValue));
        }

        private void MainView_OnRouteUpdated(object? sender, ObservableCollection<StarSystemViewModel> e)
        {
            OnPropertyChanged(nameof(Route));
        }

        private void MainView_OnCurrentSystemUpdatedEvent(object? sender, StarSystemViewModel? e)
        {
            ODExplorer.Models.DispatcherHelper.Invoke(() =>
            {
                InHyperSpace = false;
                HyperSpaceText = string.Empty;

                OnPropertyChanged(nameof(CurrentSystem));
                OnPropertyChanged(nameof(OrganicSignals));
                OnPropertyChanged(nameof(IsInSpanshTargetSystem));
                ApplyBodyCollectionViewSourceSorting();
            });
        }

        private void ApplyBodyCollectionViewSourceSorting()
        {
            if (CurrentSystem is null)
            {
                CurrentSystemBodies = null;
                return;
            }

            ODExplorer.Models.DispatcherHelper.Invoke(() =>
            {
                var gridSettings = settingsStore.SystemGridSetting;
                var comparer = new SystemBodyViewModelMainComparer(gridSettings);
                var filtered = CurrentSystem.Bodies
                    .Where(b => !gridSettings.IgnoreNonBodies || !b.IsNonBody)
                    .OrderBy(b => b, Comparer<SystemBodyViewModel>.Create((a, b2) => comparer.Compare(a, b2)));

                currentSystemBodies = new ObservableCollection<SystemBodyViewModel>(filtered);
                OnPropertyChanged(nameof(CurrentSystemBodies));
            });
        }

        private bool IgnoreSystemBodiesFilter(SystemBodyViewModel body) => !body.IsNonBody;

        private void Spansh_OnCurrentTargetChanged(object? sender, ExplorationTarget? e) => RefreshSpanshTarget();
        private void Spansh_OnCurrentContainerChanged(object? sender, SpanshCsvContainer? e) => RefreshSpanshTarget();

        private void RefreshSpanshTarget()
        {
            OnPropertyChanged(nameof(HasSpanshRoute));
            OnPropertyChanged(nameof(SpanshTargetSystem));
            OnPropertyChanged(nameof(SpanshTargetBodies));
            OnPropertyChanged(nameof(IsInSpanshTargetSystem));
        }

        public override void Dispose()
        {

            this.mainView.OnCurrentSystemUpdatedEvent -= MainView_OnCurrentSystemUpdatedEvent;
            this.mainView.OnRouteUpdated -= MainView_OnRouteUpdated;
            this.mainView.OnBodyUpdated -= MainView_OnBodyUpdated;
            this.mainView.OnBioUpdated -= MainView_OnBioUpdated;
            this.mainView.OnSelectedBodyUpdated -= MainView_OnSelectedBodyUpdated;

            this.spanshStore.OnCurrentTargetChanged -= Spansh_OnCurrentTargetChanged;
            this.spanshStore.OnCurrentContainerChanged -= Spansh_OnCurrentContainerChanged;

            this.explorationData.OnFSDJump -= ExplorationData_OnFSDJump;
        }

        #region Commands
        public ICommand SwitchView { get; }
        public ICommand CopySystemName { get; }
        public ICommand CycleSortDirection { get; }
        public ICommand SetSortCategory { get; }

        private void OnSwitchView(CartoViewState state)
        {
            // No-op — unified view only
        }

        private void OnCopySystemName(object? _)
        {
            if (CurrentSystem?.Name is { Length: > 0 } name)
            {
                mainView.NotificationStore.CopyToClipBoard(name);
            }
        }

        private void OnCycleSortDirection(object? _)
        {
            var current = settingsStore.SystemGridSetting.SortDirection;
            settingsStore.SystemGridSetting.SortDirection = current == System.ComponentModel.ListSortDirection.Ascending
                ? System.ComponentModel.ListSortDirection.Descending
                : System.ComponentModel.ListSortDirection.Ascending;
            settingsStore.OnSystemGridSettingsUpdated();
            OnPropertyChanged(nameof(SortAscending));
            OnPropertyChanged(nameof(SortDirectionGlyph));
            RefreshBodiesView();
        }

        private void OnSetSortCategory(BodySortCategory category)
        {
            settingsStore.SystemGridSetting.BodySortingOptions = category;
            settingsStore.OnSystemGridSettingsUpdated();
            OnPropertyChanged(nameof(SelectedSortCategory));
            RefreshBodiesView();
        }
        #endregion       

        private void RefreshBodiesView()
        {
            ODExplorer.Models.DispatcherHelper.Invoke(() =>
            {
                if (CurrentSystem == null)
                    return;
                var gridSettings = settingsStore.SystemGridSetting;
                var comparer = new SystemBodyViewModelMainComparer(gridSettings);
                var filtered = CurrentSystem.Bodies
                    .Where(b => !gridSettings.IgnoreNonBodies || !b.IsNonBody)
                    .OrderBy(b => b, Comparer<SystemBodyViewModel>.Create((a, b2) => comparer.Compare(a, b2)));
                currentSystemBodies = new ObservableCollection<SystemBodyViewModel>(filtered);
                OnPropertyChanged(nameof(CurrentSystemBodies));
            });
        }

        private void ExplorationData_OnFSDJump(object? sender, string e)
        {
            HyperSpaceText = $"JUMPING TO {e.ToUpperInvariant()}";
            InHyperSpace = true;
            RefreshBodiesView();
        }

        private void ExplorationData_OnBioDataSold(object? sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(ExoValue));
        }

        private void ExplorationData_OnCartoValueChanged(object? sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(CartoValue));
        }
    }
}
