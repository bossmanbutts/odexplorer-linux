using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Tussock_Stigmasis : SpeciesBase
{
	public override string Genus => "Tussock";

	public override string SpeciesCodex => "$Codex_Ent_Tussocks_13_Name;";

	public override string SpeciesName => "Stigmasis";

	public override double MinGravity => 0.0;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0057;

	public override double MinTemperature => 132.0;

	public override double MaxTemperature => 167.0;

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
			int num = 2;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 9;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_M_Name;", "Tussock Stigmasis - Emerald", VariantColours.Emerald, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_K_Name;", "Tussock Stigmasis - Green", VariantColours.Green, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_G_Name;", "Tussock Stigmasis - Lime", VariantColours.Lime, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_D_Name;", "Tussock Stigmasis - Maroon", VariantColours.Maroon, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_W_Name;", "Tussock Stigmasis - Orange", VariantColours.Orange, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_Y_Name;", "Tussock Stigmasis - Red", VariantColours.Red, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_L_Name;", "Tussock Stigmasis - Sage", VariantColours.Sage, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_T_Name;", "Tussock Stigmasis - Teal", VariantColours.Teal, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Tussocks_13_F_Name;", "Tussock Stigmasis - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinPressure, MaxPressure, planet.SurfacePressure))
		{
			return null;
		}
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
