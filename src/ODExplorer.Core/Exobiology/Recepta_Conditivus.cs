using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Recepta_Conditivus : SpeciesBase
{
	public override string Genus => "Recepta";

	public override string SpeciesCodex => "$Codex_Ent_Recepta_03_Name;";

	public override string SpeciesName => "Conditivus";

	public override double MinGravity => 0.0415;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 0.0;

	public override double MaxTemperature => 0.0;

	public override double MinDistance => 0.0;

	public override double MaxDistance => 0.0;

	public override List<PlanetClass> Planets => new List<PlanetClass>();

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 4;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_oxygen_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Recepta_03_Technetium_Name;", "Recepta Conditivus - Aquamarine", VariantColours.Aquamarine, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_03_Tellurium_Name;", "Recepta Conditivus - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.tellurium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_03_Yttrium_Name;", "Recepta Conditivus - Green", VariantColours.Green, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_03_Antimony_Name;", "Recepta Conditivus - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.antimony);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_03_Polonium_Name;", "Recepta Conditivus - White", VariantColours.White, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_03_Ruthenium_Name;", "Recepta Conditivus - Yellow", VariantColours.Yellow, StarType.Unknown, PlanetMaterial.ruthenium);
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
		if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => x.Name.StartsWith("Sulfur", StringComparison.OrdinalIgnoreCase) || x.Name.StartsWith("Sulphur", StringComparison.OrdinalIgnoreCase)).Percent < 1.05)
		{
			return null;
		}
		AtmosphereDescription atmosphere = planet.Atmosphere;
		bool flag = (uint)(atmosphere - 74) <= 1u;
		if (flag && !MathHelpers.DoubleBetweenMinEquals(132.0, 272.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere && !MathHelpers.DoubleBetweenMinEquals(151.0, 195.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_oxygen_atmosphere && !MathHelpers.DoubleBetweenMinEquals(151.0, 175.0, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
