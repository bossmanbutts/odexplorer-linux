using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Electricae_Radialem : SpeciesBase
{
	public override string Genus => "Electricae";

	public override string SpeciesCodex => "$Codex_Ent_Electricae_02_Name;";

	public override string SpeciesName => "Radialem";

	public override double MinGravity => 0.025;

	public override double MaxGravity => 0.275;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 0.0;

	public override double MaxTemperature => 0.0;

	public override double MinDistance => 500.0;

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
			int num = 3;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_neon_rich_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Electricae_02_Technetium_Name;", "Electricae Radialem - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Electricae_02_Ruthenium_Name;", "Electricae Radialem - Blue", VariantColours.Blue, StarType.Unknown, PlanetMaterial.ruthenium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Electricae_02_Polonium_Name;", "Electricae Radialem - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Electricae_02_Antimony_Name;", "Electricae Radialem - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.antimony);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Electricae_02_Yttrium_Name;", "Electricae Radialem - Green", VariantColours.Green, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Electricae_02_Tellurium_Name;", "Electricae Radialem - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.tellurium);
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
		if (planet.DistanceToNebula > MinDistance)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
