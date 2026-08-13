using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Tubus_Cavas : SpeciesBase
{
	public override string Genus => "Tubus";

	public override string SpeciesCodex => "$Codex_Ent_Tubus_03_Name;";

	public override string SpeciesName => "Cavas";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.153;

	public override double MinPressure => 0.003;

	public override double MaxPressure => double.MaxValue;

	public override double MinTemperature => 160.0;

	public override double MaxTemperature => 195.1;

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
			span[index] = PlanetClass.RockyBody;
			return list;
		}
	}

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 2;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_rich_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 11;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_1;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_4;
			num2++;
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
			int num = 13;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_N_Name;", "Tubus Cavas - Amethyst", VariantColours.Amethyst, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_B_Name;", "Tubus Cavas - Emerald", VariantColours.Emerald, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_O_Name;", "Tubus Cavas - Green", VariantColours.Green, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_F_Name;", "Tubus Cavas - Grey", VariantColours.Grey, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_A_Name;", "Tubus Cavas - Indigo", VariantColours.Indigo, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_W_Name;", "Tubus Cavas - Lime", VariantColours.Lime, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_K_Name;", "Tubus Cavas - Maroon", VariantColours.Maroon, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_T_Name;", "Tubus Cavas - Mauve", VariantColours.Mauve, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_TTS_Name;", "Tubus Cavas - Ocher", VariantColours.Ocher, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_G_Name;", "Tubus Cavas - Red", VariantColours.Red, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_M_Name;", "Tubus Cavas - Teal", VariantColours.Teal, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_L_Name;", "Tubus Cavas - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_03_D_Name;", "Tubus Cavas - Yellow", VariantColours.Yellow, StarType.D, PlanetMaterial.None);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		if (!MathHelpers.DoubleBetweenMinEquals(MinPressure, MaxPressure, planet.SurfacePressure))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
