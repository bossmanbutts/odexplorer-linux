using System;
using System.Collections.Generic;
using System.Linq;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Extensions;
using ScanItemComponent = EliteJournalReader.Events.ScanItemComponent;
using ShipMaterials = EliteJournalReader.Events.ShipMaterials;

namespace ODUtils.Exobiology
{
    // Converts the port's SystemBody/StarSystem models into the ExoPlanet the
    // prediction engine consumes, translating the ODUtils.Models enums to the
    // EliteJournalReader equivalents the rules compare against.
    public static class ExoPlanetBuilder
    {
        public static ExoPlanet Build(ODUtils.Models.SystemBody body, ODUtils.Models.StarSystem system, DateTime timestamp, double distanceToNebula = 0)
        {
            var stars = system.SystemBodies
                .Where(x => x.IsStar && x.StarType != ODUtils.Models.StarType.Unknown)
                .Select(x => ToEjrStarType(x.StarType))
                .Distinct()
                .ToList();

            if (stars.Count == 0 && body.GoverningStar != ODUtils.Models.StarType.Unknown)
                stars.Add(ToEjrStarType(body.GoverningStar));

            var parentStars = new List<StarType>();
            if (body.GoverningStar != ODUtils.Models.StarType.Unknown)
                parentStars.Add(ToEjrStarType(body.GoverningStar));

            var composition = body.AtmosphereComposition?
                .Select(a => new ScanItemComponent { Name = a.Name, Percent = a.Percent })
                .ToList() ?? [];

            var materials = body.Materials?
                .Select(m => new ShipMaterials { Name = ToEjrMaterial(m.Name), Percent = m.Percent })
                .ToList();

            return new ExoPlanet(
                ToEjrPlanetClass(body.PlanetClass),
                body.AtmosphereDescription,
                ToEjrAtmosphereClass(body.AtmosphereType),
                composition,
                ToEjrVolcanism(body.VolcanismName),
                body.SurfaceGravity,
                body.SurfaceTemp,
                body.SurfacePressure / 101325,
                body.DistanceFromArrivalLs,
                body.OrbitalPeriod,
                materials,
                stars,
                parentStars.Distinct().ToList(),
                ToEjrRegion(body.Owner.Region.Name),
                timestamp,
                distanceToNebula,
                body.BiologicalSignals,
                system.Address);
        }

        private static PlanetClass ToEjrPlanetClass(ODUtils.Models.PlanetClass planetClass)
            => Enum.TryParse(planetClass.ToString(), out PlanetClass result) ? result : PlanetClass.Unknown;

        private static AtmosphereClass ToEjrAtmosphereClass(ODUtils.Models.AtmosphereClass atmosphereClass)
            => Enum.TryParse(atmosphereClass.ToString(), out AtmosphereClass result) ? result : AtmosphereClass.Unknown;

        private static Volcanism ToEjrVolcanism(string? volcanismName)
        {
            if (string.IsNullOrWhiteSpace(volcanismName))
                return Volcanism.None;

            var token = $"{volcanismName.Replace(' ', '_')}_volcanism";
            return Enum.TryParse(token, ignoreCase: true, out Volcanism result) ? result : Volcanism.None;
        }

        private static StarType ToEjrStarType(ODUtils.Models.StarType starType)
        {
            if (starType == ODUtils.Models.StarType.BH)
                return StarType.H;

            return Enum.TryParse(starType.ToString(), out StarType result) ? result : StarType.Unknown;
        }

        private static PlanetMaterial ToEjrMaterial(string materialName)
            => Enum.TryParse(materialName, ignoreCase: true, out PlanetMaterial result) ? result : PlanetMaterial.None;

        private static GalacticRegions ToEjrRegion(string? regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                return GalacticRegions.Unknown;

            foreach (var region in Enum.GetValues<GalacticRegions>())
            {
                if (region != GalacticRegions.Unknown && region.GetEnumDescription().Equals(regionName, StringComparison.OrdinalIgnoreCase))
                    return region;
            }

            return GalacticRegions.Unknown;
        }
    }
}
