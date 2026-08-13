using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Stratum_Excutitus : SpeciesBase
{
	public override string Genus => "Stratum";

	public override string SpeciesCodex => "$Codex_Ent_Stratum_01_Name;";

	public override string SpeciesName => "Excutitus";

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
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_oxygen_atmosphere;
			return list;
		}
	}

	public override List<GalacticRegions> Regions
	{
		get
		{
			int num = 8;
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
			span[num2] = GalacticRegions.Codex_RegionName_16;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_17;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_18;
			num2++;
			span[num2] = GalacticRegions.Codex_RegionName_35;
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
			span[num2] = new Variant("$Codex_Ent_Stratum_01_TTS_Name;", "Stratum Excutitus - Amethyst", VariantColours.Amethyst, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_F_Name;", "Stratum Excutitus - Emerald", VariantColours.Emerald, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_M_Name;", "Stratum Excutitus - Green", VariantColours.Green, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_T_Name;", "Stratum Excutitus - Grey", VariantColours.Grey, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_Y_Name;", "Stratum Excutitus - Indigo", VariantColours.Indigo, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_K_Name;", "Stratum Excutitus - Lime", VariantColours.Lime, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_D_Name;", "Stratum Excutitus - Mauve", VariantColours.Mauve, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_W_Name;", "Stratum Excutitus - Red", VariantColours.Red, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_Ae_Name;", "Stratum Excutitus - Teal", VariantColours.Teal, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_01_L_Name;", "Stratum Excutitus - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
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
		if (planet.Atmosphere == AtmosphereDescription.thin_oxygen_atmosphere && planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => x.Name.StartsWith("Sulfur", StringComparison.OrdinalIgnoreCase) || x.Name.StartsWith("Sulphur", StringComparison.OrdinalIgnoreCase)).Percent < 0.05)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
