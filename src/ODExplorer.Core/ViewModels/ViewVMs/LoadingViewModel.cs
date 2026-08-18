using ODExplorer.Stores;
using ODUtils.APis;
using ODUtils.Commands;
using ODUtils.Dialogs.ViewModels;
using ODUtils.IO;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ODExplorer.ViewModels.ViewVMs
{
    public sealed class LoadingViewModel : OdViewModelBase
    {
        public LoadingViewModel(JournalParserStore journalParseStore, SettingsStore settingsStore, NavigationViewModel navigationView)
        {
            journalStore = journalParseStore;
            this.navigationView = navigationView;
            journalStore.OnJournalStoreStatusChange += JournalStore_OnStatusChange;

            OpenPayPal = new RelayCommand(OnOpenPayPal);
            OpenGitHub = new RelayCommand(OnOpenGitHub);
            DownloadUpdate = new RelayCommand(OnDownloadUpdate);
            if (settingsStore.SelectedCommanderID >= 0)
                journalStore.ReadNewCommander(settingsStore.SelectedCommanderID);

            _ = CheckForUpdatesAsync();
        }

        private readonly JournalParserStore journalStore;
        private readonly NavigationViewModel navigationView;

        public ICommand OpenPayPal { get; }
        public ICommand OpenGitHub { get; }
        public ICommand DownloadUpdate { get; }

        private string statusText = string.Empty;
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private string updateText = string.Empty;
        public string UpdateText
        {
            get => updateText;
            set
            {
                updateText = value;
                OnPropertyChanged(nameof(UpdateText));
                OnPropertyChanged(nameof(HasUpdate));
            }
        }

        private string? updateDownloadUrl;
        public bool HasUpdate => !string.IsNullOrEmpty(updateText);

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                var updateInfo = await Json.GetJsonFromUrlAndDeserialise<UpdateInfo>(
                    "https://raw.githubusercontent.com",
                    "/WarmedxMints/ODUpdates/main/ODExplorerUpdate.json");

                if (updateInfo?.Version is not null && updateInfo.Version > App.AppVersion)
                {
                    UpdateText = $"Update available: v{updateInfo.Version}";
                    updateDownloadUrl = updateInfo.DownloadUrl;
                }
            }
            catch
            {
                // Network or parse failure; silently continue.
            }
        }

        private void OnDownloadUpdate(object? obj)
        {
            if (!string.IsNullOrEmpty(updateDownloadUrl))
                ODUtils.Helpers.OperatingSystem.OpenUrl(updateDownloadUrl);
        }

        private void OnOpenPayPal(object? obj)
        {
            ODUtils.Helpers.OperatingSystem.OpenUrl("https://www.paypal.com/donate/?business=UPEJS3PN7H4XJ&no_recurring=0&item_name=Creator+of+OD+Software.+Thank+you+for+your+donation.&currency_code=GBP");
        }

        private void OnOpenGitHub(object? obj)
        {
            ODUtils.Helpers.OperatingSystem.OpenUrl("https://github.com/WarmedxMints/OD-Explorer");
        }

        public override void Dispose()
        {
            journalStore.OnJournalStoreStatusChange -= JournalStore_OnStatusChange;
        }

        private void JournalStore_OnStatusChange(object? sender, string? e)
        {
            if (string.Equals(e, "No Commanders Found"))
            {
                StatusText = "No Commanders Found\nPlease Select a Directory to scan\n\nOpening Settings Panel...";
                _ = Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(5000);
                    navigationView.SettingsViewCommand.Execute(null);
                });
                return;
            }

            StatusText = e ?? string.Empty;
        }
    }
}
