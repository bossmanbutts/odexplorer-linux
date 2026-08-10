using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using ODExplorer.Database;
using ODExplorer.Stores;
using ODExplorer.ViewModels.ViewVMs;
using ODUtils.APis;
using ODUtils.Database.Interfaces;
using ODUtils.Exobiology;
using ODUtils.ViewModelNavigation;

namespace ODExplorer.UI.Avalonia.Tests;

// Builds a MainViewModel over a throwaway SQLite DB (same graph shape as
// App.BuildViewModelGraph) but with an offline EDSM service and no EdAstro
// network refresh, so UI tests run without touching the network.
internal static class TestHarness
{
    public static MainViewModel CreateMainViewModel()
    {
        ODExplorer.Models.DispatcherHelper.Current ??= new ODExplorer.UI.Avalonia.Services.DispatcherAdapter();

        // Unique per-invocation DB so tests can run concurrently without
        // clashing over one shared SQLite file.
        string dbFile = Path.Combine(Path.GetTempPath(), "odex_ui_tests_" + Guid.NewGuid().ToString("N") + ".db");
        try
        {
            if (File.Exists(dbFile)) File.Delete(dbFile);
        }
        catch { }

        var dbContextFactory = new OdExplorerDbContextFactory($"Data Source={dbFile}");
        using (var migrationContext = dbContextFactory.CreateDbContext())
        {
            migrationContext.Database.Migrate();
        }

        IOdToolsDatabaseProvider databaseProvider = new OdExplorerDatabaseProvider(dbContextFactory);
        var settingsStore = new SettingsStore(databaseProvider);
        var notificationStore = new NotificationStore(settingsStore);
        var exoData = new ExoData();
        var journalParserStore = new JournalParserStore(databaseProvider, settingsStore);
        var organicCheckListDataStore = new OrganicCheckListDataStore(journalParserStore, exoData, settingsStore, registerWithParser: false);
        var explorationDataStore = new ExplorationDataStore(journalParserStore, new OfflineEdsmApiService(), databaseProvider,
            notificationStore, settingsStore, exoData, organicCheckListDataStore);
        journalParserStore.RegisterParser(organicCheckListDataStore);
        var spanshCsvStore = new SpanshCsvStore(journalParserStore, databaseProvider, settingsStore, notificationStore);

        var navigationStore = new OdNavigationStore();
        MainViewModel? mainViewModel = null;
        NavigationViewModel? navigationViewModel = null;

        var loadingService = new OdNavigationService<LoadingViewModel>(navigationStore, () =>
            new LoadingViewModel(journalParserStore, settingsStore, navigationViewModel!));
        var cartoService = new OdNavigationService<CartographicViewModel>(navigationStore, () =>
            new CartographicViewModel(explorationDataStore, journalParserStore, settingsStore, mainViewModel!));
        var organicService = new OdNavigationService<OrganicViewModel>(navigationStore, () =>
            new OrganicViewModel(organicCheckListDataStore, settingsStore, journalParserStore, explorationDataStore, new ExoData(), notificationStore));
        var settingsService = new OdNavigationService<SettingsViewModel>(navigationStore, () =>
            SettingsViewModel.CreateViewModel(settingsStore, databaseProvider, navigationViewModel!, journalParserStore, notificationStore));
        var displaySettingsService = new OdNavigationService<DisplaySettingsViewModel>(navigationStore, () =>
            new DisplaySettingsViewModel(settingsStore, notificationStore));
        var cartoDetailsService = new OdNavigationService<CartoDetailsViewModel>(navigationStore, () =>
            new CartoDetailsViewModel(explorationDataStore, settingsStore, new OfflineEdsmApiService(), databaseProvider, journalParserStore, notificationStore));
        var spanshService = new OdNavigationService<SpanshViewModel>(navigationStore, () =>
            new SpanshViewModel(spanshCsvStore, settingsStore, notificationStore));
        var edAstroService = new OdNavigationService<EdAstroViewModel>(navigationStore, () =>
            new EdAstroViewModel(explorationDataStore, notificationStore));

        navigationViewModel = new(loadingService, cartoService, organicService, settingsService,
            displaySettingsService, cartoDetailsService, spanshService, edAstroService);

        mainViewModel = new MainViewModel(navigationStore, navigationViewModel!, settingsStore, explorationDataStore,
            journalParserStore, organicCheckListDataStore, notificationStore, spanshCsvStore, databaseProvider);

        navigationViewModel.LoadingViewCommand.Execute(null);
        return mainViewModel;
    }

    // Offline EDSM so no HTTP calls are made during UI tests.
    private sealed class OfflineEdsmApiService : EdsmApiService
    {
        public override System.Threading.Tasks.Task<ODUtils.Models.StarType> GetPrimaryStarClassAsync(string systemName)
            => System.Threading.Tasks.Task.FromResult(ODUtils.Models.StarType.Unknown);

        public override System.Threading.Tasks.Task<EdsmSystemValue?> GetSystemValueAsync(string systemName)
            => System.Threading.Tasks.Task.FromResult<EdsmSystemValue?>(null);

        public override System.Threading.Tasks.Task<(int Count, int Scanned)> GetBodyCountAsync(long systemAddress)
            => System.Threading.Tasks.Task.FromResult((0, 0));
    }
}
