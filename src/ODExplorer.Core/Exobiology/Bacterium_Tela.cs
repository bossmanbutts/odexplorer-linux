using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Extensions;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Bacterium_Tela : SpeciesBase
{
	public override string Genus => "Bacterium";

	public override string SpeciesCodex => "$Codex_Ent_Bacterial_07_Name;";

	public override string SpeciesName => "Tela";

	public override double MinGravity => 0.025;

	public override double MaxGravity => 0.61;

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
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Bacterial_07_Tin_Name;", "Bacterium Tela - Cobalt", VariantColours.Cobalt, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_07_Cadmium_Name;", "Bacterium Tela - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_07_Tungsten_Name;", "Bacterium Tela - Green", VariantColours.Green, StarType.Unknown, PlanetMaterial.tungsten);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_07_Niobium_Name;", "Bacterium Tela - Magenta", VariantColours.Magenta, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_07_Mercury_Name;", "Bacterium Tela - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Bacterial_07_Molybdenum_Name;", "Bacterium Tela - Yellow", VariantColours.Yellow, StarType.Unknown, PlanetMaterial.molybdenum);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (planet.Volcanism == EliteJournalReader.Volcanism.None)
		{
			AtmosphereClass atmosphereClass = planet.AtmosphereClass;
			if ((atmosphereClass == AtmosphereClass.CarbonDioxide || atmosphereClass == AtmosphereClass.SulphurDioxide || atmosphereClass == AtmosphereClass.WaterRich) ? true : false)
			{
				if (planet.SurfaceGravity < 0.04)
				{
					return null;
				}
				if (planet.SurfaceTemperature < 300.0)
				{
					return null;
				}
				return base.GetPredictions(planet);
			}
			if (planet.AtmosphereClass == AtmosphereClass.Water)
			{
				if (planet.SurfaceGravity < 0.04)
				{
					return null;
				}
				if (planet.SurfaceTemperature < 390.0)
				{
					return null;
				}
				return base.GetPredictions(planet);
			}
			return null;
		}
		if (!planet.Atmosphere.GetEnumDescription().Contains("thin", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		if (!MathHelpers.DoubleBetweenMinEquals(MinGravity, MaxGravity, planet.SurfaceGravity))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
