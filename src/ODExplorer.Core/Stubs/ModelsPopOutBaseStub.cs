namespace ODExplorer.Models
{
    // Lightweight PopOutBase stub used by SettingsStore when UI PopOutBase is not available.
    public class PopOutBase
    {
        public string Title { get; set; } = string.Empty;
        public ODExplorer.ViewModels.ModelVMs.WindowPositionViewModel Position { get; set; } = new();
        public PopOutMode Mode { get; set; } = PopOutMode.Normal;
        public bool AlwaysOnTop { get; set; }
        public bool ShowTitle { get; set; } = true;
        public bool ShowInTaskBar { get; set; } = true;
        public object? AdditionalSettings { get; set; }
        public double ZoomLevel { get; set; } = 1.0;
        public int Count { get; set; } = 1;
    }
}
