using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Alcyoneum : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_06_Name;";

	public override string SpeciesName => "Alcyoneum";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.3715;

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

	public override List<Variant> Variants
	{
		get
		{
			int num = 15;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_W_Name;", "Bacterium Alcyoneum - Amethyst", VariantColours.Amethyst, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_G_Name;", "Bacterium Alcyoneum - Emerald", VariantColours.Emerald, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_K_Name;", "Bacterium Alcyoneum - Green", VariantColours.Green, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_B_Name;", "Bacterium Alcyoneum - Grey", VariantColours.Grey, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_N_Name;", "Bacterium Alcyoneum - Indigo", VariantColours.Indigo, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_F_Name;", "Bacterium Alcyoneum - Lime", VariantColours.Lime, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_TTS_Name;", "Bacterium Alcyoneum - Maroon", VariantColours.Maroon, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_Y_Name;", "Bacterium Alcyoneum - Mauve", VariantColours.Mauve, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_D_Name;", "Bacterium Alcyoneum - Ocher", VariantColours.Ocher, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_Ae_Name;", "Bacterium Alcyoneum - Orange", VariantColours.Orange, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_T_Name;", "Bacterium Alcyoneum - Red", VariantColours.Red, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_L_Name;", "Bacterium Alcyoneum - Sage", VariantColours.Sage, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_M_Name;", "Bacterium Alcyoneum - Teal", VariantColours.Teal, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_O_Name;", "Bacterium Alcyoneum - Turquoise", VariantColours.Turquoise, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_06_A_Name;", "Bacterium Alcyoneum - Yellow", VariantColours.Yellow, StarType.A, PlanetMaterial.None);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.SurfacePressure >= MaxPressure)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
