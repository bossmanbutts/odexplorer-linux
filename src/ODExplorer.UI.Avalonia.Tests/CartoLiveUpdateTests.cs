using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using NUnit.Framework;
using ODExplorer.ViewModels.ViewVMs;
using ODUtils.Models;

namespace ODExplorer.UI.Avalonia.Tests;

// Regression: the explorer (Carto) tab must refresh its current-system header
// and body data live when a new FSDJump is picked up while the parser is live.
public class CartoLiveUpdateTests
{
    private static string[] StartupLines(string cmdr, string system, long address) =>
    [
        "{\"timestamp\":\"2024-01-01T00:00:00Z\",\"event\":\"Fileheader\",\"part\":1,\"language\":\"English\"}",
        "{\"timestamp\":\"2024-01-01T00:00:01Z\",\"event\":\"LoadGame\",\"Commander\":\"" + cmdr + "\",\"Ship\":\"CobraMkIII\",\"GameMode\":\"Solo\",\"Credits\":1000000}",
        "{\"timestamp\":\"2024-01-01T00:00:02Z\",\"event\":\"FSDJump\",\"StarSystem\":\"" + system + "\",\"SystemAddress\":" + address + ",\"StarPos\":[30.0,-40.0,5.0],\"StarType\":\"K\",\"StarClass\":\"K\",\"Body\":7,\"Bodies\":4,\"JumpDist\":8.5}"
    ];

