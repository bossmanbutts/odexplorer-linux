using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Osseus_Pellebantus : SpeciesBase
{
	public override string Genus => "Osseus";

	public override string SpeciesCodex => "$Codex_Ent_Osseus_06_Name;";

	public override string SpeciesName => "Pellebantus";

	public override double MinGravity => 0.0;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.058;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 190.0;

	public override double MaxTemperature => 196.0;

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
			int num = 31;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_2;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_4;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_5;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_6;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_8;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_9;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_10;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_11;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_12;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_13;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_14;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_16;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_17;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_18;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_19;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_20;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_21;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_22;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_23;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_24;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_25;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_26;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_27;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_28;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_29;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_31;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_35;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_37;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_40;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_41;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_42;
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
			span[num2] = new Variant("$Codex_Ent_Osseus_06_T_Name;", "Osseus Pellebantus - Emerald", VariantColours.Emerald, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_TTS_Name;", "Osseus Pellebantus - Green", VariantColours.Green, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_G_Name;", "Osseus Pellebantus - Grey", VariantColours.Grey, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_K_Name;", "Osseus Pellebantus - Indigo", VariantColours.Indigo, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_A_Name;", "Osseus Pellebantus - Lime", VariantColours.Lime, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_Y_Name;", "Osseus Pellebantus - Maroon", VariantColours.Maroon, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_F_Name;", "Osseus Pellebantus - Turquoise", VariantColours.Turquoise, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Osseus_06_O_Name;", "Osseus Pellebantus - Yellow", VariantColours.Yellow, StarType.O, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (!Regions.Contains(planet.Region))
		{
			return null;
		}
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
		if (planet.SurfacePressure < MinPressure)
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
