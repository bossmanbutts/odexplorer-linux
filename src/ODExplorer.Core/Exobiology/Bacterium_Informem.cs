using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Informem : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_08_Name;";

	public override string SpeciesName => "Informem";

	public override double MinGravity => 0.108;

	public override double MaxGravity => 0.6;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 43.0;

	public override double MaxTemperature => 149.0;

	public override double MinDistance => 0.0;

	public override double MaxDistance => 0.0;

	public override List<PlanetClass> Planets => new List<PlanetClass>();

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 1;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = AtmosphereDescription.thin_nitrogen_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_08_Technetium_Name;", "Bacterium Informem - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_08_Yttrium_Name;", "Bacterium Informem - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_08_Ruthenium_Name;", "Bacterium Informem - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.ruthenium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_08_Polonium_Name;", "Bacterium Informem - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_08_Antimony_Name;", "Bacterium Informem - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.antimony);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_08_Tellurium_Name;", "Bacterium Informem - Yellow", VariantColours.Yellow, StarType.Unknown, PlanetMaterial.tellurium);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
