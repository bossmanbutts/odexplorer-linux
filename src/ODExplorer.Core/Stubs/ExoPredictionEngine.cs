// Prediction engine mirroring the BioScan matcher (load.py, bio_scan/bio_data/
// rulesets/*.py) for the rule subset that is evaluable from journal-derived body
// data. Rules that depend on galaxy position (regions, nebulae, guardian zones)
// are excluded at data-generation time so the engine never predicts a species
// it cannot verify; it therefore under-predicts rather than over-predicts.

using System;
using System.Collections.Generic;
using System.Linq;
using ODUtils.Models;

namespace ODUtils.Exobiology
{
    internal static class ExoPredictionEngine
    {
        private const double EarthGravity = 9.797759;
        private const double EarthPressure = 101231.656250;
        private const double SecondsPerDay = 86400.0;

        public static List<PredSpecies> Predict(SystemBody body, StarSystem system)
        {
            var result = new List<PredSpecies>();

            if (body.IsPlanet == false || body.PlanetClass == PlanetClass.Unknown)
                return result;

            foreach (var species in ExoPredictionRules.All)
            {
                if (species.Rules.Length == 0)
                    continue;

                foreach (var rule in species.Rules)
                {
                    if (Matches(rule, body, system))
                    {
                        result.Add(species);
                        break;
                    }
                }
            }

            return result;
        }

        private static bool Matches(PredRule rule, SystemBody body, StarSystem system)
        {
            if (rule.BodyType is not null && rule.BodyType.Contains(GetBodyType(body)) == false)
                return false;

            if (rule.Atmosphere is not null && rule.Atmosphere.Contains(GetAtmosphereToken(body)) == false)
                return false;

            if (rule.AtmosphereComponent is not null)
            {
                foreach (var (gas, percent) in rule.AtmosphereComponent)
                {
                    if (GetGasPercent(body, gas) < percent)
                        return false;
                }
            }

            if (rule.MinGravity is not null && body.SurfaceGravity / EarthGravity < rule.MinGravity.Value)
                return false;
            if (rule.MaxGravity is not null && body.SurfaceGravity / EarthGravity > rule.MaxGravity.Value)
                return false;

            // BioScan skips the temperature/pressure checks when the value is unknown
            // ("if not body.get_temp(): continue"), so a missing value passes the rule.
            if (rule.MinTemperature is not null && body.SurfaceTemp != 0 && body.SurfaceTemp < rule.MinTemperature.Value)
                return false;
            if (rule.MaxTemperature is not null && body.SurfaceTemp != 0 && body.SurfaceTemp > rule.MaxTemperature.Value)
                return false;

            if (rule.MinPressure is not null && body.SurfacePressure != 0 && body.SurfacePressure / EarthPressure < rule.MinPressure.Value)
                return false;
            if (rule.MaxPressure is not null && body.SurfacePressure != 0 && body.SurfacePressure / EarthPressure >= rule.MaxPressure.Value)
                return false;

            if (rule.MaxOrbitalPeriod is not null && body.OrbitalPeriod / SecondsPerDay >= rule.MaxOrbitalPeriod.Value)
                return false;

            if (rule.Volcanism is not null && MatchesVolcanism(rule.Volcanism, GetVolcanism(body)) == false)
                return false;

            if (rule.ParentStar is not null && MatchesParentStar(rule.ParentStar, body, system) == false)
                return false;

            if (rule.Distance is not null && body.DistanceFromArrivalLs < rule.Distance.Value)
                return false;

            return true;
        }

        private static string GetBodyType(SystemBody body)
        {
            return body.PlanetClass switch
            {
                PlanetClass.IcyBody => "Icy body",
                PlanetClass.RockyBody => "Rocky body",
                PlanetClass.RockyIceBody => "Rocky ice body",
                PlanetClass.HighMetalContentBody => "High metal content body",
                PlanetClass.MetalRichBody => "Metal rich body",
                _ => string.Empty
            };
        }

        // BioScan's get_atmosphere() returns the journal atmosphere string with
        // thin/thick stripped and formatted as a CamelCase token ("carbon dioxide"
        // -> "CarbonDioxide", "argon-rich" -> "ArgonRich"); the AtmosphereClass
        // enum member names already match those tokens exactly.
        private static string GetAtmosphereToken(SystemBody body)
            => body.Atmosphere == AtmosphereClass.Unknown ? string.Empty : body.Atmosphere.ToString();

        private static double GetGasPercent(SystemBody body, string gas)
        {
            var item = body.AtmosphereComposition?.FirstOrDefault(c => c.Name == gas);
            return item?.Percent ?? 0;
        }

        private static string GetVolcanism(SystemBody body)
            => body.VolcanismName.ToLowerInvariant();

        private static bool MatchesVolcanism(List<string> entries, string volcanism)
        {
            // Scalar BioScan values are wrapped into single-entry lists at generation.
            if (entries.Count == 1 && entries[0] == "None")
                return string.IsNullOrEmpty(volcanism);
            if (entries.Count == 1 && entries[0] == "Any")
                return string.IsNullOrEmpty(volcanism) == false;

            foreach (var entry in entries)
            {
                if (entry.StartsWith('=') ? volcanism == entry[1..] : volcanism.Contains(entry, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static bool MatchesParentStar(List<string> entries, SystemBody body, StarSystem system)
        {
            // BioScan checks the system's main star first, then the body's parent
            // stars. system.StarType is the main star; GoverningStar covers parents.
            foreach (var starType in entries)
            {
                if (StarCheck(starType, system.StarType) || StarCheck(starType, body.GoverningStar))
                    return true;
            }

            return false;
        }

        // Mirrors BioScan's star_check(): letter queries match the base class plus
        // its giant variants; D/C/W prefix-match the white dwarf/carbon/wolf-rayet
        // subclasses. Our StarType enum already collapses giant variants onto the
        // base letter, so plain equality is correct for those.
        private static bool StarCheck(string query, StarType star)
        {
            if (star == StarType.Unknown)
                return false;

            return query switch
            {
                "D" or "C" or "W" => star.ToString().StartsWith(query, StringComparison.Ordinal),
                _ => star.ToString() == query
            };
        }
    }
}
