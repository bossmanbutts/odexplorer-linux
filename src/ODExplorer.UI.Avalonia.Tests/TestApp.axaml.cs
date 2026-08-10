using Avalonia;
using Avalonia.Markup.Xaml;

namespace ODExplorer.UI.Avalonia.Tests;

// Headless test Application: mirrors the real app's adapter wiring without the
// database/network composition root so tests stay fast and deterministic.
public class TestApp : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            ODExplorer.Models.DispatcherHelper.Current = new ODExplorer.UI.Avalonia.Services.DispatcherAdapter();
        }
        catch { }

        base.OnFrameworkInitializationCompleted();
    }
}
