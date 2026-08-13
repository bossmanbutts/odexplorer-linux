using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fungoida_Bullarum : SpeciesBase
{
	public override string Genus => "Fungoida";

	public override string SpeciesCodex => "$Codex_Ent_Fungoids_03_Name;";

	public override string SpeciesName => "Bullarum";

	public override double MinGravity => 0.0587;

	public override double MaxGravity => 0.2755;

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
			span[num2] = PlanetClass.RockyIceBody;
			num2++;
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
			span[num2] = AtmosphereDescription.thin_nitrogen_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 6;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Fungoids_03_Tellurium_Name;", "Fungoida Bullarum - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.tellurium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_03_Ruthenium_Name;", "Fungoida Bullarum - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.ruthenium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_03_Polonium_Name;", "Fungoida Bullarum - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_03_Yttrium_Name;", "Fungoida Bullarum - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_03_Technetium_Name;", "Fungoida Bullarum - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_03_Antimony_Name;", "Fungoida Bullarum - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.antimony);
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
		if (planet.Atmosphere == AtmosphereDescription.thin_nitrogen_atmosphere && !MathHelpers.DoubleBetween(50.0, 70.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_argon_atmosphere && !MathHelpers.DoubleBetween(50.0, 135.0, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
