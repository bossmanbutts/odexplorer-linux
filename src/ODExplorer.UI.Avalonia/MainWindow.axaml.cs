using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using ODExplorer.Models;
using ODExplorer.UI.Avalonia.Controls;
using ODExplorer.ViewModels.ViewVMs;

namespace ODExplorer.UI.Avalonia;

public partial class MainWindow : Window
{
    private readonly ToastHost toastHost;
    private bool quitRequested;

    public MainWindow()
    {
        InitializeComponent();
        toastHost = new ToastHost(null!);
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;

        toastHost = new ToastHost(viewModel.SettingsStore)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsHitTestVisible = false,
        };
        ContentRoot.Children.Add(toastHost);

        viewModel.NotificationStore.OnToast += OnToast;
        Closing += OnWindowClosing;
    }

    private void OnToast(ToastMessage message)
    {
        Dispatcher.UIThread.Post(() => toastHost.Show(message));
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (quitRequested)
        {
            return;
        }

        // With "minimise to tray" enabled, the close button hides the window
        // instead of terminating the app; use the tray "Quit" item to exit.
        if (DataContext is MainViewModel { SettingsStore.MinimiseToTray: true })
        {
            e.Cancel = true;
            Hide();
        }
    }

    public void RequestQuit()
    {
        quitRequested = true;
        Close();
    }
}
