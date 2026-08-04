// Functional in-memory ODUtils.Exobiology implementation with an embedded
// species/genus table. Values, colony ranges and region availability are
// best-effort approximations for parity v1; the schema matches the real
// ODUtils.Exobiology surface so the real package can be swapped in later.

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
                (1, "Aleoida Arcadian", 300_000, 25), (2, "Aleoida Coronamus", 300_000, 25),
                (3, "Aleoida Gravis", 300_000, 25), (4, "Aleoida Laminiae", 300_000, 25),
                (5, "Aleoida Lupus", 300_000, 25), (6, "Aleoida Praecursoris", 300_000, 25),
                (7, "Aleoida Roseum", 800_000, 25)
            });
            AddGenus("Bacterial", "Bacterium", new[]
            {
                (1, "Bacterium Acerosis", 100_000, 2), (2, "Bacterium Alcyoneum", 200_000, 2),
                (3, "Bacterium Cerebrus", 200_000, 2), (4, "Bacterium Informem", 100_000, 2),
                (5, "Bacterium Nebulus", 100_000, 2), (6, "Bacterium Omentus", 100_000, 2),
                (7, "Bacterium Scopulum", 100_000, 2), (8, "Bacterium Tela", 100_000, 2),
                (9, "Bacterium Vesicula", 100_000, 2), (10, "Bacterium Volu", 100_000, 2),
                (11, "Bacterium Bullaris", 100_000, 2), (12, "Bacterium Punctulum", 100_000, 2)
            });
            AddGenus("Cactoid", "Cactoida", new[]
            {
                (1, "Cactoida Cortexum", 200_000, 25), (2, "Cactoida Lapis", 120_000, 25),
                (3, "Cactoida Peperatus", 200_000, 25), (4, "Cactoida Pullulanta", 120_000, 25),
                (5, "Cactoida Rimula", 120_000, 25), (6, "Cactoida Vermis", 200_000, 25)
            });
            AddGenus("Clypeus", "Clypeus", new[]
            {
                (1, "Clypeus Margaritus", 600_000, 25), (2, "Clypeus Speculumi", 600_000, 25)
            });
            AddGenus("Conchas", "Concha", new[]
            {
                (1, "Concha Aureolas", 800_000, 25), (2, "Concha Bicoronata", 500_000, 25),
                (3, "Concha Labiata", 500_000, 25), (4, "Concha Renibus", 500_000, 25),
                (5, "Concha Bullarum", 500_000, 25)
            });
            AddGenus("Electricae", "Electricae", new[]
            {
                (1, "Electricae Pluma", 400_000, 15), (2, "Electricae Radialem", 1_200_000, 15),
                (3, "Electricae Alatae", 400_000, 15)
            });
            AddGenus("Fonticulus", "Fonticulua", new[]
            {
                (1, "Fonticulua Campestris", 200_000, 10), (2, "Fonticulua Digitos", 200_000, 10),
                (3, "Fonticulua Fluctus", 200_000, 10), (4, "Fonticulua Lapidem", 200_000, 10),
                (5, "Fonticulua Upupam", 700_000, 10)
            });
            AddGenus("Fumerolas", "Fumerola", new[]
            {
                (1, "Fumerola Carbosis", 500_000, 25), (2, "Fumerola Extremus", 1_000_000, 25),
                (3, "Fumerola Nitris", 500_000, 25), (4, "Fumerola Aquatis", 500_000, 25)
            });
            AddGenus("Fungoids", "Fungoida", new[]
            {
                (1, "Fungoida Setisis", 1_000_000, 30), (2, "Fungoida Albatum", 300_000, 30),
                (3, "Fungoida Gelata", 300_000, 30), (4, "Fungoida Bullarum", 300_000, 30),
                (5, "Fungoida Stabitis", 300_000, 30), (6, "Fungoida Tela", 300_000, 30),
                (7, "Fungoida Flabellum", 300_000, 30), (8, "Fungoida Magellanic", 300_000, 30),
                (9, "Fungoida Minimus", 300_000, 30), (10, "Fungoida Basilis", 300_000, 30)
            });
            AddGenus("Osseus", "Osseus", new[]
            {
                (1, "Osseus Discus", 500_000, 25), (2, "Osseus Fractus", 500_000, 25),
                (3, "Osseus Pellebantus", 500_000, 25), (4, "Osseus Spiralis", 500_000, 25),
                (5, "Osseus Turner", 500_000, 25), (6, "Osseus Pumice", 800_000, 25)
            });
            AddGenus("Recepta", "Recepta", new[]
            {
                (1, "Recepta Deltahedronis", 600_000, 25), (2, "Recepta Umbris", 600_000, 25)
            });
            AddGenus("Shrubs", "Frutexa", new[]
            {
                (1, "Frutexa Acus", 300_000, 25), (2, "Frutexa Collum", 300_000, 25),
                (3, "Frutexa Fera", 300_000, 25), (4, "Frutexa Flammasis", 300_000, 25),
                (5, "Frutexa Metallicum", 300_000, 25), (6, "Frutexa Sponsae", 300_000, 25),
                (7, "Frutexa Tessera", 300_000, 25)
            });
            AddGenus("Stratum", "Stratum", new[]
            {
                (1, "Stratum", 500_000, 25), (2, "Stratum Araneamus", 500_000, 25),
                (3, "Stratum Cucumisis", 700_000, 25), (4, "Stratum Excutitus", 500_000, 25),
                (5, "Stratum Frigus", 500_000, 25), (6, "Stratum Laminamus", 500_000, 25),
                (7, "Stratum Limaxus", 500_000, 25), (8, "Stratum Paleas", 500_000, 25),
                (9, "Stratum Serpentis", 500_000, 25), (10, "Stratum Tectonicas", 1_600_000, 25)
            });
            AddGenus("Tubus", "Tubus", new[]
            {
                (1, "Tubus Compagibus", 200_000, 15), (2, "Tubus Cavas", 200_000, 15),
                (3, "Tubus Rosarium", 800_000, 15), (4, "Tubus Conifer", 200_000, 15)
            });
            AddGenus("Tussocks", "Tussock", new[]
            {
                (1, "Tussock Albata", 300_000, 25), (2, "Tussock Capillum", 300_000, 25),
                (3, "Tussock Cultrato", 300_000, 25), (4, "Tussock Divisa", 300_000, 25),
                (5, "Tussock Ignis", 300_000, 25), (6, "Tussock Pennata", 800_000, 25),
                (7, "Tussock Propagito", 300_000, 25), (8, "Tussock Serrati", 300_000, 25),
                (9, "Tussock Ventusa", 300_000, 25), (10, "Tussock Virgam", 300_000, 25)
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
