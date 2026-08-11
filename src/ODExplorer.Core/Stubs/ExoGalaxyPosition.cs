// Galaxy-position resolution for the exo prediction engine, mirroring BioScan:
//   - region id: RLE grid lookup (EliteDangerousRegionMap / ExploData findRegion)
//     over ExoGalaxyData.RegionRows
//   - region rules: bio_scan/bio_data/regions.py region_map membership
//   - nebula rules: nebula-sector name prefixes plus nearest large/planetary
//     nebula distance checks (bio_scan/nebula_data)
//
// BioScan semantics are preserved exactly: a rule that cannot be evaluated
// because the position is unknown is skipped (the species is kept), matching
// load.py's `if this.system.region is not None` / `if not this.system.x`.

using System;

namespace ODUtils.Exobiology
{
    internal static class ExoGalaxyPosition
    {
        // Map origin for the region grid (RegionMap.py: x0, z0).
        private const double X0 = -49985;
        private const double Z0 = -24105;
        private const double LargeNebulaDistance = 150.0;
        private const double PlanetaryNebulaDistance = 100.0;

        // Returns the region row id for a galactic position, or null when the
        // position is outside the region map. Mirrors ExploData's findRegion().
        public static int? FindRegionId(double x, double y, double z)
        {
            var px = (int)((x - X0) * 83 / 4096);
            var pz = (int)((z - Z0) * 83 / 4096);

            if (px < 0 || pz < 0 || pz >= ExoGalaxyData.RegionRows.Length)
                return null;

            var row = ExoGalaxyData.RegionRows[pz];
            var rx = 0;
            var pv = 0;

            for (int i = 0; i < row.Length; i += 2)
            {
                var runLength = row[i];
                if (px < rx + runLength)
                {
                    pv = row[i + 1];
                    break;
                }
                rx += runLength;
            }

            return pv == 0 ? null : pv;
        }

        public static string? FindRegionName(double x, double y, double z)
        {
            var id = FindRegionId(x, y, z);
            return id is null ? null : ExoGalaxyData.RegionNames[id.Value];
        }

        // BioScan 'regions' rule (load.py:510). A null region id skips the rule
        // entirely; '!name' entries eliminate the rule when the id is inside
        // name; a rule with positive entries requires at least one match.
        public static bool MatchesRegions(int? regionId, System.Collections.Generic.List<string> regions)
        {
            if (regionId is null)
                return true;

            foreach (var region in regions)
            {
                if (region.StartsWith('!') && InRegion(regionId.Value, region[1..]))
                    return false;
            }

            var count = 0;
            foreach (var region in regions)
            {
                if (region.StartsWith('!'))
                    continue;

                count++;
                if (InRegion(regionId.Value, region))
                    return true;
            }

            return count == 0;
        }

        public static bool InRegion(int regionId, string ruleRegion)
        {
            return ExoGalaxyData.RegionMapRules.TryGetValue(ruleRegion, out var ids)
                && Array.IndexOf(ids, (short)regionId) >= 0;
        }

        // BioScan 'nebula' rule (load.py:638). Skipped when x is unknown; 'large'
        // accepts a sector-name prefix or a large-named nebula within 150 ly;
        // 'all' additionally accepts a planetary nebula within 100 ly.
        public static bool MatchesNebula(string systemName, double x, double y, double z, string type)
        {
            if (x == 0)
                return true;

            if (type is not ("all" or "large"))
                return true;

            foreach (var sector in ExoGalaxyData.NebulaSectors)
            {
                if (systemName.StartsWith(sector, StringComparison.Ordinal))
                    return true;
            }

            if (NearestDistance(ExoGalaxyData.LargeNamedNebula, x, y, z) < LargeNebulaDistance)
                return true;

            if (type == "all" && NearestDistance(ExoGalaxyData.PlanetaryNebula, x, y, z) < PlanetaryNebulaDistance)
                return true;

            return false;
        }

        private static double NearestDistance(float[] flatTriplets, double x, double y, double z)
        {
            var min = double.MaxValue;

            for (int i = 0; i + 2 < flatTriplets.Length; i += 3)
            {
                var dx = flatTriplets[i] - x;
                var dy = flatTriplets[i + 1] - y;
                var dz = flatTriplets[i + 2] - z;
                var d = dx * dx + dy * dy + dz * dz;
                if (d < min)
                    min = d;
            }

            return Math.Sqrt(min);
        }
    }
}
