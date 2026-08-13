using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Scopulum : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_03_Name;";

	public override string SpeciesName => "Scopulum";

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
			span[num2] = EliteJournalReader.Volcanism.carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_methane_magma_volcanism;
			return list;
		}
	}

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

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 5;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_neon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.neon_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_helium_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_03_Tungsten_Name;", "Bacterium Scopulum - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_03_Molybdenum_Name;", "Bacterium Scopulum - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_03_Tin_Name;", "Bacterium Scopulum - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_03_Mercury_Name;", "Bacterium Scopulum - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_03_Niobium_Name;", "Bacterium Scopulum - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_03_Cadmium_Name;", "Bacterium Scopulum - White", VariantColours.White, StarType.Unknown, PlanetMaterial.cadmium);
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
