using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Stratum_Limaxus : SpeciesBase
{
	public override string Genus => "Stratum";

	public override string SpeciesCodex => "$Codex_Ent_Stratum_05_Name;";

	public override string SpeciesName => "Limaxus";

	public override double MinGravity => 0.04;

	public override double MaxGravity => 0.48;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 165.0;

	public override double MaxTemperature => 190.0;

	public override double MinDistance => 0.0;

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
			span[index] = PlanetClass.RockyBody;
			return list;
		}
	}

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
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 9;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_9;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_10;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_11;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_12;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_24;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_25;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_26;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_28;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_42;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 10;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_TTS_Name;", "Stratum Limaxus - Amethyst", VariantColours.Amethyst, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_F_Name;", "Stratum Limaxus - Emerald", VariantColours.Emerald, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_M_Name;", "Stratum Limaxus - Green", VariantColours.Green, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_T_Name;", "Stratum Limaxus - Grey", VariantColours.Grey, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_Y_Name;", "Stratum Limaxus - Indigo", VariantColours.Indigo, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_K_Name;", "Stratum Limaxus - Lime", VariantColours.Lime, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_D_Name;", "Stratum Limaxus - Mauve", VariantColours.Mauve, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_W_Name;", "Stratum Limaxus - Red", VariantColours.Red, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_Ae_Name;", "Stratum Limaxus - Teal", VariantColours.Teal, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_05_L_Name;", "Stratum Limaxus - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (!Regions.Contains(planet.Region))
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MinTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
