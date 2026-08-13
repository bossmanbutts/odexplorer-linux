using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Tubus_Conifer : SpeciesBase
{
	public override string Genus => "Tubus";

	public override string SpeciesCodex => "$Codex_Ent_Tubus_01_Name;";

	public override string SpeciesName => "Conifer";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.153;

	public override double MinPressure => 0.003;

	public override double MaxPressure => double.MaxValue;

	public override double MinTemperature => 160.0;

	public override double MaxTemperature => 195.0;

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
			span[num2] = GalacticRegions.Codex_RegionName_3;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_7;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_15;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_30;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_32;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_33;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_34;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_36;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_38;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_39;
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
			span[num2] = new Variant("$Codex_Ent_Tubus_01_N_Name;", "Tubus Conifer - Amethyst", VariantColours.Amethyst, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_B_Name;", "Tubus Conifer - Emerald", VariantColours.Emerald, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_O_Name;", "Tubus Conifer - Green", VariantColours.Green, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_F_Name;", "Tubus Conifer - Grey", VariantColours.Grey, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_A_Name;", "Tubus Conifer - Indigo", VariantColours.Indigo, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_W_Name;", "Tubus Conifer - Lime", VariantColours.Lime, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_K_Name;", "Tubus Conifer - Maroon", VariantColours.Maroon, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_T_Name;", "Tubus Conifer - Mauve", VariantColours.Mauve, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_TTS_Name;", "Tubus Conifer - Ocher", VariantColours.Ocher, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_G_Name;", "Tubus Conifer - Red", VariantColours.Red, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_M_Name;", "Tubus Conifer - Teal", VariantColours.Teal, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_L_Name;", "Tubus Conifer - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_01_D_Name;", "Tubus Conifer - Yellow", VariantColours.Yellow, StarType.D, PlanetMaterial.None);
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
