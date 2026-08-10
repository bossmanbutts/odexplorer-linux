using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NUnit.Framework;
using ODExplorer.UI.Avalonia.Controls;
using ODExplorer.UI.Avalonia.Views;
using ODExplorer.ViewModels.ViewVMs;

namespace ODExplorer.UI.Avalonia.Tests;

// Headless shell smoke: the MainWindow opens with the real ViewModel graph and
// its toast host reacts to NotificationStore events (the wiring that the
// original freeze bug was reachable through).
public class ShellTests
{
    [AvaloniaTest]
    public void MainWindow_opens_with_loading_view_and_wired_toast_host()
    {
        var vm = TestHarness.CreateMainViewModel();
        vm.SettingsStore.OnBoardingComplete = true; // skip the onboarding modal in tests

        var window = new MainWindow(vm);
        window.Show();
        Dispatcher.UIThread.RunJobs();

        Assert.That(window.DataContext, Is.SameAs(vm));
        Assert.That(window.GetVisualDescendants().OfType<LoadingView>().Any(), Is.True,
            "shell starts on the Loading view via the ViewLocator template");

        var toastHost = window.GetVisualDescendants().OfType<ToastHost>().Single();
        var liveCards = () => toastHost.Children.OfType<Canvas>().Single().Children.Count;
        Assert.That(liveCards(), Is.EqualTo(0));

        vm.NotificationStore.ShowTestNotification();
        Dispatcher.UIThread.RunJobs();

        Assert.That(liveCards(), Is.EqualTo(1), "a toast raised through the store appears in the window's toast host");

        window.Close();
        Dispatcher.UIThread.RunJobs();
    }
}
