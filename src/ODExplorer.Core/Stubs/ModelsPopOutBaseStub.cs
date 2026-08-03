namespace ODExplorer.Models
{
    // Lightweight PopOutBase stub used by SettingsStore when UI PopOutBase is not available.
    public class PopOutBase
    {
        public virtual string Title { get; set; } = string.Empty;
        public ODExplorer.ViewModels.ModelVMs.WindowPositionViewModel Position { get; set; } = new();
        public PopOutMode Mode { get; set; } = PopOutMode.Normal;
        public bool AlwaysOnTop { get; set; }
        public bool ShowTitle { get; set; } = true;
        public bool ShowInTaskBar { get; set; } = true;
        public object? AdditionalSettings { get; set; }
        public double ZoomLevel { get; set; } = 1.0;
        public int Count { get; set; } = 1;

        /// <summary>DataContext set by MainViewModel before raising OpenPopoutRequested.</summary>
        public object? DataContext { get; set; }

        /// <summary>Called by MainViewModel to apply saved position/mode params before showing.</summary>
        public virtual void ApplyParams(ODExplorer.Models.PopOutParams p)
        {
            if (p is null) return;
            if (p.Position != null)
            {
                Position.Top = p.Position.Top;
                Position.Left = p.Position.Left;
                Position.Height = p.Position.Height;
                Position.Width = p.Position.Width;
                Position.State = p.Position.State;
            }
            Mode = p.Mode;
            AlwaysOnTop = p.AlwaysOnTop;
            ShowTitle = p.ShowTitle;
            ShowInTaskBar = p.ShowInTaskBar;
            ZoomLevel = p.ZoomLevel;
            Count = p.Count;
        }

        /// <summary>Called by MainViewModel when the journal parser goes offline; UI should close the window.</summary>
        public virtual void ForceClose() { }

        /// <summary>Called after position/mode reset by the user.</summary>
        public virtual void InvokeReset() { }
    }
}
