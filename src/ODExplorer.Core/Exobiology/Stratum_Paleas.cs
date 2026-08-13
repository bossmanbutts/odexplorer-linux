using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Stratum_Paleas : SpeciesBase
{
	public override string Genus => "Stratum";

	public override string SpeciesCodex => "$Codex_Ent_Stratum_02_Name;";

	public override string SpeciesName => "Paleas";

	public override double MinGravity => 0.04;

	public override double MaxGravity => 0.585;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 165.0;

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
			int num = 5;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_oxygen_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_ammonia_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Stratum_02_TTS_Name;", "Stratum Paleas - Amethyst", VariantColours.Amethyst, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_F_Name;", "Stratum Paleas - Emerald", VariantColours.Emerald, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_M_Name;", "Stratum Paleas - Green", VariantColours.Green, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_T_Name;", "Stratum Paleas - Grey", VariantColours.Grey, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_Y_Name;", "Stratum Paleas - Indigo", VariantColours.Indigo, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_K_Name;", "Stratum Paleas - Lime", VariantColours.Lime, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_D_Name;", "Stratum Paleas - Mauve", VariantColours.Mauve, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_W_Name;", "Stratum Paleas - Red", VariantColours.Red, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_Ae_Name;", "Stratum Paleas - Teal", VariantColours.Teal, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_02_L_Name;", "Stratum Paleas - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
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
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere && planet.SurfaceTemperature >= 381.0)
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_ammonia_atmosphere && planet.SurfaceTemperature >= 177.0)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
