using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(ODExplorer.UI.Avalonia.Tests.TestAppBuilder))]

namespace ODExplorer.UI.Avalonia.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .WithInterFont();
}
