using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Concha_Renibus : SpeciesBase
{
	public override string Genus => "Concha";

	public override string SpeciesCodex => "$Codex_Ent_Conchas_01_Name;";

	public override string SpeciesName => "Renibus";

	public override double MinGravity => 0.039;

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
			span[num2] = PlanetClass.RockyBody;
			num2++;
			span[num2] = PlanetClass.HighMetalContentBody;
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
			span[num2] = AtmosphereDescription.thin_ammonia_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Conchas_01_Tin_Name;", "Concha Renibus - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_01_Niobium_Name;", "Concha Renibus - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_01_Mercury_Name;", "Concha Renibus - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_01_Molybdenum_Name;", "Concha Renibus - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_01_Cadmium_Name;", "Concha Renibus - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_01_Tungsten_Name;", "Concha Renibus - White", VariantColours.White, StarType.Unknown, PlanetMaterial.tungsten);
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
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere)
		{
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "CarbonDioxide", StringComparison.OrdinalIgnoreCase)).Percent < 97.5)
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(180.0, 196.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(0.0254, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_methane_atmosphere)
		{
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "Methane", StringComparison.OrdinalIgnoreCase)).Percent < 100.0)
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(79.0, 103.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(0.0125, 0.0201, planet.SurfacePressure))
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_atmosphere)
		{
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "Water", StringComparison.OrdinalIgnoreCase)).Percent < 100.0)
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(390.0, 452.2, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(0.0527, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_ammonia_atmosphere)
		{
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "Ammonia", StringComparison.OrdinalIgnoreCase)).Percent < 100.0)
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(163.0, 177.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(0.0125, 0.013, planet.SurfacePressure))
			{
				return null;
			}
		}
		return base.GetPredictions(planet);
	}
}
