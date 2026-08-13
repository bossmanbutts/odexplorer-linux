using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fonticulua_Digitos : SpeciesBase
{
	public override string Genus => "Fonticulua";

	public override string SpeciesCodex => "$Codex_Ent_Fonticulus_06_Name;";

	public override string SpeciesName => "Digitos";

	public override double MinGravity => 0.0;

	public override double MaxGravity => 0.058;

	public override double MinPressure => 0.03;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 83.0;

	public override double MaxTemperature => 109.0;

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
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_methane_rich_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_M_Name;", "Fonticulua Digitos - Amethyst", VariantColours.Amethyst, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_K_Name;", "Fonticulua Digitos - Emerald", VariantColours.Emerald, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_A_Name;", "Fonticulua Digitos - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_O_Name;", "Fonticulua Digitos - Grey", VariantColours.Grey, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_B_Name;", "Fonticulua Digitos - Lime", VariantColours.Lime, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_F_Name;", "Fonticulua Digitos - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_G_Name;", "Fonticulua Digitos - Teal", VariantColours.Teal, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_L_Name;", "Fonticulua Digitos - Mauve", VariantColours.Mauve, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_T_Name;", "Fonticulua Digitos - Orange", VariantColours.Orange, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_TTS_Name;", "Fonticulua Digitos - Red", VariantColours.Red, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_Y_Name;", "Fonticulua Digitos - Ocher", VariantColours.Ocher, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_D_Name;", "Fonticulua Digitos - Turquoise", VariantColours.Turquoise, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fonticulus_06_N_Name;", "Fonticulua Digitos - Sage", VariantColours.Sage, StarType.N, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (planet.SurfaceGravity > MaxGravity)
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
		if (planet.SurfacePressure < MinPressure)
		{
			return null;
		}
		if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => x.Name.Contains("Methane", StringComparison.OrdinalIgnoreCase)).Percent < 99.9)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
