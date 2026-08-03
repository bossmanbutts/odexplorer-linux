namespace ODExplorer.Controls
{
    // Minimal pop-out base interface used by core when persisting pop-out parameters.
    // UI should provide a concrete implementation inheriting ODExplorer.Models.PopOutBase.
    public interface IPopOutBase
    {
        string Title { get; }
        ODExplorer.ViewModels.ModelVMs.WindowPositionViewModel Position { get; }
        ODExplorer.Models.PopOutMode Mode { get; }
        bool AlwaysOnTop { get; }
        bool ShowTitle { get; }
        bool ShowInTaskBar { get; }
        object? AdditionalSettings { get; }
        double ZoomLevel { get; }
        int Count { get; }
    }
}
