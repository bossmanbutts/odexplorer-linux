using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Aleoida_Gravis : SpeciesBase
{
	public override string Genus => "Aleoida";

	public override string SpeciesCodex => "$Codex_Ent_Aleoids_05_Name;";

	public override string SpeciesName => "Gravis";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.056;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 190.0;

	public override double MaxTemperature => 196.0;

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
			span[index] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 12;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_Y_Name;", "Aleoida Gravis - Amethyst", VariantColours.Amethyst, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_M_Name;", "Aleoida Gravis - Emerald", VariantColours.Emerald, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_A_Name;", "Aleoida Gravis - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_W_Name;", "Aleoida Gravis - Grey", VariantColours.Grey, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_D_Name;", "Aleoida Gravis - Indigo", VariantColours.Indigo, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_L_Name;", "Aleoida Gravis - Lime", VariantColours.Lime, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_TTS_Name;", "Aleoida Gravis - Mauve", VariantColours.Mauve, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_N_Name;", "Aleoida Gravis - Ocher", VariantColours.Ocher, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_T_Name;", "Aleoida Gravis - Sage", VariantColours.Sage, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_F_Name;", "Aleoida Gravis - Teal", VariantColours.Teal, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_K_Name;", "Aleoida Gravis - Turquoise", VariantColours.Turquoise, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_05_B_Name;", "Aleoida Gravis - Yellow", VariantColours.Yellow, StarType.B, PlanetMaterial.None);
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
		if (planet.SurfacePressure < MinPressure)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
