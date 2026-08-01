namespace ODExplorer.Controls
{
    // Minimal pop-out base interface used by core when persisting pop-out parameters.
    // UI should provide a concrete PopOutBase that matches these properties.
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

    // Provide an empty PopOutBase type to maintain compatibility; UI should implement IPopOutBase.
    public class PopOutBase : IPopOutBase
    {
        public string Title => string.Empty;
        public ODExplorer.ViewModels.ModelVMs.WindowPositionViewModel Position => new();
        public ODExplorer.Models.PopOutMode Mode => ODExplorer.Models.PopOutMode.Normal;
        public bool AlwaysOnTop => false;
        public bool ShowTitle => true;
        public bool ShowInTaskBar => true;
        public object? AdditionalSettings => null;
        public double ZoomLevel => 1.0;
        public int Count => 1;
    }
}
