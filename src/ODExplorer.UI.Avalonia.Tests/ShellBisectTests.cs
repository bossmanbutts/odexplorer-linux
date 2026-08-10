using System.Linq;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using NUnit.Framework;
using ODExplorer.UI.Avalonia.Views;

namespace ODExplorer.UI.Avalonia.Tests;

public class ShellBisectTests
{
    [AvaloniaTest]
    public void BareWindow_with_main_vm_datacontext()
    {
        var vm = TestHarness.CreateMainViewModel();
        vm.SettingsStore.OnBoardingComplete = true;

        var window = new global::Avalonia.Controls.Window { DataContext = vm };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        TestContext.Progress.WriteLine("LoadingView present: " +
            window.GetVisualDescendants().OfType<LoadingView>().Any());

        window.Close();
        Dispatcher.UIThread.RunJobs();
    }
}