    [AvaloniaTest]
    public void Carto_current_system_updates_on_live_fsd_jump()
    {
        var mainView = TestHarness.CreateMainViewModel();
        mainView.SettingsStore.OnBoardingComplete = true;

        var dir = Path.Combine(Path.GetTempPath(), "odex_carto_live_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var journal = Path.Combine(dir, "Journal.240101000000.01.log");
        File.WriteAllLines(journal, StartupLines("CartoCMDR", "FirstSys", 10477373803));

        mainView.OnboardingFinishedWithDirectory(dir);

        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (mainView.UiEnabled == false && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(50);
            Dispatcher.UIThread.RunJobs();
        }

        var carto = mainView.CurrentViewModel as CartographicViewModel;
        Assert.That(carto, Is.Not.Null, "parser-live navigation should land on the Carto view");
        Assert.That(carto.CurrentSystem?.Name, Is.EqualTo("FIRSTSYS"));
        Assert.That(carto.CurrentSystem?.StarClass, Is.EqualTo("K"));

        // Live tail: jump to a brand-new system while IsLive.
        File.AppendAllText(journal,
            "{\"timestamp\":\"2024-01-01T00:00:03Z\",\"event\":\"FSDJump\",\"StarSystem\":\"SecondSys\",\"SystemAddress\":3103895106050,\"StarPos\":[10.0,20.0,30.0],\"StarType\":\"G\",\"StarClass\":\"G\",\"Body\":7,\"Bodies\":1,\"JumpDist\":40.0}\n");

        deadline = DateTime.UtcNow.AddSeconds(15);
        while (carto.CurrentSystem?.Name != "SECONDSYS" && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(50);
            Dispatcher.UIThread.RunJobs();
        }

        Assert.That(carto.CurrentSystem?.Name, Is.EqualTo("SECONDSYS"),
            "Carto CurrentSystem must track the live FSDJump target");
        Assert.That(carto.CurrentSystem?.StarClass, Is.EqualTo("G"));
        Assert.That(carto.CurrentSystem?.Address, Is.EqualTo(3103895106050));
    }

    // A live FSS discovery scan + body scan in the current system must update the
    // header counts and the explorer tab's body list, not just the carto value.
    [AvaloniaTest]
    public void Carto_header_and_bodies_update_on_live_fss_and_scan()
    {
        var mainView = TestHarness.CreateMainViewModel();
        mainView.SettingsStore.OnBoardingComplete = true;

        var dir = Path.Combine(Path.GetTempPath(), "odex_carto_fss_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var journal = Path.Combine(dir, "Journal.240201000000.01.log");
        File.WriteAllLines(journal, StartupLines("FssCMDR", "FssSys", 6000000000001));

        mainView.OnboardingFinishedWithDirectory(dir);

        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (mainView.UiEnabled == false && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(50);
            Dispatcher.UIThread.RunJobs();
        }

        var carto = mainView.CurrentViewModel as CartographicViewModel;
        Assert.That(carto, Is.Not.Null);
        Assert.That(carto.CurrentSystem?.Name, Is.EqualTo("FSSSYS"));

        // Live: honk the system. This rebuilds the current-system VM, so the
        // PropertyChanged probe below attaches after the honk has settled.
        File.AppendAllText(journal,
            "{\"timestamp\":\"2024-02-01T00:00:03Z\",\"event\":\"FSSDiscoveryScan\",\"SystemName\":\"FssSys\",\"SystemAddress\":6000000000001,\"BodyCount\":5,\"Progress\":0.5}\n");

        var honkDeadline = DateTime.UtcNow.AddSeconds(15);
        while (carto.CurrentSystem?.DiscoveredBodiesCount != 5 && DateTime.UtcNow < honkDeadline)
        {
            Thread.Sleep(50);
            Dispatcher.UIThread.RunJobs();
        }

        var raised = new System.Collections.Generic.HashSet<string>();
        carto.CurrentSystem.PropertyChanged += (_, e) => raised.Add(e.PropertyName ?? "");

        // Live: FSS-signal a bio body and detail-scan it.
        File.AppendAllText(journal,
            "{\"timestamp\":\"2024-02-01T00:00:04Z\",\"event\":\"FSSBodySignals\",\"SystemAddress\":6000000000001,\"BodyID\":1,\"Signals\":[{\"Type\":\"$SAA_SignalType_Biological;\",\"Type_Localised\":\"Biological\",\"Count\":2}]}\n" +
            "{\"timestamp\":\"2024-02-01T00:00:05Z\",\"event\":\"Scan\",\"ScanType\":\"Detailed\",\"BodyName\":\"FssSys 1\",\"BodyID\":1,\"StarSystem\":\"FssSys\",\"SystemAddress\":6000000000001,\"DistanceFromArrivalLS\":1800.0,\"PlanetClass\":\"Rocky body\",\"Landable\":true,\"SurfaceTemperature\":280.0,\"MassEM\":0.05,\"Radius\":800000.0,\"SurfaceGravity\":1.1,\"OrbitalPeriod\":100000.0,\"WasDiscovered\":false,\"WasMapped\":false,\"Signals\":[{\"Type\":\"$SAA_SignalType_Biological;\",\"Type_Localised\":\"Biological\",\"Count\":2}]}\n");

        deadline = DateTime.UtcNow.AddSeconds(15);
        while (mainView.OrganicSignals.Any(x => x.BodyID == 1) == false
               && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(50);
            Dispatcher.UIThread.RunJobs();
        }

        TestContext.Out.WriteLine("OrganicSignals: "
            + string.Join(", ", mainView.OrganicSignals.Select(x => $"{x.Name}(bio={x.BiologicalSignals})")));
        TestContext.Out.WriteLine("CurrentSystemBodies: "
            + string.Join(", ", (carto.CurrentSystemBodies ?? []).Select(x => $"{x.Name}(bio={x.BiologicalSignals})")));

        Assert.That(carto.CurrentSystem?.DiscoveredBodiesCount, Is.EqualTo(5),
            "FSS discovery scan must update the discovered-body count live");
        Assert.That(carto.CurrentSystemBodies?.Any(x => x.BodyID == 1), Is.True,
            "a live body scan must appear in the explorer tab body list");
        Assert.That(carto.OrganicSignals.Any(x => x.BodyID == 1), Is.True,
            "a live bio-signal body must appear in the organic signals list");
        Assert.That(carto.CurrentSystem?.BodyCount, Is.GreaterThanOrEqualTo(1),
            "the header body count must tick up as bodies are scanned live");
        Assert.That(raised, Does.Contain("BodyCount"),
            "BodyCount must raise PropertyChanged on a live body scan so the header binding refreshes");
    }

    // Regression: CheckIfSystemKnown must merge StarType from a new FSDJump
    // when the system was first added via Location with StarType.Unknown.
    [AvaloniaTest]
    public void Carto_startype_merges_from_fsijump_over_location_unknown()
    {
        var mainView = TestHarness.CreateMainViewModel();
        mainView.SettingsStore.OnBoardingComplete = true;

        var dir = Path.Combine(Path.GetTempPath(), "odex_carto_st_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var journal = Path.Combine(dir, "Journal.240301000000.01.log");

        // First: a Location event (which passes StarType.Unknown), then a FSDJump
        // to the same system which should carry the real StarType.
        File.WriteAllLines(journal,
        [
            "{\"timestamp\":\"2024-03-01T00:00:00Z\",\"event\":\"Fileheader\",\"part\":1,\"language\":\"English\"}",
            "{\"timestamp\":\"2024-03-01T00:00:01Z\",\"event\":\"LoadGame\",\"Commander\":\"StarTypeCMDR\",\"Ship\":\"CobraMkIII\",\"GameMode\":\"Solo\",\"Credits\":1000000}",
            "{\"timestamp\":\"2024-03-01T00:00:02Z\",\"event\":\"Location\",\"StarSystem\":\"Alpha Centauri\",\"SystemAddress\":123456789,\"StarPos\":[3.03125,3.15625,3.15625]}",
            "{\"timestamp\":\"2024-03-01T00:00:03Z\",\"event\":\"FSDJump\",\"StarSystem\":\"Alpha Centauri\",\"SystemAddress\":123456789,\"StarPos\":[3.03125,3.15625,3.15625],\"StarType\":\"A\",\"StarClass\":\"A\",\"Body\":7,\"Bodies\":4,\"JumpDist\":1.0}"
        ]);

        mainView.OnboardingFinishedWithDirectory(dir);

        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (mainView.UiEnabled == false && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(50);
            Dispatcher.UIThread.RunJobs();
        }

        var carto = mainView.CurrentViewModel as CartographicViewModel;
        Assert.That(carto, Is.Not.Null, "parser-live navigation should land on the Carto view");
        Assert.That(carto.CurrentSystem?.Name, Is.EqualTo("ALPHA CENTAURI"));
        Assert.That(carto.CurrentSystem?.StarClass, Is.EqualTo("A"),
            "StarType from FSDJump must be preserved over Location's Unknown when CheckIfSystemKnown merges");
    }
}
