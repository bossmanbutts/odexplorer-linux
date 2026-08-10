using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using NUnit.Framework;
using ODExplorer.Models;
using ODExplorer.Stores;
using ODExplorer.UI.Avalonia.Controls;

namespace ODExplorer.UI.Avalonia.Tests;

// Regression coverage for the "three toasts then the app hangs" freeze: Show()
// used to evict the oldest toast asynchronously (300 ms deferred removal), so
// the `while (active.Count >= MaxNotificationCount) RemoveOldest();` loop never
// shrank `active` and spun forever once the host was full.
public class ToastHostTests
{
    private static ToastMessage Msg(int i) => new("Title " + i, "Message " + i);

    private static int LiveCards(ToastHost host)
        => host.Children.OfType<Canvas>().Single().Children.Count;

    [AvaloniaTest]
    public void Show_evicts_synchronously_when_full_and_does_not_hang()
    {
        var settings = new SettingsStore();
        settings.NotificationSettings.MaxNotificationCount = 3;

        var host = new ToastHost(settings);

        // Regression: with async eviction this loop spins forever (test timeout);
        // with the fix it terminates and caps the active cards at the max count.
        for (int i = 0; i < 50; i++)
        {
            host.Show(Msg(i));
        }

        Assert.That(LiveCards(host), Is.EqualTo(3), "older toasts are removed synchronously so the eviction loop terminates");
    }

    [AvaloniaTest]
    public void Show_with_single_slot_keeps_most_recent()
    {
        var settings = new SettingsStore();
        settings.NotificationSettings.MaxNotificationCount = 1;

        var host = new ToastHost(settings);
        host.Show(Msg(1));
        host.Show(Msg(2));

        Assert.That(LiveCards(host), Is.EqualTo(1));
    }

    [AvaloniaTest]
    public void Show_suppressed_when_notifications_disabled()
    {
        var settings = new SettingsStore();
        settings.NotificationSettings.NotificationsEnabled = false;

        var host = new ToastHost(settings);
        host.Show(Msg(1));

        Assert.That(LiveCards(host), Is.EqualTo(0));
    }

    [AvaloniaTest]
    public async Task Show_evicts_card_after_display_time()
    {
        var settings = new SettingsStore();
        settings.NotificationSettings.DisplayTime = 1;

        var host = new ToastHost(settings);
        host.Show(Msg(1));
        Assert.That(LiveCards(host), Is.EqualTo(1), "card is shown immediately");

        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (LiveCards(host) > 0 && DateTime.UtcNow < deadline)
        {
            await Task.Delay(100);
        }

        Assert.That(LiveCards(host), Is.EqualTo(0), "expired card is removed after the display time elapses");
    }

    [AvaloniaTest]
    public void Clear_removes_all_cards_immediately()
    {
        var settings = new SettingsStore();
        settings.NotificationSettings.MaxNotificationCount = 5;

        var host = new ToastHost(settings);
        for (int i = 0; i < 5; i++) host.Show(Msg(i));

        host.Clear();

        Assert.That(LiveCards(host), Is.EqualTo(0));
    }
}
