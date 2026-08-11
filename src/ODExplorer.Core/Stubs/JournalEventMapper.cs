// Maps journal JSON lines to typed JournalEntry instances for the in-memory
// pipeline. Parsing is delegated to the vendored EliteJournalReader library
// (EliteJournalReader.JournalWatcher.GetEventData), which produces the real
// typed *EventArgs objects; this class only wraps them into the ODUtils.Journal
// JournalEntry and exposes value/region helpers shared with the stores.

using System;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Extensions;
using ODUtils.Journal;
using ODUtils.Models;
using AtmosphereClass = ODUtils.Models.AtmosphereClass;
using GalacticRegions = ODUtils.Models.GalacticRegions;
using JournalEntry = ODUtils.Journal.JournalEntry;
using OrganicScanStage = ODUtils.Models.OrganicScanStage;
using PlanetClass = ODUtils.Models.PlanetClass;
using StarType = ODUtils.Models.StarType;

namespace ODExplorer.Journal
{
    public static class JournalEventMapper
    {
        public static JournalEntry? Map(string line, string filename, int commanderId)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            JournalEventArgs? data;
            try
            {
                data = EliteJournalReader.JournalWatcher.GetEventData(line);
            }
            catch
            {
                return null;
            }

            if (data is null)
                return null;

            var eventName = data.OriginalEvent?["event"]?.ToString();
            if (string.IsNullOrEmpty(eventName))
                return null;

            return new JournalEntry
            {
                Event = eventName,
                EventType = JournalTypeHelpers.FromString(eventName),
                CommanderID = commanderId,
                TimeStamp = data.Timestamp,
                Filename = filename,
                Offset = 0,
                OriginalEvent = data.OriginalEvent,
                EventData = data
            };
        }

        // ── StarSystem / SystemBody value helpers (shared with the stores) ────────

        public static StarType GetStarType(string? starType)
        {
            if (string.IsNullOrEmpty(starType))
                return StarType.Unknown;

            return starType switch
            {
                "T Tauri" => StarType.TTS,
                "Herbig Ae/Be" => StarType.AeBe,
                "Wolf-Rayet" => StarType.W,
                "Wolf-Rayet N" => StarType.WN,
                "Wolf-Rayet NC" => StarType.WNC,
                "Wolf-Rayet C" => StarType.WC,
                "Wolf-Rayet O" => StarType.WO,
                "Carbon Star" => StarType.C,
                "C-N" => StarType.CN,
                "C-J" => StarType.CJ,
                "C-H" => StarType.CH,
                "C-Hd" => StarType.CHd,
                "MS-type" => StarType.MS,
                "S-type" => StarType.S,
                "Neutron Star" => StarType.N,
                "Black Hole" => StarType.BH,
                "Supergiant" => StarType.X,
                _ => Enum.TryParse<StarType>(starType, true, out var st) ? st : StarType.Unknown
            };
        }

        public static StarType GetStarType(EliteJournalReader.StarType starType)
            => GetStarType(starType.ToString());

        public static StarLuminosityClass GetStarLuminosity(string? luminosity)
        {
            if (string.IsNullOrEmpty(luminosity))
                return StarLuminosityClass.Unknown;

            return luminosity switch
            {
                "Ia" => StarLuminosityClass.Ia,
                "Ib" => StarLuminosityClass.Ib,
                "Iab" => StarLuminosityClass.Ib,
                "I" => StarLuminosityClass.Ia,
                "II" => StarLuminosityClass.II,
                "III" => StarLuminosityClass.III,
                "IIIa" => StarLuminosityClass.IIIa,
                "IIIb" => StarLuminosityClass.IIIb,
                "IV" => StarLuminosityClass.IV,
                "V" => StarLuminosityClass.V,
                "Va" => StarLuminosityClass.Va,
                "Vb" => StarLuminosityClass.Vb,
                "Vz" => StarLuminosityClass.Vz,
                "VI" => StarLuminosityClass.VI,
                "VII" => StarLuminosityClass.VII,
                _ => Enum.TryParse<StarLuminosityClass>(luminosity, true, out var lc) ? lc : StarLuminosityClass.Unknown
            };
        }

