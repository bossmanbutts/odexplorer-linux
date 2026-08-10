using System.Text.Json.Serialization;
using ODExplorer.Controls;
using ODExplorer.ViewModels.ModelVMs;

namespace ODExplorer.Models
{
    public sealed class PopOutParams
    {
        public string Title { get; set; } = string.Empty;
        public int Count { get; set; }
        public WindowPositionViewModel Position { get; set; } = new();
        public PopOutMode Mode { get; set; }
        public bool AlwaysOnTop { get; set; }
        public bool ShowTitle { get; set; } = true;
        public bool ShowInTaskBar { get; set; } = true;
        public bool Active { get; set; }
        [JsonIgnore]
        public object? AdditionalSettings { get; set; }
        public double ZoomLevel { get; set; } = 1d;

        public static PopOutParams CreateParams(object popOut, int count, bool active)
        {
            // Expect popOut to implement ODExplorer.Controls.IPopOutBase in the UI; use dynamic to avoid compile-time dependency.
            dynamic d = popOut;
            var pos = (ODExplorer.ViewModels.ModelVMs.WindowPositionViewModel?)d.Position ?? new();
            return new()
            {
                Title = d.Title ?? string.Empty,
                Count = count,
                Position = pos.Clone(),
                Mode = (ODExplorer.Models.PopOutMode)(d.Mode ?? PopOutMode.Normal),
                AlwaysOnTop = d.AlwaysOnTop,
                ShowTitle = d.ShowTitle,
                ShowInTaskBar = d.ShowInTaskBar,
                Active = active,
                AdditionalSettings = d.AdditionalSettings,
                ZoomLevel = d.ZoomLevel,
            };
        }

        public void UpdateParams(object popOut, bool active)
        {
            dynamic d = popOut;
            Position = ((ODExplorer.ViewModels.ModelVMs.WindowPositionViewModel?)d.Position ?? new()).Clone();
            Mode = (ODExplorer.Models.PopOutMode)(d.Mode ?? PopOutMode.Normal);
            AlwaysOnTop = d.AlwaysOnTop;
            ShowTitle = d.ShowTitle;
            ShowInTaskBar = d.ShowInTaskBar;
            Active = active;
            AdditionalSettings = d.AdditionalSettings;
            ZoomLevel = d.ZoomLevel;
        }
    }
}
