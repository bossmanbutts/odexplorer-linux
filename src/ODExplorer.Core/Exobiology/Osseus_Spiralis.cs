using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Osseus_Spiralis : SpeciesBase
{
	public override string Genus => "Osseus";

	public override string SpeciesCodex => "$Codex_Ent_Osseus_03_Name;";

	public override string SpeciesName => "Spiralis";

	public override double MinGravity => 0.0;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.015;

	public override double MinTemperature => 160.0;

	public override double MaxTemperature => 177.0;

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
			span[num2] = PlanetClass.RockyIceBody;
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
			span[index] = AtmosphereDescription.thin_ammonia_atmosphere;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 8;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_T_Name;", "Osseus Spiralis - Emerald", VariantColours.Emerald, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_TTS_Name;", "Osseus Spiralis - Green", VariantColours.Green, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_G_Name;", "Osseus Spiralis - Grey", VariantColours.Grey, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_K_Name;", "Osseus Spiralis - Indigo", VariantColours.Indigo, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_A_Name;", "Osseus Spiralis - Lime", VariantColours.Lime, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_Y_Name;", "Osseus Spiralis - Maroon", VariantColours.Maroon, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_F_Name;", "Osseus Spiralis - Turquoise", VariantColours.Turquoise, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_03_O_Name;", "Osseus Spiralis - Yellow", VariantColours.Yellow, StarType.O, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (planet.SurfaceGravity >= MaxGravity)
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
