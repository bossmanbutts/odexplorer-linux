using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Stratum_Frigus : SpeciesBase
{
	public override string Genus => "Stratum";

	public override string SpeciesCodex => "$Codex_Ent_Stratum_08_Name;";

	public override string SpeciesName => "Frigus";

	public override double MinGravity => 0.04;

	public override double MaxGravity => 0.55;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 190.0;

	public override double MaxTemperature => 0.0;

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
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_rich_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 10;
			List<GalacticRegions> list = new List<GalacticRegions>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<GalacticRegions> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = GalacticRegions.Codex_RegionName_3;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_7;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_15;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_30;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_32;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_33;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_34;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_36;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_38;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_39;
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
			span[num2] = new Variant("$Codex_Ent_Stratum_08_TTS_Name;", "Stratum Frigus - Amethyst", VariantColours.Amethyst, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_F_Name;", "Stratum Frigus - Emerald", VariantColours.Emerald, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_M_Name;", "Stratum Frigus - Green", VariantColours.Green, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_T_Name;", "Stratum Frigus - Grey", VariantColours.Grey, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_Y_Name;", "Stratum Frigus - Indigo", VariantColours.Indigo, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_K_Name;", "Stratum Frigus - Lime", VariantColours.Lime, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_D_Name;", "Stratum Frigus - Mauve", VariantColours.Mauve, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_W_Name;", "Stratum Frigus - Red", VariantColours.Red, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_Ae_Name;", "Stratum Frigus - Teal", VariantColours.Teal, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_08_L_Name;", "Stratum Frigus - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
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
		if (planet.SurfaceTemperature < MinTemperature)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
