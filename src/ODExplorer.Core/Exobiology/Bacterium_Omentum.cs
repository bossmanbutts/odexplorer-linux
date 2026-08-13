using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Omentum : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_11_Name;";

	public override string SpeciesName => "Omentum";

	public override double MinGravity => 0.025;

	public override double MaxGravity => 0.6;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 0.0;

	public override double MaxTemperature => 0.0;

	public override double MinDistance => 0.0;

	public override double MaxDistance => 0.0;

	public override List<Volcanism> Volcanism
	{
		get
		{
			int num = 4;
			List<Volcanism> list = new List<Volcanism>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Volcanism> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = EliteJournalReader.Volcanism.major_nitrogen_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_nitrogen_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_nitrogen_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_ammonia_magma_volcanism;
			return list;
		}
	}

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

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 6;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_neon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_neon_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_helium_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.water_rich_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_11_Molybdenum_Name;", "Bacterium Omentum - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_11_Tungsten_Name;", "Bacterium Omentum - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_11_Cadmium_Name;", "Bacterium Omentum - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_11_Niobium_Name;", "Bacterium Omentum - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_11_Tin_Name;", "Bacterium Omentum - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_11_Mercury_Name;", "Bacterium Omentum - White", VariantColours.White, StarType.Unknown, PlanetMaterial.mercury);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (!MathHelpers.DoubleBetweenMinEquals(MinGravity, MaxGravity, planet.SurfaceGravity))
		{
			return null;
		}
		if (!Planets.Contains(planet.PlanetClass))
		{
			return null;
		}
		if (!Atmospheres.Contains(planet.Atmosphere))
		{
			return null;
		}
		if (!Volcanism.Contains(planet.Volcanism))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
