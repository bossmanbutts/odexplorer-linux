// AUTO-GENERATED from the BioScan rulesets (https://github.com/EDDiscovery/BioScan)
// bio_scan/bio_data/rulesets/*.py — the 15 Odyssey genera. Do not edit by hand;
// regenerate with the Python script in /tmp/opencode/bioscan (one-off port).
//
// Only rules usable from journal-derived body data are included here; rules that
// require galaxy-position data (galactic regions, nebulae, guardian zones) are
// omitted so the engine never makes a prediction it cannot verify.

using System;
using System.Collections.Generic;

namespace ODUtils.Exobiology
{
    // Prediction criteria mirroring the BioScan matcher. All values are the raw
    // BioScan value forms; the engine normalizes journal data to match.
    internal sealed class PredRule
    {
        public List<string>? BodyType { get; set; }
        public List<string>? Atmosphere { get; set; }
        public Dictionary<string, double>? AtmosphereComponent { get; set; }
        public double? MinGravity { get; set; }
        public double? MaxGravity { get; set; }
        public double? MinTemperature { get; set; }
        public double? MaxTemperature { get; set; }
        public double? MinPressure { get; set; }
        public double? MaxPressure { get; set; }
        public double? MaxOrbitalPeriod { get; set; }
        // Entries: "Any" = volcanism required; "None" = volcanism forbidden;
        // "=x" = exact match on volcanism string; otherwise word-prefix substring match.
        public List<string>? Volcanism { get; set; }
        public List<string>? Regions { get; set; }
        public List<string>? Bodies { get; set; }
        public List<string>? MainStar { get; set; }
        public List<string>? Star { get; set; }
        public List<string>? ParentStar { get; set; }
        public string? Nebula { get; set; }
        public bool? Guardian { get; set; }
        public List<string>? Tuber { get; set; }
        public double? Distance { get; set; }
        public string? System { get; set; }
        public List<string>? MainStarLum { get; set; }
        public List<string>? Luminosity { get; set; }
    }

    internal sealed class PredSpecies
    {
        public PredSpecies(string codex, string name, long value, PredRule[] rules)
        {
            Codex = codex;
            Name = name;
            Value = value;
            Rules = rules;
        }

        public string Codex { get; }
        public string Name { get; }
        public long Value { get; }
        public PredRule[] Rules { get; }
    }

