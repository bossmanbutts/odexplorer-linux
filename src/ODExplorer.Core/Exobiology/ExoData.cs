using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EliteJournalReader;

namespace ODUtils.Exobiology;

public sealed class ExoData
{
	public static readonly DateTime NewPriceDate = new DateTime(2022, 11, 29, 7, 0, 0);

	public readonly List<GenusBase> genuses = new List<GenusBase>();

	private static readonly Dictionary<string, ExoEnglishNames> variantCodexToEnglishName = new Dictionary<string, ExoEnglishNames>();

	private static readonly Dictionary<string, Variant> variantInfo = new Dictionary<string, Variant>();

	public static Dictionary<string, List<GalacticRegions>> SpeciesRegions { get; private set; } = new Dictionary<string, List<GalacticRegions>>();

	internal static List<GalacticRegions> AllRegions
	{
		get
		{
			int num = 42;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_1;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_2;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_3;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_4;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_5;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_6;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_7;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_8;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_9;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_10;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_11;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_12;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_13;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_14;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_15;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_16;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_17;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_18;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_19;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_20;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_21;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_22;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_23;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_24;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_25;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_26;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_27;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_28;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_29;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_30;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_31;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_32;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_33;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_34;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_35;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_36;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_37;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_38;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_39;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_40;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_41;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_42;
			return list;
		}
	}

	public List<GenusBase> AllGenus => genuses;

	public void Initialise()
	{
		genuses.Clear();
		variantCodexToEnglishName.Clear();
		SpeciesRegions.Clear();
		IEnumerable<Type> source = from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetTypes())
			where typeof(GenusBase).IsAssignableFrom(type)
			where !type.IsAbstract && !type.IsGenericTypeDefinition && !type.IsInterface
			select type;
		List<GenusBase> list = source.Select((Type x) => Activator.CreateInstance(x) as GenusBase).ToList();
		IEnumerable<Type> source2 = from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetTypes())
			where typeof(SpeciesBase).IsAssignableFrom(type)
			where !type.IsAbstract && !type.IsGenericTypeDefinition && !type.IsInterface
			select type;
		List<SpeciesBase> allSpecies = source2.Select((Type x) => Activator.CreateInstance(x) as SpeciesBase).ToList();
		foreach (GenusBase item in list)
		{
			if (item == null)
			{
				continue;
			}
			genuses.Add(item);
			item.Initialise(allSpecies);
			foreach (SpeciesBase item2 in item.Species)
			{
				if (item2 == null)
				{
					continue;
				}
				SpeciesRegions.TryAdd(item2.SpeciesCodex, item2.Regions);
				foreach (Variant variant in item2.Variants)
				{
					if (variant != null)
					{
						variantCodexToEnglishName.TryAdd(variant.Codex, new ExoEnglishNames(item.Genus, item.Codex, item2.SpeciesName, item2.SpeciesCodex, variant.Colour.ToString(), variant.Codex));
						variantInfo.TryAdd(variant.Codex, variant);
					}
				}
			}
		}
	}

	public Dictionary<string, List<ExoPrediction>> GetPredictions(ExoPlanet planet)
	{
		Dictionary<string, List<ExoPrediction>> dictionary = new Dictionary<string, List<ExoPrediction>>();
		foreach (GenusBase genuse in genuses)
		{
			List<ExoPrediction> exoPredictions = genuse.GetExoPredictions(planet);
			if (exoPredictions != null && exoPredictions.Count > 0)
			{
				if (dictionary.TryGetValue(genuse.Genus, out var value))
				{
					value.AddRange(exoPredictions);
				}
				else
				{
					dictionary.Add(genuse.Genus, exoPredictions);
				}
			}
		}
		return dictionary;
	}

	public static ExoEnglishNames? GetNames(string variantCodex)
	{
		if (variantCodexToEnglishName.TryGetValue(variantCodex, out ExoEnglishNames value))
		{
			return value;
		}
		return null;
	}

	public static Variant? GetVariant(string variantCodex)
	{
		if (variantInfo.TryGetValue(variantCodex, out Variant value))
		{
			return value;
		}
		return null;
	}

	public static ExoEnglishNames? GetNamesFromSpecies(string speciesCodex)
	{
		return variantCodexToEnglishName.Values.FirstOrDefault((ExoEnglishNames x) => x.SpeciesCodex == speciesCodex);
	}
}
