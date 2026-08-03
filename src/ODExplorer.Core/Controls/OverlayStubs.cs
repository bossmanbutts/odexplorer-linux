namespace ODExplorer.Controls
{
    // Stub overlays — UI project should subclass these with real Avalonia windows.
    // Core ViewModels reference these types to trigger OpenPopoutRequested via MainViewModel.OpenPopout().

    public class SystemBodiesOverlay : ODExplorer.Models.PopOutBase
    {
        public override string Title => "System Bodies";
    }

    public class ExobiologyOverlay : ODExplorer.Models.PopOutBase
    {
        public override string Title => "Exobiology";
    }
}
