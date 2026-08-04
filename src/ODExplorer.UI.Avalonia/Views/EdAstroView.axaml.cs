using Avalonia.Controls;
using Avalonia.Interactivity;
using ODExplorer.ViewModels.ModelVMs;
using ODExplorer.ViewModels.ViewVMs;

namespace ODExplorer.UI.Avalonia.Views;

public partial class EdAstroView : UserControl
{
    public EdAstroView()
    {
        InitializeComponent();
    }

    private void CopyPoi_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not EdAstroViewModel vm
            || sender is not Button { DataContext: EdAstroPoiViewModel poi })
        {
            return;
        }

        vm.CopyToClipboard(poi);
    }

    private void OpenPoi_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not EdAstroViewModel vm || vm.SelectedPoi is null)
        {
            return;
        }

        ODUtils.Helpers.OperatingSystem.OpenUrl(vm.SelectedPoi.PioUrl.ToString());
    }

    private void OpenHome_Click(object? sender, RoutedEventArgs e)
    {
        ODUtils.Helpers.OperatingSystem.OpenUrl("https://edastro.com/");
    }
}