    internal static class ExoPredictionRules
    {
        public static readonly PredSpecies[] All =
        [
    // ── aleoida ──
    new PredSpecies("$Codex_Ent_Aleoids_01_Name;", "Aleoida Arcus", 7252500, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 175, MaxTemperature = 180, MinPressure = 0.0161, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Aleoids_02_Name;", "Aleoida Coronamus", 6284600, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 180, MaxTemperature = 190, MinPressure = 0.025, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Aleoids_03_Name;", "Aleoida Spica", 3385200, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Aleoids_04_Name;", "Aleoida Laminiae", 3385200, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Aleoids_05_Name;", "Aleoida Gravis", 12934900, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 190, MaxTemperature = 197, MinPressure = 0.054, Volcanism = [ "None" ] },
    }),
    // ── bacterium ──
    new PredSpecies("$Codex_Ent_Bacterial_01_Name;", "Bacterium Aurasus", 1000000, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body", "Rocky ice body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.039, MaxGravity = 0.608, MinTemperature = 145, MaxTemperature = 400 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_02_Name;", "Bacterium Nebulus", 5289900, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Helium" ], MinGravity = 0.4, MaxGravity = 0.55, MinTemperature = 20, MaxTemperature = 21, MinPressure = 0.067 },
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "Helium" ], MinGravity = 0.4, MaxGravity = 0.7, MinTemperature = 20, MaxTemperature = 21, MinPressure = 0.067 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_03_Name;", "Bacterium Scopulum", 4934500, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Argon" ], MinGravity = 0.15, MaxGravity = 0.26, MinTemperature = 56, MaxTemperature = 150, Volcanism = [ "carbon dioxide", "methane" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Helium" ], MinGravity = 0.48, MaxGravity = 0.51, MinTemperature = 20, MaxTemperature = 21, MinPressure = 0.075, Volcanism = [ "methane" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Methane" ], MinGravity = 0.025, MaxGravity = 0.047, MinTemperature = 84, MaxTemperature = 110, MinPressure = 0.03, Volcanism = [ "methane" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Neon" ], MinGravity = 0.025, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 65, MaxPressure = 0.008, Volcanism = [ "carbon dioxide", "methane" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "NeonRich" ], MinGravity = 0.025, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 65, MinPressure = 0.005, Volcanism = [ "carbon dioxide", "methane" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.2, MaxGravity = 0.3, MinTemperature = 60, MaxTemperature = 70, Volcanism = [ "carbon dioxide", "methane" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.27, MaxGravity = 0.4, MinTemperature = 150, MaxTemperature = 220, MinPressure = 0.01, Volcanism = [ "carbon dioxide", "methane" ] },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_04_Name;", "Bacterium Acies", 1000000, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Neon" ], MinGravity = 0.255, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 61, MaxPressure = 0.01 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_05_Name;", "Bacterium Vesicula", 1000000, new PredRule[]
    {
        new PredRule { Atmosphere = [ "Argon" ], MinGravity = 0.027, MaxGravity = 0.51, MinTemperature = 50, MaxTemperature = 245 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_06_Name;", "Bacterium Alcyoneum", 1658500, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.376, MinTemperature = 152, MaxTemperature = 177, MaxPressure = 0.0135 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_07_Name;", "Bacterium Tela", 1949000, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Argon" ], MinGravity = 0.045, MaxGravity = 0.45, MinTemperature = 50, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "ArgonRich" ], MinGravity = 0.24, MaxGravity = 0.45, MinTemperature = 50, MaxTemperature = 150, MaxPressure = 0.05, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "Ammonia" ], MinGravity = 0.025, MaxGravity = 0.23, MinTemperature = 165, MaxTemperature = 177, MinPressure = 0.0025, MaxPressure = 0.02, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.45, MaxGravity = 0.61, MinTemperature = 300, MinPressure = 0.006, Volcanism = [ "None" ] },
        new PredRule { Atmosphere = [ "CarbonDioxide", "CarbonDioxideRich" ], MinGravity = 0.025, MaxGravity = 0.61, MinTemperature = 167, MinPressure = 0.006, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Helium" ], MinGravity = 0.025, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 21, MinPressure = 0.067, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky body", "High metal content body" ], Atmosphere = [ "Methane" ], MinGravity = 0.026, MaxGravity = 0.126, MinTemperature = 80, MaxTemperature = 109, MinPressure = 0.012, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Neon" ], MinGravity = 0.27, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 95, MaxPressure = 0.008, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "NeonRich" ], MinGravity = 0.27, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 95, MinPressure = 0.003, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "Nitrogen" ], MinGravity = 0.21, MaxGravity = 0.35, MinTemperature = 55, MaxTemperature = 80, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "Oxygen" ], MinGravity = 0.23, MaxGravity = 0.5, MinTemperature = 150, MaxTemperature = 240, MinPressure = 0.01, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.18, MaxGravity = 0.61, MinTemperature = 148, MaxTemperature = 550, Volcanism = [ "Any" ] },
        new PredRule { Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.18, MaxGravity = 0.61, MinTemperature = 300, MaxTemperature = 550, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.5, MaxGravity = 0.55, MinTemperature = 500, MaxTemperature = 650, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.063, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "WaterRich" ], MinGravity = 0.315, MaxGravity = 0.44, MinTemperature = 190, MaxTemperature = 330, MinPressure = 0.01, Volcanism = [ "Any" ] },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_08_Name;", "Bacterium Informem", 8418000, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.05, MaxGravity = 0.6, MinTemperature = 42.5, MaxTemperature = 151, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.17, MaxGravity = 0.63, MinTemperature = 50, MaxTemperature = 90 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_09_Name;", "Bacterium Volu", 7774700, new PredRule[]
    {
        new PredRule { Atmosphere = [ "Oxygen" ], MinGravity = 0.239, MaxGravity = 0.61, MinTemperature = 143.5, MaxTemperature = 246, MinPressure = 0.013 },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_10_Name;", "Bacterium Bullaris", 1152500, new PredRule[]
    {
        new PredRule { Atmosphere = [ "Methane" ], MinGravity = 0.0245, MaxGravity = 0.35, MinTemperature = 67, MaxTemperature = 109 },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "MethaneRich" ], MinGravity = 0.44, MaxGravity = 0.6, MinTemperature = 74, MaxTemperature = 141, MinPressure = 0.01, MaxPressure = 0.05, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_11_Name;", "Bacterium Omentum", 4638900, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Argon" ], MinGravity = 0.045, MaxGravity = 0.45, MinTemperature = 50, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "ArgonRich" ], MinGravity = 0.23, MaxGravity = 0.45, MinTemperature = 80, MaxTemperature = 90, MinPressure = 0.01, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Helium" ], MinGravity = 0.4, MaxGravity = 0.51, MinTemperature = 20, MaxTemperature = 21, MinPressure = 0.065, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Methane" ], MinGravity = 0.0265, MaxGravity = 0.0455, MinTemperature = 84, MaxTemperature = 108, MinPressure = 0.035, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Neon" ], MinGravity = 0.31, MaxGravity = 0.6, MinTemperature = 20, MaxTemperature = 61, MaxPressure = 0.0065, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "NeonRich" ], MinGravity = 0.27, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 93, MinPressure = 0.0027, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.2, MaxGravity = 0.26, MinTemperature = 60, MaxTemperature = 80, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "WaterRich" ], MinGravity = 0.38, MaxGravity = 0.45, MinTemperature = 190, MaxTemperature = 330, MinPressure = 0.07, Volcanism = [ "nitrogen", "ammonia" ] },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_12_Name;", "Bacterium Cerbrus", 1689800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.042, MaxGravity = 0.605, MinTemperature = 132, MaxTemperature = 500 },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.064, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.064, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "WaterRich" ], MinGravity = 0.4, MaxGravity = 0.5, MinTemperature = 190, MaxTemperature = 330, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Bacterial_13_Name;", "Bacterium Verrata", 3897000, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "Icy body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.03, MaxGravity = 0.09, MinTemperature = 160, MaxTemperature = 180, MaxPressure = 0.0135, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky ice body", "Icy body" ], Atmosphere = [ "Argon" ], MinGravity = 0.165, MaxGravity = 0.33, MinTemperature = 57.5, MaxTemperature = 145, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "ArgonRich" ], MinGravity = 0.04, MaxGravity = 0.08, MinTemperature = 80, MaxTemperature = 90, MaxPressure = 0.01, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky ice body", "Icy body" ], Atmosphere = [ "CarbonDioxide", "CarbonDioxideRich" ], MinGravity = 0.25, MaxGravity = 0.32, MinTemperature = 167, MaxTemperature = 240, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Helium" ], MinGravity = 0.49, MaxGravity = 0.53, MinTemperature = 20, MaxTemperature = 21, MinPressure = 0.065, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky ice body", "Icy body" ], Atmosphere = [ "Neon" ], MinGravity = 0.29, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 51, MaxPressure = 0.075, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky ice body", "Icy body" ], Atmosphere = [ "NeonRich" ], MinGravity = 0.43, MaxGravity = 0.61, MinTemperature = 20, MaxTemperature = 65, MinPressure = 0.005, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.205, MaxGravity = 0.241, MinTemperature = 60, MaxTemperature = 80, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky ice body", "Icy body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.24, MaxGravity = 0.35, MinTemperature = 154, MaxTemperature = 220, MinPressure = 0.01, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.054, Volcanism = [ "water" ] },
    }),
    // ── cactoida ──
    new PredSpecies("$Codex_Ent_Cactoid_01_Name;", "Cactoida Cortexum", 3667600, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Cactoid_02_Name;", "Cactoida Lapis", 2483600, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Cactoid_03_Name;", "Cactoida Vermis", 16202800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.265, MaxGravity = 0.276, MinTemperature = 160, MaxTemperature = 210, MaxPressure = 0.005, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "water" ] },
    }),
    new PredSpecies("$Codex_Ent_Cactoid_04_Name;", "Cactoida Pullulanta", 3667600, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Cactoid_05_Name;", "Cactoida Peperatis", 2483600, new PredRule[]
    {
    }),
    // ── clypeus ──
    new PredSpecies("$Codex_Ent_Clypeus_01_Name;", "Clypeus Lacrimam", 8418000, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 190, MinPressure = 0.054, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "water" ] },
    }),
    new PredSpecies("$Codex_Ent_Clypeus_02_Name;", "Clypeus Margaritus", 11873200, new PredRule[]
    {
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 190, MaxTemperature = 197, MinPressure = 0.054, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Clypeus_03_Name;", "Clypeus Speculumi", 16202800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 190, MaxTemperature = 197, MinPressure = 0.055, Volcanism = [ "None" ], Distance = 2000 },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "None" ], Distance = 2000 },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.276, Volcanism = [ "water" ], Distance = 2000 },
    }),
    // ── concha ──
    new PredSpecies("$Codex_Ent_Conchas_01_Name;", "Concha Renibus", 4572400, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.045, MinTemperature = 176, MaxTemperature = 177, Volcanism = [ "silicate", "metallic" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 180, MinPressure = 0.025, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Methane" ], MinGravity = 0.04, MaxGravity = 0.15, MinTemperature = 78, MaxTemperature = 100, MinPressure = 0.01, Volcanism = [ "silicate", "metallic" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.65, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.65, Volcanism = [ "water" ] },
    }),
    new PredSpecies("$Codex_Ent_Conchas_02_Name;", "Concha Aureolas", 7774700, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 152, MaxTemperature = 177, MaxPressure = 0.0135 },
    }),
    new PredSpecies("$Codex_Ent_Conchas_03_Name;", "Concha Labiata", 2352400, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 150, MaxTemperature = 200, MinPressure = 0.002, Volcanism = [ "None" ] },
    }),
    // BioScan had a 2^24-1 placeholder here; actual value is 19,010,800 (Vista Genomics)
    new PredSpecies("$Codex_Ent_Conchas_04_Name;", "Concha Biconcavis", 19010800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.053, MaxGravity = 0.275, MinTemperature = 42, MaxTemperature = 52, MaxPressure = 0.0047, Volcanism = [ "None" ] },
    }),
    // ── electricae ──
    new PredSpecies("$Codex_Ent_Electricae_01_Name;", "Electricae Pluma", 6284600, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Argon", "ArgonRich" ], MinGravity = 0.025, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 150, ParentStar = [ "A", "N", "D", "H", "AeBe" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Neon", "NeonRich" ], MinGravity = 0.26, MaxGravity = 0.276, MinTemperature = 20, MaxTemperature = 70, MaxPressure = 0.005, ParentStar = [ "A", "N", "D", "H", "AeBe" ] },
    }),
    new PredSpecies("$Codex_Ent_Electricae_02_Name;", "Electricae Radialem", 6284600, new PredRule[]
    {
    }),
    // ── fonticulua ──
    new PredSpecies("$Codex_Ent_Fonticulus_01_Name;", "Fonticulua Segmentatus", 19010800, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Neon", "NeonRich" ], MinGravity = 0.25, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 75, MaxPressure = 0.006, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Fonticulus_02_Name;", "Fonticulua Campestris", 1000000, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Argon" ], MinGravity = 0.027, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 150 },
    }),
    new PredSpecies("$Codex_Ent_Fonticulus_03_Name;", "Fonticulua Upupam", 5727600, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "ArgonRich" ], MinGravity = 0.209, MaxGravity = 0.276, MinTemperature = 61, MaxTemperature = 125, MinPressure = 0.0175 },
    }),
    new PredSpecies("$Codex_Ent_Fonticulus_04_Name;", "Fonticulua Lapida", 3111000, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.19, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 81 },
    }),
    new PredSpecies("$Codex_Ent_Fonticulus_05_Name;", "Fonticulua Fluctus", 20000000, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.235, MaxGravity = 0.276, MinTemperature = 143, MaxTemperature = 200, MinPressure = 0.012 },
    }),
    new PredSpecies("$Codex_Ent_Fonticulus_06_Name;", "Fonticulua Digitos", 1804100, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Methane" ], MinGravity = 0.025, MaxGravity = 0.07, MinTemperature = 83, MaxTemperature = 109, MinPressure = 0.03 },
    }),
    // ── frutexa ──
    new PredSpecies("$Codex_Ent_Shrubs_01_Name;", "Frutexa Flabellum", 1808900, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Shrubs_02_Name;", "Frutexa Acus", 7774700, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Shrubs_03_Name;", "Frutexa Metallicum", 1632500, new PredRule[]
    {
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 152, MaxTemperature = 176, MaxPressure = 0.01, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 146, MaxTemperature = 197, MinPressure = 0.002, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Methane" ], MinGravity = 0.05, MaxGravity = 0.1, MinTemperature = 100, MaxTemperature = 300 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.07, MaxTemperature = 400, MaxPressure = 0.07, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Shrubs_04_Name;", "Frutexa Flammasis", 10326000, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Shrubs_05_Name;", "Frutexa Fera", 1632500, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Shrubs_06_Name;", "Frutexa Sponsae", 5988000, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.056, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.056, Volcanism = [ "water" ] },
    }),
    new PredSpecies("$Codex_Ent_Shrubs_07_Name;", "Frutexa Collum", 1639800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 132, MaxTemperature = 215, MaxPressure = 0.004 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.265, MaxGravity = 0.276, MinTemperature = 132, MaxTemperature = 135, MaxPressure = 0.004, Volcanism = [ "None" ] },
    }),
    // ── fumerola ──
    new PredSpecies("$Codex_Ent_Fumerolas_01_Name;", "Fumerola Carbosis", 6284600, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Argon" ], MinGravity = 0.168, MaxGravity = 0.276, MinTemperature = 57, MaxTemperature = 150, Volcanism = [ "carbon", "methane" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Methane" ], MinGravity = 0.025, MaxGravity = 0.047, MinTemperature = 84, MaxTemperature = 110, MinPressure = 0.03, Volcanism = [ "methane magma" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Neon" ], MinGravity = 0.26, MaxGravity = 0.276, MinTemperature = 40, MaxTemperature = 60, Volcanism = [ "carbon", "methane" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.2, MaxGravity = 0.276, MinTemperature = 57, MaxTemperature = 70, Volcanism = [ "carbon", "methane" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.26, MaxGravity = 0.276, MinTemperature = 160, MaxTemperature = 180, Volcanism = [ "carbon" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.185, MaxGravity = 0.276, MinTemperature = 149, MaxTemperature = 272, Volcanism = [ "carbon", "methane" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Ammonia", "ArgonRich", "CarbonDioxideRich" ], MaxGravity = 0.276, Volcanism = [ "carbon" ] },
    }),
    new PredSpecies("$Codex_Ent_Fumerolas_02_Name;", "Fumerola Extremus", 16202800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.09, MinTemperature = 161, MaxTemperature = 177, MaxPressure = 0.0135, Volcanism = [ "silicate", "metallic", "rocky" ] },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Argon" ], MinGravity = 0.07, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 121, Volcanism = [ "silicate", "metallic", "rocky" ] },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Methane" ], MinGravity = 0.025, MaxGravity = 0.127, MinTemperature = 77, MaxTemperature = 109, MinPressure = 0.01, Volcanism = [ "silicate", "metallic", "rocky" ] },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.07, MaxGravity = 0.276, MinTemperature = 54, MaxTemperature = 210, Volcanism = [ "silicate", "metallic", "rocky" ] },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.05, MaxGravity = 0.276, MinTemperature = 500, Volcanism = [ "silicate", "metallic", "rocky" ] },
    }),
    new PredSpecies("$Codex_Ent_Fumerolas_03_Name;", "Fumerola Nitris", 7500900, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Neon" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 30, MaxTemperature = 129, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Argon", "ArgonRich", "NeonRich" ], MinGravity = 0.044, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 141, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Methane" ], MinGravity = 0.025, MaxGravity = 0.1, MinTemperature = 83, MaxTemperature = 109, Volcanism = [ "nitrogen" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.21, MaxGravity = 0.276, MinTemperature = 60, MaxTemperature = 81, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], MaxGravity = 0.276, MinTemperature = 150, Volcanism = [ "nitrogen", "ammonia" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.21, MaxGravity = 0.276, MinTemperature = 160, MaxTemperature = 250, Volcanism = [ "nitrogen", "ammonia" ] },
    }),
    new PredSpecies("$Codex_Ent_Fumerolas_04_Name;", "Fumerola Aquatis", 6284600, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky ice body", "Rocky body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.028, MaxGravity = 0.276, MinTemperature = 161, MaxTemperature = 177, MinPressure = 0.002, MaxPressure = 0.02, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "Argon", "ArgonRich" ], MinGravity = 0.166, MaxGravity = 0.276, MinTemperature = 57, MaxTemperature = 150, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.25, MaxGravity = 0.276, MinTemperature = 160, MaxTemperature = 180, MinPressure = 0.01, MaxPressure = 0.03, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Methane" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 80, MaxTemperature = 100, MinPressure = 0.01, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Neon" ], MinGravity = 0.26, MaxGravity = 0.276, MinTemperature = 20, MaxTemperature = 60, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.195, MaxGravity = 0.245, MinTemperature = 56, MaxTemperature = 80, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.23, MaxGravity = 0.276, MinTemperature = 153, MaxTemperature = 190, MinPressure = 0.01, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body", "Rocky body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.18, MaxGravity = 0.276, MinTemperature = 150, MaxTemperature = 270, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.06, Volcanism = [ "water" ] },
    }),
    // ── fungoida ──
    new PredSpecies("$Codex_Ent_Fungoids_01_Name;", "Fungoida Setisis", 1670100, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 152, MaxTemperature = 177, MaxPressure = 0.0135 },
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "Methane" ], MinGravity = 0.033, MaxGravity = 0.276, MinTemperature = 68, MaxTemperature = 109, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Methane" ], MinGravity = 0.033, MaxGravity = 0.276, MinTemperature = 67, MaxTemperature = 109 },
    }),
    new PredSpecies("$Codex_Ent_Fungoids_02_Name;", "Fungoida Stabitis", 2680300, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Fungoids_03_Name;", "Fungoida Bullarum", 3703200, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Argon" ], MinGravity = 0.058, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 129, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.155, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 70, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Fungoids_04_Name;", "Fungoida Gelata", 3330300, new PredRule[]
    {
    }),
    // ── osseus ──
    new PredSpecies("$Codex_Ent_Osseus_01_Name;", "Osseus Fractus", 4027800, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Osseus_02_Name;", "Osseus Discus", 12934900, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.088, MinTemperature = 161, MaxTemperature = 177, MaxPressure = 0.0135, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "Argon" ], MinGravity = 0.2, MaxGravity = 0.276, MinTemperature = 65, MaxTemperature = 120, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.026, MaxGravity = 0.276, MinTemperature = 500, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Methane" ], MinGravity = 0.04, MaxGravity = 0.127, MinTemperature = 80, MaxTemperature = 110, MinPressure = 0.012, Volcanism = [ "Any" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.055 },
    }),
    new PredSpecies("$Codex_Ent_Osseus_03_Name;", "Osseus Spiralis", 2404700, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 160, MaxTemperature = 177, MaxPressure = 0.0135 },
    }),
    new PredSpecies("$Codex_Ent_Osseus_04_Name;", "Osseus Pumice", 3156300, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Argon" ], MinGravity = 0.059, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 135, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "Argon" ], MinGravity = 0.059, MaxGravity = 0.276, MinTemperature = 50, MaxTemperature = 135, Volcanism = [ "water", "geysers" ] },
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "ArgonRich" ], MinGravity = 0.035, MaxGravity = 0.276, MinTemperature = 60, MaxTemperature = 80.5, MinPressure = 0.03, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Methane" ], MinGravity = 0.033, MaxGravity = 0.276, MinTemperature = 67, MaxTemperature = 109 },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body", "High metal content body" ], Atmosphere = [ "Nitrogen" ], MinGravity = 0.05, MaxGravity = 0.276, MinTemperature = 42, MaxTemperature = 70.1, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Osseus_05_Name;", "Osseus Cornibus", 1483000, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Osseus_06_Name;", "Osseus Pellebantus", 9739000, new PredRule[]
    {
    }),
    // ── recepta ──
    new PredSpecies("$Codex_Ent_Recepta_01_Name;", "Recepta Umbrux", 12934900, new PredRule[]
    {
        new PredRule { Atmosphere = [ "CarbonDioxide" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 151, MaxTemperature = 200 },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.23, MaxGravity = 0.276, MinTemperature = 154, MaxTemperature = 175, MinPressure = 0.01, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.23, MaxGravity = 0.276, MinTemperature = 154, MaxTemperature = 175, MinPressure = 0.01, Volcanism = [ "water" ] },
        new PredRule { Atmosphere = [ "SulphurDioxide" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 132, MaxTemperature = 273 },
    }),
    new PredSpecies("$Codex_Ent_Recepta_02_Name;", "Recepta Deltahedronix", 16202800, new PredRule[]
    {
        new PredRule { Atmosphere = [ "CarbonDioxide" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 150, MaxTemperature = 195, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Icy body", "Rocky ice body" ], Atmosphere = [ "CarbonDioxide" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 150, MaxTemperature = 195, Volcanism = [ "water" ] },
        new PredRule { Atmosphere = [ "SulphurDioxide" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 132, MaxTemperature = 272 },
    }),
    new PredSpecies("$Codex_Ent_Recepta_03_Name;", "Recepta Conditivus", 14313700, new PredRule[]
    {
        new PredRule { BodyType = [ "Icy body", "Rocky body", "High metal content body" ], Atmosphere = [ "CarbonDioxide", "CarbonDioxideRich" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 150, MaxTemperature = 195, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.23, MaxGravity = 0.276, MinTemperature = 154, MaxTemperature = 175, MinPressure = 0.01, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Icy body" ], Atmosphere = [ "Oxygen" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.23, MaxGravity = 0.276, MinTemperature = 154, MaxTemperature = 175, MinPressure = 0.01, Volcanism = [ "water" ] },
        new PredRule { Atmosphere = [ "SulphurDioxide" ], AtmosphereComponent = new() { ["SulphurDioxide"] = 1.05 }, MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 132, MaxTemperature = 275 },
    }),
    // ── stratum ──
    new PredSpecies("$Codex_Ent_Stratum_01_Name;", "Stratum Excutitus", 2448900, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Stratum_02_Name;", "Stratum Paleas", 1362000, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.35, MinTemperature = 165, MaxTemperature = 177, MaxPressure = 0.0135 },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.04, MaxGravity = 0.585, MinTemperature = 165, MaxTemperature = 395, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "CarbonDioxideRich" ], MinGravity = 0.43, MaxGravity = 0.585, MinTemperature = 185, MaxTemperature = 260, MinPressure = 0.015, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.056, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.056, MinPressure = 0.065, Volcanism = [ "water" ] },
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.39, MaxGravity = 0.59, MinTemperature = 165, MaxTemperature = 250, MinPressure = 0.022 },
    }),
    new PredSpecies("$Codex_Ent_Stratum_03_Name;", "Stratum Laminamus", 2788300, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Stratum_04_Name;", "Stratum Araneamus", 2448900, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.26, MaxGravity = 0.57, MinTemperature = 165, MaxTemperature = 373 },
    }),
    new PredSpecies("$Codex_Ent_Stratum_05_Name;", "Stratum Limaxus", 1362000, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Stratum_06_Name;", "Stratum Cucumisis", 16202800, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Stratum_07_Name;", "Stratum Tectonicas", 19010800, new PredRule[]
    {
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.045, MaxGravity = 0.38, MinTemperature = 165, MaxTemperature = 177 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Argon", "ArgonRich" ], MinGravity = 0.485, MaxGravity = 0.54, MinTemperature = 167, MaxTemperature = 199, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.045, MaxGravity = 0.61, MinTemperature = 165, MaxTemperature = 430 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxideRich" ], MinGravity = 0.035, MaxGravity = 0.61, MinTemperature = 165, MaxTemperature = 260 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Oxygen" ], MinGravity = 0.4, MaxGravity = 0.52, MinTemperature = 165, MaxTemperature = 246 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.29, MaxGravity = 0.62, MinTemperature = 165, MaxTemperature = 450 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.045, MaxGravity = 0.063, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Stratum_08_Name;", "Stratum Frigus", 2637500, new PredRule[]
    {
    }),
    // ── tubus ──
    new PredSpecies("$Codex_Ent_Tubus_01_Name;", "Tubus Conifer", 2415500, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tubus_02_Name;", "Tubus Sororibus", 5727600, new PredRule[]
    {
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.045, MaxGravity = 0.152, MinTemperature = 160, MaxTemperature = 177, MaxPressure = 0.0135 },
        new PredRule { BodyType = [ "High metal content body" ], Atmosphere = [ "CarbonDioxide" ], MinGravity = 0.045, MaxGravity = 0.152, MinTemperature = 160, MaxTemperature = 195, Volcanism = [ "None" ] },
    }),
    new PredSpecies("$Codex_Ent_Tubus_03_Name;", "Tubus Cavas", 11873200, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tubus_04_Name;", "Tubus Rosarium", 2637500, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body" ], Atmosphere = [ "Ammonia" ], MinGravity = 0.04, MaxGravity = 0.153, MinTemperature = 160, MaxTemperature = 177, MaxPressure = 0.0135 },
    }),
    new PredSpecies("$Codex_Ent_Tubus_05_Name;", "Tubus Compagibus", 7774700, new PredRule[]
    {
    }),
    // ── tussock ──
    new PredSpecies("$Codex_Ent_Tussocks_01_Name;", "Tussock Pennata", 5853800, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_02_Name;", "Tussock Ventusa", 3227700, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_03_Name;", "Tussock Ignis", 1849000, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_04_Name;", "Tussock Cultro", 1766600, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_05_Name;", "Tussock Catena", 1766600, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_06_Name;", "Tussock Pennatis", 1000000, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_07_Name;", "Tussock Serrati", 4447100, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_08_Name;", "Tussock Albata", 3252500, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_09_Name;", "Tussock Propagito", 1000000, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_10_Name;", "Tussock Divisa", 1766600, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_11_Name;", "Tussock Caputus", 3472400, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_12_Name;", "Tussock Triticum", 7774700, new PredRule[]
    {
    }),
    new PredSpecies("$Codex_Ent_Tussocks_13_Name;", "Tussock Stigmasis", 19010800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "SulphurDioxide" ], MinGravity = 0.04, MaxGravity = 0.276, MinTemperature = 132, MaxTemperature = 180, MaxPressure = 0.01 },
    }),
    new PredSpecies("$Codex_Ent_Tussocks_14_Name;", "Tussock Virgam", 14313700, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.065, Volcanism = [ "None" ] },
        new PredRule { BodyType = [ "Rocky body", "High metal content body" ], Atmosphere = [ "Water" ], MinGravity = 0.04, MaxGravity = 0.065, Volcanism = [ "water" ] },
    }),
    new PredSpecies("$Codex_Ent_Tussocks_15_Name;", "Tussock Capillum", 7025800, new PredRule[]
    {
        new PredRule { BodyType = [ "Rocky ice body" ], Atmosphere = [ "Argon" ], MinGravity = 0.22, MaxGravity = 0.276, MinTemperature = 80, MaxTemperature = 129 },
        new PredRule { BodyType = [ "Rocky body", "Rocky ice body" ], Atmosphere = [ "Methane" ], MinGravity = 0.033, MaxGravity = 0.276, MinTemperature = 80, MaxTemperature = 110 },
    }),
        ];
    }
}
