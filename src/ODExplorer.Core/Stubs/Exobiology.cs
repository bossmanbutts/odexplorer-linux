// Functional in-memory ODUtils.Exobiology implementation with an embedded
// species/genus table. Species lists, values and colony ranges are generated
// from the BioScan rulesets (bio_scan/bio_data/rulesets/*.py) and cross-checked
// against Vista Genomics data; the schema matches the real ODUtils.Exobiology
// surface so the real package can be swapped in later.

using System;
using System.Collections.Generic;
using System.Linq;
using ODUtils.Models;

namespace ODUtils.Exobiology
{
    public sealed class OrganicInfo
    {
        public string EnglishName { get; set; } = string.Empty;
        public long Value { get; set; }
        public int ColonyRange { get; set; }
    }

    public static class OrganicValues
    {
        public static readonly DateTime NewPriceDate = new(2023, 4, 25, 0, 0, 0, DateTimeKind.Utc);
    }

    public sealed class ExoNames
    {
        public string Genus { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Variant { get; set; } = string.Empty;
        public string GenusCodex { get; set; } = string.Empty;
        public string SpeciesCodex { get; set; } = string.Empty;
    }

    public sealed class ExoVariant
    {
        public string Codex { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public VariantColours Colour { get; set; }
    }

    public sealed class ExoSpecies
    {
        public string SpeciesCodex { get; set; } = string.Empty;
        public string SpeciesName { get; set; } = string.Empty;
        public string GenusName { get; set; } = string.Empty;
        public List<GalacticRegions> Regions { get; set; } = [];
        public List<ExoVariant> Variants { get; set; } = [];
        public long Value { get; set; }
        public int ColonyRange { get; set; }

        public bool IsAvailableIn(GalacticRegions region) => Regions.Contains(region);
    }

    public sealed class ExoGenus
    {
        public string Codex { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public List<ExoSpecies> Species { get; set; } = [];
    }

    public sealed class ExoData
    {
        // Best-effort variant palette. Exact letter→colour mappings are per-species in
        // the real ODUtils data; these are approximations for parity v1.
        private static readonly (string letter, VariantColours colour)[] palette =
        [
            ("A", VariantColours.Amethyst), ("B", VariantColours.Aquamarine), ("C", VariantColours.Blue),
            ("D", VariantColours.Cobalt), ("E", VariantColours.Emerald), ("F", VariantColours.Gold),
            ("G", VariantColours.Green), ("H", VariantColours.Lime), ("I", VariantColours.Magenta),
            ("J", VariantColours.Mauve), ("K", VariantColours.Ocher), ("L", VariantColours.Orange),
            ("M", VariantColours.Peach), ("N", VariantColours.Red), ("O", VariantColours.Sage),
            ("P", VariantColours.Teal), ("Q", VariantColours.Turquoise), ("R", VariantColours.White),
            ("S", VariantColours.Yellow)
        ];

        private static readonly List<ExoGenus> genusTable = BuildGenusTable();
        private static readonly Dictionary<string, ExoSpecies> speciesByCodex = genusTable
            .SelectMany(x => x.Species)
            .ToDictionary(x => x.SpeciesCodex, x => x);

        public static readonly Dictionary<string, List<GalacticRegions>> SpeciesRegions = speciesByCodex
            .ToDictionary(x => x.Key, x => x.Value.Regions);

        public List<ExoGenus> AllGenus { get; } = genusTable;

        public void Initialise() { }

        public static ExoNames? GetNamesFromSpecies(string speciesCodex)
        {
            if (speciesByCodex.TryGetValue(speciesCodex, out var species))
            {
                return new ExoNames
                {
                    Genus = species.GenusName,
                    Species = species.SpeciesName,
                    GenusCodex = $"$Codex_Ent_{speciesCodex.Split('_')[2]}_Genus_Name;",
                    SpeciesCodex = speciesCodex
                };
            }
            return null;
        }

        public static ExoNames? GetNames(string variantCodex)
        {
            // $Codex_Ent_<Genus>_<NN>_<Letter>_Name;  →  "$Codex","Ent",<Genus>,<NN>,<Letter>,"Name;"
            if (string.IsNullOrEmpty(variantCodex))
                return null;

            var parts = variantCodex.Split('_');
            if (parts.Length < 6)
                return null;

            var speciesCodex = $"$Codex_Ent_{parts[2]}_{parts[3]}_Name;";
            if (!speciesByCodex.TryGetValue(speciesCodex, out var species))
                return null;

            return new ExoNames
            {
                Genus = species.GenusName,
                Species = species.SpeciesName,
                Variant = $"{species.SpeciesName} {parts[4]}",
                GenusCodex = $"$Codex_Ent_{parts[2]}_Genus_Name;",
                SpeciesCodex = speciesCodex
            };
        }

