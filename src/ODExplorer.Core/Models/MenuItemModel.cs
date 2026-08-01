using System;
namespace ODExplorer.Models
{
    public sealed class MenuItemModel
    {
        public string Header { get; init; } = string.Empty;
        public Action? Execute { get; init; }
        public bool IsEnabled { get; init; } = true;
    }
}
