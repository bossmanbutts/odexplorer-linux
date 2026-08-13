using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Aleoida_Laminiae : SpeciesBase
{
	public override string Genus => "Aleoida";

	public override string SpeciesCodex => "$Codex_Ent_Aleoids_04_Name;";

	public override string SpeciesName => "Laminiae";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.013;

	public override double MinTemperature => 152.0;

	public override double MaxTemperature => 177.0;

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
			span[index] = AtmosphereDescription.thin_ammonia_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 15;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_1;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_4;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_7;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_8;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_9;
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
			span[num2] = GalacticRegions.Codex_RegionName_35;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_40;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 12;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_Y_Name;", "Aleoida Laminiae - Amethyst", VariantColours.Amethyst, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_M_Name;", "Aleoida Laminiae - Emerald", VariantColours.Emerald, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_A_Name;", "Aleoida Laminiae - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_W_Name;", "Aleoida Laminiae - Grey", VariantColours.Grey, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_D_Name;", "Aleoida Laminiae - Indigo", VariantColours.Indigo, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_L_Name;", "Aleoida Laminiae - Lime", VariantColours.Lime, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_TTS_Name;", "Aleoida Laminiae - Mauve", VariantColours.Mauve, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_N_Name;", "Aleoida Laminiae - Ocher", VariantColours.Ocher, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_T_Name;", "Aleoida Laminiae - Sage", VariantColours.Sage, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_F_Name;", "Aleoida Laminiae - Teal", VariantColours.Teal, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_K_Name;", "Aleoida Laminiae - Turquoise", VariantColours.Turquoise, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Aleoids_04_B_Name;", "Aleoida Laminiae - Yellow", VariantColours.Yellow, StarType.B, PlanetMaterial.None);
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
