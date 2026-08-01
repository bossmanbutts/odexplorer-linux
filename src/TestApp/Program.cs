using ODExplorer.Adapters;
using ODExplorer.Audio;
using ODExplorer.Stores;
using System;

// Minimal test app that wires NoOp adapters and calls SettingsStore.LoadSettings()
class Program
{
    static int Main(string[] args)
    {
        // Configure NoOp adapters
        var odUtils = new NoOpOdUtilsAdapter();
        var notifier = new NoOpNotificationAdapter();
        var paths = new NoOpPlatformPaths();

        // Create a minimal in-memory "database provider" object that provides GetAllSettings/AddSettings methods via dynamic.
        var inMemoryDb = new InMemorySettingsProvider();

        var settingsStore = new ODExplorer.Stores.SettingsStore(inMemoryDb);

        try
        {
            settingsStore.LoadSettings();
            Console.WriteLine("Settings loaded successfully (no-op provider).");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"LoadSettings failed: {ex.Message}");
            return 2;
        }
    }
}

public class InMemorySettingsProvider
{
    public System.Collections.Generic.List<object> GetAllSettings() => new();
    public void AddSettings(System.Collections.Generic.List<object> settings) { }
}
