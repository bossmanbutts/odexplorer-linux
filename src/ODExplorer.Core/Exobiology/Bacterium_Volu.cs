using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Volu : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_09_Name;";

	public override string SpeciesName => "Volu";

	public override double MinGravity => 0.24;

	public override double MaxGravity => 0.515;

	public override double MinPressure => 0.014;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 145.0;

	public override double MaxTemperature => 245.0;

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
			int num = 6;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Bacterial_09_Polonium_Name;", "Bacterium Volu - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_09_Ruthenium_Name;", "Bacterium Volu - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.ruthenium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_09_Tellurium_Name;", "Bacterium Volu - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.tellurium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_09_Yttrium_Name;", "Bacterium Volu - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_09_Technetium_Name;", "Bacterium Volu - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_09_Antimony_Name;", "Bacterium Volu - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.antimony);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (!MathHelpers.DoubleBetweenMinEquals(MinGravity, MaxGravity, planet.SurfaceGravity))
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
