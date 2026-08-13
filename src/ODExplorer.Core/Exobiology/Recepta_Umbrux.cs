using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Recepta_Umbrux : SpeciesBase
{
	public override string Genus => "Recepta";

	public override string SpeciesCodex => "$Codex_Ent_Recepta_01_Name;";

	public override string SpeciesName => "Umbrux";

	public override double MinGravity => 0.0415;

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
			span[num2] = AtmosphereDescription.thin_oxygen_atmosphere;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 14;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_A_Name;", "Recepta Umbrux - Amethyst", VariantColours.Amethyst, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_N_Name;", "Recepta Umbrux - Emerald", VariantColours.Emerald, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_Ae_Name;", "Recepta Umbrux - Grey", VariantColours.Grey, StarType.AeBe, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_O_Name;", "Recepta Umbrux - Indigo", VariantColours.Indigo, StarType.O, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_Y_Name;", "Recepta Umbrux - Lime", VariantColours.Lime, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_M_Name;", "Recepta Umbrux - Maroon", VariantColours.Maroon, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_F_Name;", "Recepta Umbrux - Mauve", VariantColours.Mauve, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_L_Name;", "Recepta Umbrux - Ocher", VariantColours.Ocher, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_G_Name;", "Recepta Umbrux - Orange", VariantColours.Orange, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_K_Name;", "Recepta Umbrux - Red", VariantColours.Red, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_TTS_Name;", "Recepta Umbrux - Sage", VariantColours.Sage, StarType.TTS, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_T_Name;", "Recepta Umbrux - Teal", VariantColours.Teal, StarType.T, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_B_Name;", "Recepta Umbrux - Turquoise", VariantColours.Turquoise, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Recepta_01_D_Name;", "Recepta Umbrux - Yellow", VariantColours.Yellow, StarType.D, PlanetMaterial.None);
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
		if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => x.Name.StartsWith("Sulfur", StringComparison.OrdinalIgnoreCase) || x.Name.StartsWith("Sulphur", StringComparison.OrdinalIgnoreCase)).Percent < 1.05)
		{
			return null;
		}
		AtmosphereDescription atmosphere = planet.Atmosphere;
		bool flag = (uint)(atmosphere - 74) <= 1u;
		if (flag && !MathHelpers.DoubleBetweenMinEquals(132.0, 272.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere && !MathHelpers.DoubleBetweenMinEquals(151.0, 195.0, planet.SurfaceTemperature))
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_oxygen_atmosphere && !MathHelpers.DoubleBetweenMinEquals(151.0, 175.0, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
