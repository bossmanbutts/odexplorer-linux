using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fumerola_Nitris : SpeciesBase
{
	public override string Genus => "Fumerola";

	public override string SpeciesCodex => "$Codex_Ent_Fumerolas_03_Name;";

	public override string SpeciesName => "Nitris";

	public override double MinGravity => 0.025;

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
			int num = 1;
			List<PlanetClass> list = new List<PlanetClass>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<PlanetClass> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = PlanetClass.IcyBody;
			return list;
		}
	}

	public override List<AtmosphereDescription> Atmospheres => new List<AtmosphereDescription>();

	public override List<Volcanism> Volcanism
	{
		get
		{
			int num = 4;
			List<Volcanism> list = new List<Volcanism>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Volcanism> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = EliteJournalReader.Volcanism.minor_ammonia_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_nitrogen_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_nitrogen_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_nitrogen_magma_volcanism;
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
			span[num2] = new Variant("$Codex_Ent_Fumerolas_03_Tungsten_Name;", "Fumerola Nitris - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_03_Molybdenum_Name;", "Fumerola Nitris - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_03_Tin_Name;", "Fumerola Nitris - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_03_Mercury_Name;", "Fumerola Nitris - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_03_Niobium_Name;", "Fumerola Nitris - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_03_Cadmium_Name;", "Fumerola Nitris - White", VariantColours.White, StarType.Unknown, PlanetMaterial.cadmium);
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
