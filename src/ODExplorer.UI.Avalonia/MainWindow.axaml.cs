using Avalonia.Controls;
using ODExplorer.ViewModels.ViewVMs;

namespace ODExplorer.UI.Avalonia;

public partial class MainWindow : Window
{
    private bool quitRequested;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
        Closing += OnWindowClosing;
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
