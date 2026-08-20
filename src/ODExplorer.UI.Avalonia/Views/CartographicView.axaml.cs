using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using ODExplorer.Models;
using ODExplorer.ViewModels.ModelVMs;
using ODExplorer.ViewModels.ViewVMs;

namespace ODExplorer.UI.Avalonia.Views;

public partial class CartographicView : UserControl
{
    public CartographicView()
    {
        InitializeComponent();
    }

    private void Body_Tapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border
            && border.DataContext is SystemBodyViewModel body
            && DataContext is CartographicViewModel vm)
        {
            vm.SelectedBody = body;
        }
    }

    private void StarHeader_Tapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border
            && border.DataContext is StarBodyGroup group)
        {
            group.ToggleExpanded();
        }
    }
}
