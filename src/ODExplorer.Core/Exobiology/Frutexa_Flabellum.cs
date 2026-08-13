using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Frutexa_Flabellum : SpeciesBase
{
	public override string Genus => "Frutexa";

	public override string SpeciesCodex => "$Codex_Ent_Shrubs_01_Name;";

	public override string SpeciesName => "Flabellum";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.133;

	public override double MinTemperature => 152.0;

	public override double MaxTemperature => 177.0;

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
			int num = 32;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_1;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_2;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_3;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_5;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_6;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_7;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_8;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_13;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_14;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_15;
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
			span[num2] = GalacticRegions.Codex_RegionName_27;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_29;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_30;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_31;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_32;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_33;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_34;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_35;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_36;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_37;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_38;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_39;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_40;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_41;
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
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_G_Name;", "Frutexa Flabellum - Emerald", VariantColours.Emerald, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_F_Name;", "Frutexa Flabellum - Green", VariantColours.Green, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_M_Name;", "Frutexa Flabellum - Grey", VariantColours.Grey, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_D_Name;", "Frutexa Flabellum - Indigo", VariantColours.Indigo, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_B_Name;", "Frutexa Flabellum - Lime", VariantColours.Lime, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_TTS_Name;", "Frutexa Flabellum - Mauve", VariantColours.Mauve, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_W_Name;", "Frutexa Flabellum - Orange", VariantColours.Orange, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_N_Name;", "Frutexa Flabellum - Red", VariantColours.Red, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_L_Name;", "Frutexa Flabellum - Teal", VariantColours.Teal, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Shrubs_01_O_Name;", "Frutexa Flabellum - Yellow", VariantColours.Yellow, StarType.O, PlanetMaterial.None);
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
		if (planet.SurfacePressure >= MaxPressure)
		{
			return null;
		}
		if (!MathHelpers.DoubleBetween(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
