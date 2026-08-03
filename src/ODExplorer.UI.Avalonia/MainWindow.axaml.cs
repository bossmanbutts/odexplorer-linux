using Avalonia.Controls;
using ODExplorer.ViewModels.ViewVMs;

namespace ODExplorer.UI.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
