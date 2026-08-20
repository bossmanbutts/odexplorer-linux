using ODExplorer.ViewModels.ModelVMs;
using System.Collections.ObjectModel;

namespace ODExplorer.Models
{
    /// <summary>
    /// A star with its child planetary bodies, used for tree-style display.
    /// Stars remain fixed at the top; children are sorted underneath.
    /// </summary>
    public sealed class StarBodyGroup
    {
        public SystemBodyViewModel Star { get; }
        public ObservableCollection<SystemBodyViewModel> Children { get; }

        public StarBodyGroup(SystemBodyViewModel star, ObservableCollection<SystemBodyViewModel> children)
        {
            Star = star;
            Children = children;
        }
    }
}
