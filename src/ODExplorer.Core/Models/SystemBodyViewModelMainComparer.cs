using ODExplorer.ViewModels.ModelVMs;
using System;
using System.Collections;
using System.ComponentModel;

namespace ODExplorer.Models
{
    public sealed class SystemBodyViewModelMainComparer(SystemGridSettings settings) : IComparer
    {
        private readonly SystemGridSettings settings = settings;

        public int Compare(object? x, object? y)
        {
            if (x is not SystemBodyViewModel a || y is not SystemBodyViewModel b)
                throw new ArgumentException("Not a SystemBodyViewModel");

            if (settings.ExcludeStarsFromSorting)
            {
                var isStar = a.IsStar.CompareTo(b.IsStar);
                if (isStar != 0)
                    return isStar;

                if (a.IsStar && b.IsStar)
                {
                    return b.DistanceFromArrival.CompareTo(a.DistanceFromArrival);
                }
            }

            var isEdsmVb = b.IsEdsmVb.CompareTo(a.IsEdsmVb);

            if (isEdsmVb != 0)
                return isEdsmVb;

            var direction = settings.SortDirection == ListSortDirection.Ascending ? 1 : -1;

            switch (settings.BodySortingOptions)
            {
                case BodySortCategory.Distance:
                    if (direction == 1)
                        return a.DistanceFromArrival.CompareTo(b.DistanceFromArrival);
                    return b.DistanceFromArrival.CompareTo(a.DistanceFromArrival);
                case BodySortCategory.Name:
                    if (direction == 1)
                        return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
                    return string.Compare(b.Name, a.Name, StringComparison.OrdinalIgnoreCase);
                case BodySortCategory.BioSignals:
                    if (direction == 1)
                        return a.BiologicalSignals.CompareTo(b.BiologicalSignals);
                    return b.BiologicalSignals.CompareTo(a.BiologicalSignals);
                case BodySortCategory.GeoSignals:
                    if (direction == 1)
                        return a.GeologicalSignals.CompareTo(b.GeologicalSignals);
                    return b.GeologicalSignals.CompareTo(a.GeologicalSignals);
                case BodySortCategory.Value:
                default:
                    if (direction == 1)
                        return a.MappedValueActual.CompareTo(b.MappedValueActual);
                    return b.MappedValueActual.CompareTo(a.MappedValueActual);
            }
        }
    }
}
