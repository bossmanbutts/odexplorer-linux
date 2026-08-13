using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fungoida_Gelata : SpeciesBase
{
	public override string Genus => "Fungoida";

	public override string SpeciesCodex => "$Codex_Ent_Fungoids_04_Name;";

	public override string SpeciesName => "Gelata";

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
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 36;
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

	public override List<Variant> Variants
	{
		get
		{
			int num = 6;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Fungoids_04_Cadmium_Name;", "Fungoida Gelata - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_04_Niobium_Name;", "Fungoida Gelata - Green", VariantColours.Green, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_04_Mercury_Name;", "Fungoida Gelata - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_04_Molybdenum_Name;", "Fungoida Gelata - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_04_Tungsten_Name;", "Fungoida Gelata - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_04_Tin_Name;", "Fungoida Gelata - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.tin);
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
		if (planet.Atmosphere == AtmosphereDescription.thin_ammonia_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(160.0, 177.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (planet.SurfacePressure >= 0.0134)
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_methane_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(80.0, 110.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (planet.SurfacePressure >= 0.0129)
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(180.0, 200.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetween(0.0254, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "CarbonDioxide", StringComparison.OrdinalIgnoreCase)).Percent < 97.5)
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(395.0, 455.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (!MathHelpers.DoubleBetween(0.052, 0.0987, planet.SurfacePressure))
			{
				return null;
			}
		}
		return base.GetPredictions(planet);
	}
}
