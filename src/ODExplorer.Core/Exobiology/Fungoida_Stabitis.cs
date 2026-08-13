using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fungoida_Stabitis : SpeciesBase
{
	public override string Genus => "Fungoida";

	public override string SpeciesCodex => "$Codex_Ent_Fungoids_02_Name;";

	public override string SpeciesName => "Stabitis";

	public override double MinGravity => 0.041;

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
			int num = 3;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 8;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_1;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_4;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_7;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_8;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_16;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_17;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_18;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_35;
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
			span[num2] = new Variant("$Codex_Ent_Fungoids_02_Cadmium_Name;", "Fungoida Stabitis - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_02_Mercury_Name;", "Fungoida Stabitis - Green", VariantColours.Green, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_02_Molybdenum_Name;", "Fungoida Stabitis - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_02_Tin_Name;", "Fungoida Stabitis - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_02_Tungsten_Name;", "Fungoida Stabitis - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_02_Niobium_Name;", "Fungoida Stabitis - White", VariantColours.White, StarType.Unknown, PlanetMaterial.niobium);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (!Regions.Contains(planet.Region))
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
		if (!Atmospheres.Contains(planet.Atmosphere))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_methane_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(80.0, 110.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetween(0.0125, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(390.0, 452.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetween(0.056, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(180.0, 196.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetween(0.0259, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "CarbonDioxide", StringComparison.OrdinalIgnoreCase)).Percent < 97.5)
			{
				return null;
			}
		}
		return base.GetPredictions(planet);
	}
}
