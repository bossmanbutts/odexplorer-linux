using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ODExplorer.Database;
using ODExplorer.Stores;
using ODExplorer.UI.Avalonia.Services;
using ODExplorer.ViewModels.ViewVMs;
using ODUtils.APis;
using ODUtils.Database.Interfaces;
using ODUtils.Exobiology;
using ODUtils.ViewModelNavigation;

namespace ODExplorer.UI.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Wire platform adapters for core
        try
        {
            // Dispatcher
            ODExplorer.Models.DispatcherHelper.Current = new DispatcherAdapter();
        }
        catch { }

        try
        {
            // Audio provider
            ODExplorer.Audio.IAudioPlayerProvider.Current = new AudioPlayer();
        }
        catch { }

        try
        {
            // OdUtils adapter used for clipboard / open-url helpers
            var odUtils = new OdUtilsAdapter();
            ODExplorer.Adapters.OdUtilsAdapterProvider.Current = odUtils;

            // Wire the interactive MessageBox dialog to MessageBoxRequester.Requested.
            ODExplorer.Models.MessageBoxRequester.Requested += (s, args) =>
            {
                try
                {
                    var owner = ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime ? lifetime.MainWindow : null;
                    MessageBoxService.Show(owner, args);
                }
                catch { }
            };
        }
        catch { }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = CreateMainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnTrayShow(object? sender, System.EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return;
        }

        window.Show();
        window.WindowState = global::Avalonia.Controls.WindowState.Normal;
        window.Activate();
    }

    private void OnTrayQuit(object? sender, System.EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: MainWindow window })
        {
            window.RequestQuit();
        }
        else if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private static MainWindow CreateMainWindow()
    {
        var mainViewModel = BuildViewModelGraph();
        return new MainWindow(mainViewModel);
    }

    private static MainViewModel BuildViewModelGraph()
    {
        // Stores (stub implementations until real stores are wired)
        IOdToolsDatabaseProvider databaseProvider = new OdExplorerDatabaseProvider();
        var settingsStore = new SettingsStore(databaseProvider);
        var notificationStore = new NotificationStore(settingsStore);
        var journalParserStore = new JournalParserStore(databaseProvider, settingsStore);
        var explorationDataStore = new ExplorationDataStore();
        var organicCheckListDataStore = new OrganicCheckListDataStore(journalParserStore, new ExoData(), settingsStore);
        var spanshCsvStore = new SpanshCsvStore();

        var navigationStore = new OdNavigationStore();
        MainViewModel? mainViewModel = null;
        NavigationViewModel? navigationViewModel = null;

        // Navigation services (factories are lazy so view models are created on first navigation)
        var loadingService = new OdNavigationService<LoadingViewModel>(navigationStore, () =>
            new LoadingViewModel(journalParserStore, settingsStore, navigationViewModel!));

        var cartoService = new OdNavigationService<CartographicViewModel>(navigationStore, () =>
            new CartographicViewModel(explorationDataStore, journalParserStore, settingsStore, mainViewModel!));

        var organicService = new OdNavigationService<OrganicViewModel>(navigationStore, () =>
            new OrganicViewModel(organicCheckListDataStore, settingsStore, journalParserStore, explorationDataStore, new ExoData(), notificationStore));

        var settingsService = new OdNavigationService<SettingsViewModel>(navigationStore, () =>
            SettingsViewModel.CreateViewModel(settingsStore, databaseProvider, navigationViewModel!, journalParserStore));

        var displaySettingsService = new OdNavigationService<DisplaySettingsViewModel>(navigationStore, () =>
            new DisplaySettingsViewModel(settingsStore, notificationStore));

        var cartoDetailsService = new OdNavigationService<CartoDetailsViewModel>(navigationStore, () =>
            new CartoDetailsViewModel(explorationDataStore, settingsStore, new EdsmApiService(), databaseProvider, journalParserStore, notificationStore));

        var spanshService = new OdNavigationService<SpanshViewModel>(navigationStore, () =>
            new SpanshViewModel(spanshCsvStore, settingsStore, notificationStore));

        var edAstroService = new OdNavigationService<EdAstroViewModel>(navigationStore, () =>
            new EdAstroViewModel(explorationDataStore, notificationStore));

        navigationViewModel = new(loadingService, cartoService, organicService, settingsService,
            displaySettingsService, cartoDetailsService, spanshService, edAstroService);

        mainViewModel = new MainViewModel(navigationStore, navigationViewModel!, settingsStore, explorationDataStore,
            journalParserStore, organicCheckListDataStore, notificationStore, spanshCsvStore, databaseProvider);

        // Start the shell on the Loading view; real store events drive further navigation.
        navigationViewModel.LoadingViewCommand.Execute(null);

        return mainViewModel;
    }
}
