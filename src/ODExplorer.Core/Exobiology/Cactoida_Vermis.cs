using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Cactoida_Vermis : SpeciesBase
{
	public override string Genus => "Cactoida";

	public override string SpeciesCodex => "$Codex_Ent_Cactoid_03_Name;";

	public override string SpeciesName => "Vermis";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 0.0;

	public override double MaxTemperature => 0.0;

	public override double MinDistance => 0.0;

	public override double MaxDistance => 0.0;

	public override List<PlanetClass> Planets => new List<PlanetClass>();

	public override List<AtmosphereDescription> Atmospheres
	{
		get
		{
			int num = 3;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_M_Name;", "Cactoida Vermis - Amethyst", VariantColours.Amethyst, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_A_Name;", "Cactoida Vermis - Green", VariantColours.Green, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_O_Name;", "Cactoida Vermis - Grey", VariantColours.Grey, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_W_Name;", "Cactoida Vermis - Indigo", VariantColours.Indigo, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_L_Name;", "Cactoida Vermis - Mauve", VariantColours.Mauve, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_Y_Name;", "Cactoida Vermis - Ocher", VariantColours.Ocher, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_T_Name;", "Cactoida Vermis - Orange", VariantColours.Orange, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_TTS_Name;", "Cactoida Vermis - Red", VariantColours.Red, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_N_Name;", "Cactoida Vermis - Sage", VariantColours.Sage, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_G_Name;", "Cactoida Vermis - Teal", VariantColours.Teal, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_D_Name;", "Cactoida Vermis - Turquoise", VariantColours.Turquoise, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Cactoid_03_F_Name;", "Cactoida Vermis - Yellow", VariantColours.Yellow, StarType.F, PlanetMaterial.None);
			return list;
		}
	}

	public override ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		if (!MathHelpers.DoubleBetweenMinEquals(MinGravity, MaxGravity, planet.SurfaceGravity))
		{
			return null;
		}
		if (!Atmospheres.Contains(planet.Atmosphere))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_atmosphere && !MathHelpers.DoubleBetween(392.0, 450.0, planet.SurfaceTemperature))
		{
			return null;
		}
		AtmosphereDescription atmosphere = planet.Atmosphere;
		bool flag = (uint)(atmosphere - 74) <= 1u;
		if (flag && !MathHelpers.DoubleBetween(160.0, 165.0, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
