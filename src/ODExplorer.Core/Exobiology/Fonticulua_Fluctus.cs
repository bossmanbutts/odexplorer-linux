using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fonticulua_Fluctus : SpeciesBase
{
	public override string Genus => "Fonticulua";

	public override string SpeciesCodex => "$Codex_Ent_Fonticulus_05_Name;";

	public override string SpeciesName => "Fluctus";

	public override double MinGravity => 0.24;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.012;

	public override double MaxPressure => 0.08;

	public override double MinTemperature => 142.0;

	public override double MaxTemperature => 200.0;

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
			span[index] = AtmosphereDescription.thin_oxygen_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_M_Name;", "Fonticulua Fluctus - Amethyst", VariantColours.Amethyst, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_K_Name;", "Fonticulua Fluctus - Emerald", VariantColours.Emerald, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_A_Name;", "Fonticulua Fluctus - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_O_Name;", "Fonticulua Fluctus - Grey", VariantColours.Grey, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_W_Name;", "Fonticulua Fluctus - Indigo", VariantColours.Indigo, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_B_Name;", "Fonticulua Fluctus - Lime", VariantColours.Lime, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_Ae_Name;", "Fonticulua Fluctus - Lime", VariantColours.Maroon, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_L_Name;", "Fonticulua Fluctus - Mauve", VariantColours.Mauve, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_Y_Name;", "Fonticulua Fluctus - Ocher", VariantColours.Ocher, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_T_Name;", "Fonticulua Fluctus - Orange", VariantColours.Orange, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_TTS_Name;", "Fonticulua Fluctus - Red", VariantColours.Red, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_N_Name;", "Fonticulua Fluctus - Sage", VariantColours.Sage, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_G_Name;", "Fonticulua Fluctus - Teal", VariantColours.Teal, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_D_Name;", "Fonticulua Fluctus - Turquoise", VariantColours.Turquoise, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_05_F_Name;", "Fonticulua Fluctus - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinPressure, MaxPressure, planet.SurfacePressure))
		{
			return null;
		}
		if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => x.Name.Contains("Oxygen", StringComparison.OrdinalIgnoreCase)).Percent < 50.0)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
