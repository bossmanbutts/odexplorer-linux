using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Osseus_Discus : SpeciesBase
{
	public override string Genus => "Osseus";

	public override string SpeciesCodex => "$Codex_Ent_Osseus_02_Name;";

	public override string SpeciesName => "Discus";

	public override double MinGravity => 0.0;

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

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 4;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_ammonia_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			return list;
		}
	}

	public override List<Volcanism> Volcanism
	{
		get
		{
			int num = 16;
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
			span[num2] = EliteJournalReader.Volcanism.major_water_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_silicate_vapour_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_water_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.metallic_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_metallic_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_water_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.water_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_water_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.major_rocky_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.rocky_magma_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.minor_carbon_dioxide_geysers_volcanism;
			num2++;
			span[num2] = EliteJournalReader.Volcanism.water_geysers_volcanism;
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
			span[num2] = new Variant("$Codex_Ent_Osseus_02_Niobium_Name;", "Osseus Discus - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_02_Tin_Name;", "Osseus Discus - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_02_Mercury_Name;", "Osseus Discus - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_02_Molybdenum_Name;", "Osseus Discus - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_02_Tungsten_Name;", "Osseus Discus - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_02_Cadmium_Name;", "Osseus Discus - White", VariantColours.White, StarType.Unknown, PlanetMaterial.cadmium);
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
		if (planet.Atmosphere != AtmosphereDescription.thin_water_atmosphere && planet.Volcanism == EliteJournalReader.Volcanism.None)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
