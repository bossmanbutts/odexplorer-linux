using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Recepta_Deltahedronix : SpeciesBase
{
	public override string Genus => "Recepta";

	public override string SpeciesCodex => "$Codex_Ent_Recepta_02_Name;";

	public override string SpeciesName => "Deltahedronix";

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
			span[num2] = new Variant("$Codex_Ent_Recepta_02_Mercury_Name;", "Recepta Deltahedronix - Cyan", VariantColours.Cyan, StarType.Unknown, PlanetMaterial.mercury);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_02_Molybdenum_Name;", "Recepta Deltahedronix - Gold", VariantColours.Gold, StarType.Unknown, PlanetMaterial.molybdenum);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_02_Cadmium_Name;", "Recepta Deltahedronix - Lime", VariantColours.Lime, StarType.Unknown, PlanetMaterial.cadmium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_02_Niobium_Name;", "Recepta Deltahedronix - Mulberry", VariantColours.Mulberry, StarType.Unknown, PlanetMaterial.niobium);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_02_Tin_Name;", "Recepta Deltahedronix - Orange", VariantColours.Orange, StarType.Unknown, PlanetMaterial.tin);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_02_Tungsten_Name;", "Recepta Deltahedronix - Red", VariantColours.Red, StarType.Unknown, PlanetMaterial.tungsten);
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
