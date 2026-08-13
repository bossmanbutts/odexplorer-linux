using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fumerola_Carbosis : SpeciesBase
{
	public override string Genus => "Fumerola";

	public override string SpeciesCodex => "$Codex_Ent_Fumerolas_01_Name;";

	public override string SpeciesName => "Carbosis";

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
			int num = 2;
			List<PlanetClass> list = new List<PlanetClass>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<PlanetClass> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = PlanetClass.IcyBody;
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
			int num = 5;
			List<Volcanism> list = new List<Volcanism>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Volcanism> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = EliteJournalReader.Volcanism.carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_methane_magma_volcanism;
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
			span[num2] = new Variant("$Codex_Ent_Fumerolas_01_Niobium_Name;", "Fumerola Carbosis - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_01_Tin_Name;", "Fumerola Carbosis - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_01_Molybdenum_Name;", "Fumerola Carbosis - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_01_Mercury_Name;", "Fumerola Carbosis - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_01_Cadmium_Name;", "Fumerola Carbosis - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fumerolas_01_Tungsten_Name;", "Fumerola Carbosis - Yellow", VariantColours.Yellow, StarType.Unknown, PlanetMaterial.tungsten);
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
