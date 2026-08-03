using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

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
            ODExplorer.Models.DispatcherHelper.Current = new ODExplorer.UI.Avalonia.Services.DispatcherAdapter();
        }
        catch { }

        try
        {
            // Audio provider
            ODExplorer.Audio.IAudioPlayerProvider.Current = new ODExplorer.UI.Avalonia.Services.AudioPlayer();
        }
        catch { }

        try
        {
            // Other adapters
            var odUtils = new ODExplorer.UI.Avalonia.Services.OdUtilsAdapter();
            var notifier = new ODExplorer.UI.Avalonia.Services.NotificationAdapter();
            var paths = new ODExplorer.UI.Avalonia.Services.PlatformPaths();
            // Wire static providers used by core
            ODExplorer.Adapters.OdUtilsAdapterProvider.Current = odUtils;

            // Wire a simple non-blocking MessageBox handler: show a toast for now. UI should replace with proper dialog.
            ODExplorer.Models.MessageBoxRequester.Requested += (s, args) =>
            {
                try
                {
                    notifier.ShowToast(new ODExplorer.Adapters.NotificationModel { Title = args.Title, Message = args.Message });
                }
                catch { }
            };

            // If your app has a DI container, register these implementations there.
        }
        catch { }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}