        public static StarLuminosityClass GetStarLuminosity(EliteJournalReader.StarLuminosity luminosity)
            => GetStarLuminosity(luminosity.ToString());

        public static PlanetClass GetPlanetClass(string? planetClass)
        {
            if (string.IsNullOrEmpty(planetClass))
                return PlanetClass.Unknown;

            return planetClass switch
            {
                "Earthlike body" => PlanetClass.EarthlikeBody,
                "Water world" => PlanetClass.WaterWorld,
                "Ammonia world" => PlanetClass.AmmoniaWorld,
                "High metal content body" => PlanetClass.HighMetalContentBody,
                "Metal-rich body" => PlanetClass.MetalRichBody,
                "Rocky body" => PlanetClass.RockyBody,
                "Rocky ice world" => PlanetClass.RockyIceBody,
                "Icy body" => PlanetClass.IcyBody,
                "Sudarsky class I gas giant" => PlanetClass.SudarskyClassIGasGiant,
                "Sudarsky class II gas giant" => PlanetClass.SudarskyClassIIGasGiant,
                "Sudarsky class III gas giant" => PlanetClass.SudarskyClassIIIGasGiant,
                "Sudarsky class IV gas giant" => PlanetClass.SudarskyClassIVGasGiant,
                "Sudarsky class V gas giant" => PlanetClass.SudarskyClassVGasGiant,
                "Gas giant with water-based life" => PlanetClass.GasGiantWithWaterBasedLife,
                "Gas giant with ammonia-based life" => PlanetClass.GasGiantWithAmmoniaBasedLife,
                "Water giant" => PlanetClass.WaterGiant,
                "Water giant with life" => PlanetClass.WaterGiantWithLife,
                "Helium-rich gas giant" => PlanetClass.HeliumRichGasGiant,
                "Helium gas giant" => PlanetClass.HeliumGasGiant,
                _ => Enum.TryParse<PlanetClass>(planetClass, true, out var pc) ? pc : PlanetClass.Unknown
            };
        }

        public static PlanetClass GetPlanetClass(EliteJournalReader.PlanetClass planetClass)
            => GetPlanetClass(planetClass.ToString());

        public static AtmosphereClass GetAtmosphereClass(string? atmosphere)
        {
            if (string.IsNullOrEmpty(atmosphere))
                return AtmosphereClass.None;

            var a = atmosphere.ToLowerInvariant();

            if (a.Contains("suitable for water-based life")) return AtmosphereClass.SuitableForWaterBasedLife;
            if (a.Contains("ammonia") && a.Contains("oxygen")) return AtmosphereClass.AmmoniaOxygen;
            if (a.Contains("ammonia-rich")) return AtmosphereClass.AmmoniaRich;
            if (a.Contains("ammonia")) return AtmosphereClass.Ammonia;
            if (a.Contains("earthlike")) return AtmosphereClass.EarthLike;
            if (a.Contains("water-rich")) return AtmosphereClass.WaterRich;
            if (a.Contains("water")) return AtmosphereClass.Water;
            if (a.Contains("carbon dioxide-rich")) return AtmosphereClass.CarbonDioxideRich;
            if (a.Contains("carbon dioxide")) return AtmosphereClass.CarbonDioxide;
            if (a.Contains("sulphur dioxide")) return AtmosphereClass.SulphurDioxide;
            if (a.Contains("methane-rich")) return AtmosphereClass.MethaneRich;
            if (a.Contains("methane")) return AtmosphereClass.Methane;
            if (a.Contains("nitrogen")) return AtmosphereClass.Nitrogen;
            if (a.Contains("neon-rich")) return AtmosphereClass.NeonRich;
            if (a.Contains("neon")) return AtmosphereClass.Neon;
            if (a.Contains("argon-rich")) return AtmosphereClass.ArgonRich;
            if (a.Contains("argon")) return AtmosphereClass.Argon;
            if (a.Contains("oxygen")) return AtmosphereClass.Oxygen;
            if (a.Contains("helium")) return AtmosphereClass.Helium;
            if (a.Contains("silicate vapour")) return AtmosphereClass.SilicateVapour;
            if (a.Contains("metallic vapour")) return AtmosphereClass.MetallicVapour;
            if (a.Contains("no atmosphere")) return AtmosphereClass.NoAtmosphere;
            return AtmosphereClass.Unknown;
        }

