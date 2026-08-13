using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology.Radicoida.Unica;

public sealed class Radicoida_Unica : SpeciesBase
{
	public override string Genus => "Radicoida";

	public override string SpeciesCodex => "$Codex_Ent_Ingensradices_Unicus_Name;";

	public override string SpeciesName => "Unica";

	public override double MinGravity => 0.05;

	public override double MaxGravity => 0.38;

	public override double MinPressure => 0.007;

	public override double MaxPressure => 0.06;

	public override double MinTemperature => 467.0;

	public override double MaxTemperature => 699.0;

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
			int num = 1;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 1;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = GalacticRegions.Codex_RegionName_18;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 1;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = new Variant("$Codex_Ent_Ingensradices_Unicus_Name;", "Radicoida Unica", VariantColours.Unknown, StarType.Unknown, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (planet.SystemAddress != 147882789259L)
		{
			return null;
		}
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
		if (!MathHelpers.DoubleBetweenMinEquals(planet.SurfacePressure, MinPressure, MaxPressure))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
