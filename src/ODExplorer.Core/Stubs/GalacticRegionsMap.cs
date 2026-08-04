// Minimal GalacticRegions lookup for parity v1. The real ODUtils implementation
// uses precise region polygons; this distance-based heuristic is approximate and
// documented so the real helper can replace it later.

using System;

namespace ODUtils.EliteDangerousHelpers.GalacticRegions
{
    public sealed class GalacticRegion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public static class RegionMap
    {
        // Galactic centre approximation (light years from Sol) used to classify the Core.
        private const double CoreCentreX = -26000;
        private const double CoreCentreY = 0;
        private const double CoreCentreZ = 0;
        private const double BubbleRadius = 1000;
        private const double CoreRadius = 12000;

        public static GalacticRegion FindRegion(double x, double y, double z)
        {
            var solDistance = Math.Sqrt(x * x + y * y + z * z);

            if (solDistance < BubbleRadius)
                return Make(ODUtils.Models.GalacticRegions.Bubble);

            var coreDistance = Math.Sqrt(
                (x - CoreCentreX) * (x - CoreCentreX) +
                (y - CoreCentreY) * (y - CoreCentreY) +
                (z - CoreCentreZ) * (z - CoreCentreZ));

            if (coreDistance < CoreRadius)
                return Make(ODUtils.Models.GalacticRegions.Core);

            return Make(ODUtils.Models.GalacticRegions.OuterRim);
        }

        private static GalacticRegion Make(ODUtils.Models.GalacticRegions region)
        {
            return new GalacticRegion { Id = (int)region, Name = region.ToString() };
        }
    }
}