        // The real lib exposes the journal "Atmosphere" description as a
        // snake_case AtmosphereDescription enum; match on the underscore tokens.
        public static AtmosphereClass GetAtmosphereClass(EliteJournalReader.AtmosphereDescription atmosphere)
        {
            var a = atmosphere.ToString().ToLowerInvariant();

            if (a.Contains("carbon_dioxide") && a.Contains("rich")) return AtmosphereClass.CarbonDioxideRich;
            if (a.Contains("carbon_dioxide")) return AtmosphereClass.CarbonDioxide;
            if (a.Contains("sulphur_dioxide") || a.Contains("sulfur_dioxide")) return AtmosphereClass.SulphurDioxide;
            if (a.Contains("water") && a.Contains("rich")) return AtmosphereClass.WaterRich;
            if (a.Contains("water")) return AtmosphereClass.Water;
            if (a.Contains("ammonia") && a.Contains("rich")) return AtmosphereClass.AmmoniaRich;
            if (a.Contains("ammonia")) return AtmosphereClass.Ammonia;
            if (a.Contains("argon") && a.Contains("rich")) return AtmosphereClass.ArgonRich;
            if (a.Contains("argon")) return AtmosphereClass.Argon;
            if (a.Contains("methane") && a.Contains("rich")) return AtmosphereClass.MethaneRich;
            if (a.Contains("methane")) return AtmosphereClass.Methane;
            if (a.Contains("nitrogen")) return AtmosphereClass.Nitrogen;
            if (a.Contains("metallic_vapour")) return AtmosphereClass.MetallicVapour;
            if (a.Contains("silicate_vapour")) return AtmosphereClass.SilicateVapour;
            if (a.Contains("helium")) return AtmosphereClass.Helium;
            if (a.Equals("none")) return AtmosphereClass.None;
            return AtmosphereClass.Unknown;
        }

        public static AtmosphereClass GetAtmosphereType(string? atmosphereType)
        {
            if (string.IsNullOrEmpty(atmosphereType))
                return AtmosphereClass.None;

            return Enum.TryParse<AtmosphereClass>(atmosphereType, true, out var at) ? at : AtmosphereClass.Unknown;
        }

        public static AtmosphereClass GetAtmosphereType(EliteJournalReader.AtmosphereClass atmosphereType)
            => GetAtmosphereType(atmosphereType.ToString());

        public static VolcanismType GetVolcanism(string? volcanism)
        {
            if (string.IsNullOrEmpty(volcanism) || volcanism.Equals("None", StringComparison.OrdinalIgnoreCase))
                return VolcanismType.None;

            var v = volcanism.ToLowerInvariant();
            bool major = v.Contains("major");
            bool minor = v.Contains("minor");

            if (v.Contains("rocky")) return major ? VolcanismType.MajorRocky : minor ? VolcanismType.MinorRocky : VolcanismType.Rocky;
            if (v.Contains("metallic")) return major ? VolcanismType.MajorMetallic : minor ? VolcanismType.MinorMetallic : VolcanismType.Metallic;
            if (v.Contains("carbon")) return major ? VolcanismType.MajorCarbon : minor ? VolcanismType.MinorCarbon : VolcanismType.Carbon;
            if (v.Contains("water")) return VolcanismType.Water;
            if (v.Contains("ammonia")) return VolcanismType.Ammonia;
            if (v.Contains("nitrogen")) return VolcanismType.Nitrogen;
            if (v.Contains("silicate")) return VolcanismType.Silicate;
            if (v.Contains("iron")) return VolcanismType.Iron;
            return VolcanismType.None;
        }

        public static VolcanismType GetVolcanism(EliteJournalReader.Volcanism volcanism)
            => GetVolcanism(volcanism.ToString());

