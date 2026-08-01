using System;
using System.Collections.Generic;

namespace ODExplorer.Models
{
    public enum GridUnitType
    {
        Auto,
        Pixel,
        Star
    }

    // Minimal GridLength replacement for core — UI can map to framework-specific GridLength
    public readonly record struct GridLength(double Value, GridUnitType GridUnitType)
    {
        public static GridLength Auto => new(0, GridUnitType.Auto);
        public static GridLength Pixel(double px) => new(px, GridUnitType.Pixel);
        public static GridLength Star(double value) => new(value, GridUnitType.Star);
    }

    public sealed class GridSize
    {
        public GridLength this[int index]
        {
            get
            {
                if (index < 0 || GridLengths is null || GridLengths.Count - 1 < index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return GridLengths[index];
            }
            set
            {
                if (index < 0 || GridLengths is null || GridLengths.Count - 1 < index)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                GridLengths[index] = value;
            }
        }

        public List<GridLength>? GridLengths { get; set; }
    }
}
