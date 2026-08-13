using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fonticulua_Upupam : SpeciesBase
{
	public override string Genus => "Fonticulua";

	public override string SpeciesCodex => "$Codex_Ent_Fonticulus_03_Name;";

	public override string SpeciesName => "Upupam";

	public override double MinGravity => 0.2;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.019;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 60.0;

	public override double MaxTemperature => 120.0;

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
			int num = 1;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = AtmosphereDescription.thin_argon_rich_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_M_Name;", "Fonticulua Upupam - Amethyst", VariantColours.Amethyst, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_K_Name;", "Fonticulua Upupam - Emerald", VariantColours.Emerald, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_A_Name;", "Fonticulua Upupam - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_O_Name;", "Fonticulua Upupam - Grey", VariantColours.Grey, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_W_Name;", "Fonticulua Upupam - Indigo", VariantColours.Indigo, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_B_Name;", "Fonticulua Upupam - Lime", VariantColours.Lime, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_Ae_Name;", "Fonticulua Upupam - Maroon", VariantColours.Maroon, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_L_Name;", "Fonticulua Upupam - Mauve", VariantColours.Mauve, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_Y_Name;", "Fonticulua Upupam - Ocher", VariantColours.Ocher, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_T_Name;", "Fonticulua Upupam - Orange", VariantColours.Orange, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_TTS_Name;", "Fonticulua Upupam - Red", VariantColours.Red, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_N_Name;", "Fonticulua Upupam - Sage", VariantColours.Sage, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_G_Name;", "Fonticulua Upupam - Teal", VariantColours.Teal, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_D_Name;", "Fonticulua Upupam - Turquoise", VariantColours.Turquoise, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_03_F_Name;", "Fonticulua Upupam - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
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
		if (planet.SurfacePressure < MinPressure)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