        // BioScan's get_volcanism() returns the journal volcanism string in lowercase
        // (e.g. "water geysers", "major silicate vapour geysers") so that the rules'
        // case-sensitive substring matching works. Returns "" for no volcanism.
        public static string GetVolcanismName(EliteJournalReader.Volcanism volcanism)
        {
            var desc = volcanism.GetEnumDescription();

            if (string.IsNullOrWhiteSpace(desc) || desc.Equals("No Volcanism", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            desc = desc.ToLowerInvariant();
            const string suffix = " volcanism";
            if (desc.EndsWith(suffix, StringComparison.Ordinal))
                desc = desc[..^suffix.Length];

            return desc;
        }

        public static bool IsTerraformable(string? terraformState)
        {
            if (string.IsNullOrEmpty(terraformState))
                return false;

            return terraformState.Equals("Terraformable", StringComparison.OrdinalIgnoreCase) ||
                   terraformState.Contains("Candidate for terraforming", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsTerraformable(EliteJournalReader.TerraformState terraformState)
            => IsTerraformable(terraformState.ToString());

        // The real lib's OrganicScanStage enum has more members than our model's;
        // collapse the ones the stores track onto the ODUtils.Models enum.
        public static OrganicScanStage GetOrganicScanStage(EliteJournalReader.OrganicScanStage stage)
        {
            return stage switch
            {
                EliteJournalReader.OrganicScanStage.Analyse => OrganicScanStage.Analyse,
                EliteJournalReader.OrganicScanStage.Codex => OrganicScanStage.Codex,
                EliteJournalReader.OrganicScanStage.Prediction => OrganicScanStage.Prediction,
                _ => OrganicScanStage.Log
            };
        }

        // The real GalacticRegions enum is the full codex region list; our model
        // keeps a coarse Unknown/Core/Bubble/OuterRim bucket for the checklist UI.
        public static GalacticRegions GetGalacticRegion(EliteJournalReader.GalacticRegions region)
        {
            return region switch
            {
                EliteJournalReader.GalacticRegions.Unknown => GalacticRegions.Unknown,
                EliteJournalReader.GalacticRegions.Codex_RegionName_1 => GalacticRegions.Core,      // Galactic Centre
                EliteJournalReader.GalacticRegions.Codex_RegionName_27 or                              // Outer Arm
                EliteJournalReader.GalacticRegions.Codex_RegionName_35 or                              // Outer Orion Spur
                EliteJournalReader.GalacticRegions.Codex_RegionName_15 or                              // Outer Orion-Perseus Conflux
                EliteJournalReader.GalacticRegions.Codex_RegionName_26 => GalacticRegions.OuterRim,    // Outer Scutum-Centaurus Arm
                _ => GalacticRegions.Bubble
            };
        }

        // Approximate first-discovery FSS value per body type.
        public static long GetFssValue(PlanetClass planetClass)
        {
            return planetClass switch
            {
                PlanetClass.EarthlikeBody => 1_172_950,
                PlanetClass.WaterWorld => 780_250,
                PlanetClass.AmmoniaWorld => 780_250,
                PlanetClass.WaterGiantWithLife => 103_800,
                PlanetClass.WaterGiant => 77_850,
                PlanetClass.GasGiantWithWaterBasedLife or PlanetClass.GasGiantWithAmmoniaBasedLife => 77_850,
                PlanetClass.SudarskyClassIVGasGiant or PlanetClass.SudarskyClassVGasGiant => 77_850,
                PlanetClass.SudarskyClassIGasGiant or PlanetClass.SudarskyClassIIIGasGiant => 51_900,
                PlanetClass.HeliumRichGasGiant or PlanetClass.HeliumGasGiant => 155_700,
                PlanetClass.SudarskyClassIIGasGiant => 38_900,
                PlanetClass.MetalRichBody => 38_900,
                PlanetClass.HighMetalContentBody => 25_950,
                PlanetClass.RockyIceBody => 5_190,
                PlanetClass.RockyBody => 6_490,
                PlanetClass.IcyBody => 3_890,
                _ => 0
            };
        }
    }
}
