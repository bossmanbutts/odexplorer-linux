using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Tubus_Rosarium : SpeciesBase
{
	public override string Genus => "Tubus";

	public override string SpeciesCodex => "$Codex_Ent_Tubus_04_Name;";

	public override string SpeciesName => "Rosarium";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.153;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0134;

	public override double MinTemperature => 160.0;

	public override double MaxTemperature => 177.0;

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
			int num = 13;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_N_Name;", "Tubus Rosarium - Amethyst", VariantColours.Amethyst, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_B_Name;", "Tubus Rosarium - Emerald", VariantColours.Emerald, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_O_Name;", "Tubus Rosarium - Green", VariantColours.Green, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_F_Name;", "Tubus Rosarium - Grey", VariantColours.Grey, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_A_Name;", "Tubus Rosarium - Indigo", VariantColours.Indigo, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_W_Name;", "Tubus Rosarium - Lime", VariantColours.Lime, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_K_Name;", "Tubus Rosarium - Maroon", VariantColours.Maroon, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_T_Name;", "Tubus Rosarium - Mauve", VariantColours.Mauve, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_TTS_Name;", "Tubus Rosarium - Ocher", VariantColours.Ocher, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_G_Name;", "Tubus Rosarium - Red", VariantColours.Red, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_M_Name;", "Tubus Rosarium - Teal", VariantColours.Teal, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_L_Name;", "Tubus Rosarium - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tubus_04_D_Name;", "Tubus Rosarium - Yellow", VariantColours.Yellow, StarType.D, PlanetMaterial.None);
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
		if (planet.SurfacePressure >= MaxPressure)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
