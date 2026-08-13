using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Fungoida_Setisis : SpeciesBase
{
	public override string Genus => "Fungoida";

	public override string SpeciesCodex => "$Codex_Ent_Fungoids_01_Name;";

	public override string SpeciesName => "Setisis";

	public override double MinGravity => 0.032;

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
			span[num2] = PlanetClass.RockyBody;
			num2++;
			span[num2] = PlanetClass.HighMetalContentBody;
			num2++;
			span[num2] = PlanetClass.RockyIceBody;
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
			span[num2] = AtmosphereDescription.thin_methane_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_ammonia_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Fungoids_01_Ruthenium_Name;", "Fungoida Setisis - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.ruthenium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_01_Technetium_Name;", "Fungoida Setisis - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.technetium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_01_Yttrium_Name;", "Fungoida Setisis - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.yttrium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_01_Antimony_Name;", "Fungoida Setisis - Peach", VariantColours.Peach, StarType.Unknown, PlanetMaterial.antimony);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_01_Polonium_Name;", "Fungoida Setisis - White", VariantColours.White, StarType.Unknown, PlanetMaterial.polonium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Fungoids_01_Tellurium_Name;", "Fungoida Setisis - Yellow", VariantColours.Yellow, StarType.Unknown, PlanetMaterial.tellurium);
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
		if (planet.Atmosphere == AtmosphereDescription.thin_methane_atmosphere && !MathHelpers.DoubleBetween(67.0, 109.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_ammonia_atmosphere)
		{
			if (!MathHelpers.DoubleBetween(152.0, 177.0, planet.SurfaceTemperature))
			{
				return null;
			}
			if (planet.SurfacePressure >= 0.013015)
			{
				return null;
			}
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "Ammonia", StringComparison.OrdinalIgnoreCase)).Percent < 99.5)
			{
				return null;
			}
		}
		return base.GetPredictions(planet);
	}
}
