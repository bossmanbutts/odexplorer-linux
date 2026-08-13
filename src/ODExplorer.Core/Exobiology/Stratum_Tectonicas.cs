using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Stratum_Tectonicas : SpeciesBase
{
	public override string Genus => "Stratum";

	public override string SpeciesCodex => "$Codex_Ent_Stratum_07_Name;";

	public override string SpeciesName => "Tectonicas";

	public override double MinGravity => 0.045;

	public override double MaxGravity => 0.607;

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
			span[index] = PlanetClass.HighMetalContentBody;
			return list;
		}
	}

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 9;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_oxygen_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_ammonia_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_rich_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_argon_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Stratum_07_TTS_Name;", "Stratum Tectonicas - Amethyst", VariantColours.Amethyst, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_F_Name;", "Stratum Tectonicas - Emerald", VariantColours.Emerald, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_M_Name;", "Stratum Tectonicas - Green", VariantColours.Green, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_T_Name;", "Stratum Tectonicas - Grey", VariantColours.Grey, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_Y_Name;", "Stratum Tectonicas - Indigo", VariantColours.Indigo, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_K_Name;", "Stratum Tectonicas - Lime", VariantColours.Lime, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_D_Name;", "Stratum Tectonicas - Mauve", VariantColours.Mauve, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_W_Name;", "Stratum Tectonicas - Red", VariantColours.Red, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_Ae_Name;", "Stratum Tectonicas - Teal", VariantColours.Teal, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Stratum_07_L_Name;", "Stratum Tectonicas - Turquoise", VariantColours.Turquoise, StarType.L, PlanetMaterial.None);
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
		AtmosphereDescription atmosphere = planet.Atmosphere;
		bool flag = ((atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere || (uint)(atmosphere - 74) <= 1u) ? true : false);
		if (flag && planet.SurfaceTemperature >= 450.0)
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_ammonia_atmosphere && planet.SurfaceTemperature >= 178.0)
		{
			return null;
		}
		atmosphere = planet.Atmosphere;
		flag = (uint)(atmosphere - 63) <= 1u;
		if (flag && planet.SurfaceTemperature >= 250.0)
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
