using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using NUnit.Framework;
using ODExplorer.Controls;
using ODExplorer.Models;
using ODExplorer.UI.Avalonia.Views;
using ODExplorer.ViewModels.ModelVMs;

namespace ODExplorer.UI.Avalonia.Tests;

// Headless smoke tests for the pop-out overlay windows (startup restore):
// content selection, mode/title/topmost application, the parser-offline
// force-close path and the reset-geometry path.
public class PopOutWindowTests
{
    [AvaloniaTest]
    public void Builds_overlay_content_and_applies_popout_mode()
    {
        var vm = TestHarness.CreateMainViewModel();
        var popOut = new SystemBodiesOverlay
        {
            Mode = PopOutMode.Semitransparent,
            ShowTitle = false,
            AlwaysOnTop = true,
        };

        var window = new PopOutWindow(vm, popOut);
        window.Show();

        Assert.That(window.Content, Is.TypeOf<TextBlock>(), "popout hosts a title TextBlock");
        Assert.That(window.Opacity, Is.EqualTo(0.65).Within(0.001), "semitransparent mode maps to 0.65 opacity");
        Assert.That(window.Topmost, Is.True);
        Assert.That(window.Title, Is.Empty, "ShowTitle=false suppresses the title bar text");

        window.Close();
    }

    [AvaloniaTest]
    public async Task ForceClose_closes_the_window_without_persisting_close()
    {
        var vm = TestHarness.CreateMainViewModel();
        var popOut = new ExobiologyOverlay();
        var window = new PopOutWindow(vm, popOut);
        window.Show();
        await WaitUntil(() => window.IsVisible, "pop-out becomes visible after Show()");

        popOut.ForceClose();

        await WaitUntil(() => !window.IsVisible, "force-close hides the pop-out window");
        window.Close();
    }

    [AvaloniaTest]
    public async Task User_close_persists_inactive_params()
    {
        var vm = TestHarness.CreateMainViewModel();
        var popOut = new SystemBodiesOverlay();
        var window = new PopOutWindow(vm, popOut);
        window.Show();
        await WaitUntil(() => window.IsVisible, "pop-out becomes visible after Show()");

        window.Close();

        await WaitUntil(() => !window.IsVisible, "user close hides the pop-out window");

        // GetParams is an "open" API that flips Active=true; read the raw store instead.
        var saved = vm.SettingsStore.GetCommanderPopOutParams(0);
        Assert.That(saved.Count, Is.EqualTo(1));
        Assert.That(saved[0].Active, Is.False, "user close persists the pop-out as inactive for startup restore");
    }

    [AvaloniaTest]
    public async Task InvokeReset_reapplies_saved_geometry()
    {
        var vm = TestHarness.CreateMainViewModel();
        var popOut = new SystemBodiesOverlay
        {
            Position = new WindowPositionViewModel { Left = 100, Top = 120, Width = 700, Height = 520 },
        };

        var window = new PopOutWindow(vm, popOut);
        window.Show();
        await WaitUntil(() => window.IsVisible, "pop-out becomes visible after Show()");
        // Drain the Show-time layout/geometry sync so no queued Resized can
        // clobber the Position instance we mutate below (stale-Resized race).
        await Task.Delay(500);

        popOut.Position = new WindowPositionViewModel { Left = 200, Top = 240, Width = 750, Height = 560 };
        popOut.InvokeReset();

        await WaitUntil(() => Math.Abs(window.Width - 750) < 1, "reset re-applies the saved width");
        Assert.That(window.Height, Is.EqualTo(560).Within(1), "reset re-applies the saved height");

        window.Close();
    }

    private static async Task WaitUntil(Func<bool> condition, string what)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50);
        }
        Assert.That(condition(), Is.True, what);
    }
}
