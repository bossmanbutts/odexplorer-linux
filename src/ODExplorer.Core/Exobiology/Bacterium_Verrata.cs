using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Verrata : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_13_Name;";

	public override string SpeciesName => "Verrata";

	public override double MinGravity => 0.04;

	public override double MaxGravity => 0.612;

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
			span[num2] = PlanetClass.IcyBody;
			num2++;
			span[num2] = PlanetClass.RockyIceBody;
			num2++;
			span[num2] = PlanetClass.RockyBody;
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
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_ammonia_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_helium_atmosphere;
			return list;
		}
	}

	public override List<Volcanism> Volcanism
	{
		get
		{
			int num = 6;
			List<Volcanism> list = new List<Volcanism>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Volcanism> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = EliteJournalReader.Volcanism.water_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_water_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.water_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_water_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_water_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_water_magma_volcanism;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_13_Tin_Name;", "Bacterium Verrata - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_13_Tungsten_Name;", "Bacterium Verrata - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_13_Niobium_Name;", "Bacterium Verrata - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_13_Cadmium_Name;", "Bacterium Verrata - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_13_Mercury_Name;", "Bacterium Verrata - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_13_Molybdenum_Name;", "Bacterium Verrata - White", VariantColours.White, StarType.Unknown, PlanetMaterial.molybdenum);
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
