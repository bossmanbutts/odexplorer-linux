using ODExplorer.ViewModels.ModelVMs;
using ODUtils.Dialogs.ViewModels;
using System.Collections.ObjectModel;

namespace ODExplorer.Models
{
    /// <summary>
    /// A star with its child planetary bodies, used for tree-style display.
    /// Stars remain fixed at the top; children are sorted underneath.
    /// Also used for barycentre groups (binary-orbiting bodies).
    /// </summary>
    public sealed class StarBodyGroup : OdViewModelBase
    {
        public SystemBodyViewModel? Star { get; }
        public ObservableCollection<SystemBodyViewModel> Children { get; }
        public bool IsBarycentre { get; }
        public string BarycentreLabel { get; } = string.Empty;

        private bool isExpanded = true;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
                OnPropertyChanged(nameof(ExpandGlyph));
            }
        }

        public string ExpandGlyph => IsExpanded ? "▼" : "▶";
        public bool HasChildren => Children.Count > 0;

        public StarBodyGroup(SystemBodyViewModel star, ObservableCollection<SystemBodyViewModel> children)
        {
            Star = star;
            Children = children;
        }

        public StarBodyGroup(string barycentreLabel, ObservableCollection<SystemBodyViewModel> children)
        {
            Star = null;
            IsBarycentre = true;
            BarycentreLabel = barycentreLabel;
            Children = children;
        }

        public void ToggleExpanded()
        {
            IsExpanded = !IsExpanded;
        }
    }
}
