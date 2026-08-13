using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Nebulus : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_02_Name;";

	public override string SpeciesName => "Nebulus";

	public override double MinGravity => 0.4;

	public override double MaxGravity => 0.55;

	public override double MinPressure => 0.067;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 20.0;

	public override double MaxTemperature => 21.0;

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
			span[index] = PlanetClass.IcyBody;
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
			span[index] = AtmosphereDescription.thin_helium_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_02_Yttrium_Name;", "Bacterium Nebulus - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_02_Technetium_Name;", "Bacterium Nebulus - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_02_Polonium_Name;", "Bacterium Nebulus - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_02_Tellurium_Name;", "Bacterium Nebulus - Green", VariantColours.Green, StarType.Unknown, PlanetMaterial.tellurium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_02_Antimony_Name;", "Bacterium Nebulus - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.antimony);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_02_Ruthenium_Name;", "Bacterium Nebulus - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.ruthenium);
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
