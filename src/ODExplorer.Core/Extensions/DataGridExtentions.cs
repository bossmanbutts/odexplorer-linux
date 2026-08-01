using System.ComponentModel;
using System.Collections.Generic;

namespace ODExplorer.Extensions
{
    // DataGrid extensions are UI-specific. Provide a no-op placeholder in core; move the real implementation to the UI project.
    public static class DataGridExtensions
    {
        public static void SortDataGrid<T>(this object unused, List<object> sortDescriptions)
        {
            // No-op in core. UI layer should provide an extension targeting the concrete DataGrid control.
        }

        public static ListSortDirection Reverse(this ListSortDirection sortDirection)
        {
            return sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }
    }
}
