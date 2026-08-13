using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Tussock_Catena : SpeciesBase
{
	public override string Genus => "Tussock";

	public override string SpeciesCodex => "$Codex_Ent_Tussocks_05_Name;";

	public override string SpeciesName => "Catena";

	public override double MinGravity => 0.0;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0133;

	public override double MinTemperature => 152.0;

	public override double MaxTemperature => 177.0;

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
			int num = 1;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = AtmosphereDescription.thin_ammonia_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 9;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_9;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_10;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_11;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_12;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_24;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_25;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_26;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_28;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_42;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 9;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_M_Name;", "Tussock Catena - Emerald", VariantColours.Emerald, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_K_Name;", "Tussock Catena - Green", VariantColours.Green, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_G_Name;", "Tussock Catena - Lime", VariantColours.Lime, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_D_Name;", "Tussock Catena - Maroon", VariantColours.Maroon, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_W_Name;", "Tussock Catena - Orange", VariantColours.Orange, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_Y_Name;", "Tussock Catena - Red", VariantColours.Red, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_L_Name;", "Tussock Catena - Sage", VariantColours.Sage, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_T_Name;", "Tussock Catena - Teal", VariantColours.Teal, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_05_F_Name;", "Tussock Catena - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinPressure, MaxPressure, planet.SurfacePressure))
		{
			return null;
		}
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