        public OrganicInfo? GetInfo(string speciesCodex)
        {
            if (speciesByCodex.TryGetValue(speciesCodex, out var species))
            {
                return new OrganicInfo { EnglishName = species.SpeciesName, Value = species.Value, ColonyRange = species.ColonyRange };
            }
            return null;
        }

        private static List<ExoGenus> BuildGenusTable()
        {
            var regions = new List<GalacticRegions> { GalacticRegions.Core, GalacticRegions.Bubble, GalacticRegions.OuterRim };

            var ret = new List<ExoGenus>();

            void AddGenus(string token, string english, (int number, string name, int value, int range)[] species)
            {
                var genus = new ExoGenus
                {
                    Codex = $"$Codex_Ent_{token}_Genus_Name;",
                    EnglishName = english,
                    Species = species.Select(s => new ExoSpecies
                    {
                        SpeciesCodex = $"$Codex_Ent_{token}_{s.number:D2}_Name;",
                        SpeciesName = s.name,
                        GenusName = english,
                        Regions = regions,
                        Value = s.value,
                        ColonyRange = s.range,
                        Variants = BuildVariants(token, s.number, s.name)
                    }).ToList()
                };
                ret.Add(genus);
            }

            AddGenus("Aleoids", "Aleoida", new[]
            {
                (01, "Aleoida Arcus", 7252500, 25),
                (02, "Aleoida Coronamus", 6284600, 25),
                (03, "Aleoida Spica", 3385200, 25),
                (04, "Aleoida Laminiae", 3385200, 25),
                (05, "Aleoida Gravis", 12934900, 25)
            });
            AddGenus("Bacterial", "Bacterium", new[]
            {
                (01, "Bacterium Aurasus", 1000000, 2),
                (02, "Bacterium Nebulus", 5289900, 2),
                (03, "Bacterium Scopulum", 4934500, 2),
                (04, "Bacterium Acies", 1000000, 2),
                (05, "Bacterium Vesicula", 1000000, 2),
                (06, "Bacterium Alcyoneum", 1658500, 2),
                (07, "Bacterium Tela", 1949000, 2),
                (08, "Bacterium Informem", 8418000, 2),
                (09, "Bacterium Volu", 7774700, 2),
                (10, "Bacterium Bullaris", 1152500, 2),
                (11, "Bacterium Omentum", 4638900, 2),
                (12, "Bacterium Cerbrus", 1689800, 2),
                (13, "Bacterium Verrata", 3897000, 2)
            });
            AddGenus("Cactoid", "Cactoida", new[]
            {
                (01, "Cactoida Cortexum", 3667600, 30),
                (02, "Cactoida Lapis", 2483600, 30),
                (03, "Cactoida Vermis", 16202800, 30),
                (04, "Cactoida Pullulanta", 3667600, 30),
                (05, "Cactoida Peperatis", 2483600, 30)
            });
            AddGenus("Clypeus", "Clypeus", new[]
            {
                (01, "Clypeus Lacrimam", 8418000, 25),
                (02, "Clypeus Margaritus", 11873200, 25),
                (03, "Clypeus Speculumi", 16202800, 25)
            });
            AddGenus("Conchas", "Concha", new[]
            {
                (01, "Concha Renibus", 4572400, 25),
                (02, "Concha Aureolas", 7774700, 25),
                (03, "Concha Labiata", 2352400, 25),
                (04, "Concha Biconcavis", 19010800, 25)
            });
            AddGenus("Electricae", "Electricae", new[]
            {
                (01, "Electricae Pluma", 6284600, 15),
                (02, "Electricae Radialem", 6284600, 15)
            });
            AddGenus("Fonticulus", "Fonticulua", new[]
            {
                (01, "Fonticulua Segmentatus", 19010800, 10),
                (02, "Fonticulua Campestris", 1000000, 10),
                (03, "Fonticulua Upupam", 5727600, 10),
                (04, "Fonticulua Lapida", 3111000, 10),
                (05, "Fonticulua Fluctus", 20000000, 10),
                (06, "Fonticulua Digitos", 1804100, 10)
            });
            AddGenus("Shrubs", "Frutexa", new[]
            {
                (01, "Frutexa Flabellum", 1808900, 25),
                (02, "Frutexa Acus", 7774700, 25),
                (03, "Frutexa Metallicum", 1632500, 25),
                (04, "Frutexa Flammasis", 10326000, 25),
                (05, "Frutexa Fera", 1632500, 25),
                (06, "Frutexa Sponsae", 5988000, 25),
                (07, "Frutexa Collum", 1639800, 25)
            });
            AddGenus("Fumerolas", "Fumerola", new[]
            {
                (01, "Fumerola Carbosis", 6284600, 25),
                (02, "Fumerola Extremus", 16202800, 25),
                (03, "Fumerola Nitris", 7500900, 25),
                (04, "Fumerola Aquatis", 6284600, 25)
            });
            AddGenus("Fungoids", "Fungoida", new[]
            {
                (01, "Fungoida Setisis", 1670100, 30),
                (02, "Fungoida Stabitis", 2680300, 30),
                (03, "Fungoida Bullarum", 3703200, 30),
                (04, "Fungoida Gelata", 3330300, 30)
            });
            AddGenus("Osseus", "Osseus", new[]
            {
                (01, "Osseus Fractus", 4027800, 25),
                (02, "Osseus Discus", 12934900, 25),
                (03, "Osseus Spiralis", 2404700, 25),
                (04, "Osseus Pumice", 3156300, 25),
                (05, "Osseus Cornibus", 1483000, 25),
                (06, "Osseus Pellebantus", 9739000, 25)
            });
            AddGenus("Recepta", "Recepta", new[]
            {
                (01, "Recepta Umbrux", 12934900, 25),
                (02, "Recepta Deltahedronix", 16202800, 25),
                (03, "Recepta Conditivus", 14313700, 25)
            });
            AddGenus("Stratum", "Stratum", new[]
            {
                (01, "Stratum Excutitus", 2448900, 25),
                (02, "Stratum Paleas", 1362000, 25),
                (03, "Stratum Laminamus", 2788300, 25),
                (04, "Stratum Araneamus", 2448900, 25),
                (05, "Stratum Limaxus", 1362000, 25),
                (06, "Stratum Cucumisis", 16202800, 25),
                (07, "Stratum Tectonicas", 19010800, 25),
                (08, "Stratum Frigus", 2637500, 25)
            });
            AddGenus("Tubus", "Tubus", new[]
            {
                (01, "Tubus Conifer", 2415500, 15),
                (02, "Tubus Sororibus", 5727600, 15),
                (03, "Tubus Cavas", 11873200, 15),
                (04, "Tubus Rosarium", 2637500, 15),
                (05, "Tubus Compagibus", 7774700, 15)
            });
            AddGenus("Tussocks", "Tussock", new[]
            {
                (01, "Tussock Pennata", 5853800, 25),
                (02, "Tussock Ventusa", 3227700, 25),
                (03, "Tussock Ignis", 1849000, 25),
                (04, "Tussock Cultro", 1766600, 25),
                (05, "Tussock Catena", 1766600, 25),
                (06, "Tussock Pennatis", 1000000, 25),
                (07, "Tussock Serrati", 4447100, 25),
                (08, "Tussock Albata", 3252500, 25),
                (09, "Tussock Propagito", 1000000, 25),
                (10, "Tussock Divisa", 1766600, 25),
                (11, "Tussock Caputus", 3472400, 25),
                (12, "Tussock Triticum", 7774700, 25),
                (13, "Tussock Stigmasis", 19010800, 25),
                (14, "Tussock Virgam", 14313700, 25),
                (15, "Tussock Capillum", 7025800, 25)
            });

            return ret;
        }

        private static List<ExoVariant> BuildVariants(string token, int number, string speciesName)
        {
            var ret = new List<ExoVariant>();
            foreach (var (letter, colour) in palette)
            {
                ret.Add(new ExoVariant
                {
                    Codex = $"$Codex_Ent_{token}_{number:D2}_{letter}_Name;",
                    EnglishName = $"{speciesName} {colour}",
                    Colour = colour
                });
            }
            return ret;
        }
    }
}
