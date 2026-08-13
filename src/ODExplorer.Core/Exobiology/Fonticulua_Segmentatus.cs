using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fonticulua_Segmentatus : SpeciesBase
{
	public override string Genus => "Fonticulua";

	public override string SpeciesCodex => "$Codex_Ent_Fonticulus_01_Name;";

	public override string SpeciesName => "Segmentatus";

	public override double MinGravity => 0.25;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.028;

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
			int num = 2;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_neon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_neon_rich_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_M_Name;", "Fonticulua Segmentatus - Amethyst", VariantColours.Amethyst, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_K_Name;", "Fonticulua Segmentatus - Emerald", VariantColours.Emerald, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_A_Name;", "Fonticulua Segmentatus - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_O_Name;", "Fonticulua Segmentatus - Grey", VariantColours.Grey, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_W_Name;", "Fonticulua Segmentatus - Indigo", VariantColours.Indigo, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_B_Name;", "Fonticulua Segmentatus - Lime", VariantColours.Lime, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_Ae_Name;", "Fonticulua Segmentatus - Maroon", VariantColours.Maroon, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_L_Name;", "Fonticulua Segmentatus - Mauve", VariantColours.Mauve, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_Y_Name;", "Fonticulua Segmentatus - Ocher", VariantColours.Ocher, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_T_Name;", "Fonticulua Segmentatus - Orange", VariantColours.Orange, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_TTS_Name;", "Fonticulua Segmentatus - Red", VariantColours.Red, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_N_Name;", "Fonticulua Segmentatus - Sage", VariantColours.Sage, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_G_Name;", "Fonticulua Segmentatus - Teal", VariantColours.Teal, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_D_Name;", "Fonticulua Segmentatus - Turquoise", VariantColours.Turquoise, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_01_F_Name;", "Fonticulua Segmentatus - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
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
		if (planet.SurfacePressure >= MaxPressure)
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_neon_rich_atmosphere && !MathHelpers.DoubleBetweenMinEquals(58.0, 69.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_neon_atmosphere && !MathHelpers.DoubleBetweenMinEquals(50.0, 57.0, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
