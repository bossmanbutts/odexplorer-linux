namespace ODExplorer.ViewModels.ModelVMs
{
    // Minimal stub used by core while the UI provides a richer viewmodel.
    public sealed class WindowPositionViewModel
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public ODExplorer.Models.WindowState State { get; set; } = ODExplorer.Models.WindowState.Normal;

        public bool IsZero => Top == 0 && Left == 0 && Height == 0 && Width == 0;

        public WindowPositionViewModel Clone() => new() { Top = Top, Left = Left, Height = Height, Width = Width, State = State };
    }
}
