using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Cerbrus : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_12_Name;";

	public override string SpeciesName => "Cerbrus";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.6;

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
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_rich_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_W_Name;", "Bacterium Cerbrus - Amethyst", VariantColours.Amethyst, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_G_Name;", "Bacterium Cerbrus - Emerald", VariantColours.Emerald, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_K_Name;", "Bacterium Cerbrus - Green", VariantColours.Green, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_B_Name;", "Bacterium Cerbrus - Grey", VariantColours.Grey, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_N_Name;", "Bacterium Cerbrus - Indigo", VariantColours.Indigo, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_F_Name;", "Bacterium Cerbrus - Lime", VariantColours.Lime, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_TTS_Name;", "Bacterium Cerbrus - Maroon", VariantColours.Maroon, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_Y_Name;", "Bacterium Cerbrus - Mauve", VariantColours.Mauve, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_D_Name;", "Bacterium Cerbrus - Ocher", VariantColours.Ocher, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_Ae_Name;", "Bacterium Cerbrus - Orange", VariantColours.Orange, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_T_Name;", "Bacterium Cerbrus - Red", VariantColours.Red, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_L_Name;", "Bacterium Cerbrus - Sage", VariantColours.Sage, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_M_Name;", "Bacterium Cerbrus - Teal", VariantColours.Teal, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_O_Name;", "Bacterium Cerbrus - Turquoise", VariantColours.Turquoise, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_12_A_Name;", "Bacterium Cerbrus - Yellow", VariantColours.Yellow, StarType.A, PlanetMaterial.None);
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
		AtmosphereDescription atmosphere = planet.Atmosphere;
		bool flag = (uint)(atmosphere - 74) <= 1u;
		if (flag && !MathHelpers.DoubleBetweenMinEquals(132.0, 499.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_atmosphere && !MathHelpers.DoubleBetweenMinEquals(392.0, 452.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_rich_atmosphere && !MathHelpers.DoubleBetweenMinEquals(220.0, 330.0, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
