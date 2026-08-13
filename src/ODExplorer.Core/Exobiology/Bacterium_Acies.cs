using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Acies : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_04_Name;";

	public override string SpeciesName => "Acies";

	public override double MinGravity => 0.2625;

	public override double MaxGravity => 0.731;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.079;

	public override double MinTemperature => 20.0;

	public override double MaxTemperature => 139.0;

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
			span[index] = AtmosphereDescription.thin_neon_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_04_Yttrium_Name;", "Bacterium Acies - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_04_Ruthenium_Name;", "Bacterium Acies - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.ruthenium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_04_Antimony_Name;", "Bacterium Acies - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.antimony);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_04_Technetium_Name;", "Bacterium Acies - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_04_Polonium_Name;", "Bacterium Acies - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_04_Tellurium_Name;", "Bacterium Acies - White", VariantColours.White, StarType.Unknown, PlanetMaterial.tellurium);
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
		if (planet.SurfacePressure >= MaxPressure)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
