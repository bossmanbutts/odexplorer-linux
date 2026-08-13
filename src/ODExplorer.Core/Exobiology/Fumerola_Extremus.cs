using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fumerola_Extremus : SpeciesBase
{
	public override string Genus => "Fumerola";

	public override string SpeciesCodex => "$Codex_Ent_Fumerolas_02_Name;";

	public override string SpeciesName => "Extremus";

	public override double MinGravity => 0.04;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 0.0;

	public override double MaxTemperature => 0.0;

	public override double MinDistance => 0.0;

	public override double MaxDistance => 0.0;

	public override List<PlanetClass> Planets
	{
		get
		{
			int num = 3;
			List<PlanetClass> list = new List<PlanetClass>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<PlanetClass> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = PlanetClass.RockyBody;
			num2++;
			span[num2] = PlanetClass.HighMetalContentBody;
			num2++;
			span[num2] = PlanetClass.RockyIceBody;
			return list;
		}
	}

	public override List<AtmosphereDescription> Atmospheres => new List<AtmosphereDescription>();

	public override List<Volcanism> Volcanism
	{
		get
		{
			int num = 9;
			List<Volcanism> list = new List<Volcanism>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Volcanism> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = EliteJournalReader.Volcanism.minor_silicate_vapour_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_metallic_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_rocky_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_silicate_vapour_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.metallic_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.rocky_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_rocky_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_metallic_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.silicate_vapour_geysers_volcanism;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 6;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_02_Cadmium_Name;", "Fumerola Extremus - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_02_Molybdenum_Name;", "Fumerola Extremus - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_02_Mercury_Name;", "Fumerola Extremus - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_02_Tungsten_Name;", "Fumerola Extremus - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_02_Tin_Name;", "Fumerola Extremus - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_02_Niobium_Name;", "Fumerola Extremus - White", VariantColours.White, StarType.Unknown, PlanetMaterial.niobium);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		string text = planet.Atmosphere.ToString();
		if (!text.Contains("thin", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		if (!Volcanism.Contains(planet.Volcanism))
		{
			return null;
		}
		if (!MathHelpers.DoubleBetweenMinEquals(MinGravity, MaxGravity, planet.SurfaceGravity))
		{
			return null;
		}
		if (!Planets.Contains(planet.PlanetClass))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
