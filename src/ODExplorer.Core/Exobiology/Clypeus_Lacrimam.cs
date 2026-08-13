using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Clypeus_Lacrimam : SpeciesBase
{
	public override string Genus => "Clypeus";

	public override string SpeciesCodex => "$Codex_Ent_Clypeus_01_Name;";

	public override string SpeciesName => "Lacrimam";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.054;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 0.0;

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
			int num = 2;
			List<AtmosphereDescription> list = new List<AtmosphereDescription>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<AtmosphereDescription> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = AtmosphereDescription.thin_carbon_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_water_atmosphere;
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
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_G_Name;", "Clypeus Lacrimam - Amethyst", VariantColours.Amethyst, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_Y_Name;", "Clypeus Lacrimam - Green", VariantColours.Green, StarType.Y, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_K_Name;", "Clypeus Lacrimam - Grey", VariantColours.Grey, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_D_Name;", "Clypeus Lacrimam - Lime", VariantColours.Lime, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_B_Name;", "Clypeus Lacrimam - Maroon", VariantColours.Maroon, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_F_Name;", "Clypeus Lacrimam - Mauve", VariantColours.Mauve, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_A_Name;", "Clypeus Lacrimam - Orange", VariantColours.Orange, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_L_Name;", "Clypeus Lacrimam - Teal", VariantColours.Teal, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_M_Name;", "Clypeus Lacrimam - Turquoise", VariantColours.Turquoise, StarType.M, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Clypeus_01_N_Name;", "Clypeus Lacrimam - Yellow", VariantColours.Yellow, StarType.N, PlanetMaterial.None);
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
		if (planet.SurfacePressure < MinPressure)
		{
			return null;
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_carbon_dioxide_atmosphere)
		{
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "CarbonDioxide", StringComparison.OrdinalIgnoreCase)).Percent < 97.65)
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(190.0, 196.0, planet.SurfaceTemperature))
			{
				return null;
			}
		}
		if (planet.Atmosphere == AtmosphereDescription.thin_water_atmosphere)
		{
			if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "Water", StringComparison.OrdinalIgnoreCase)).Percent < 100.0)
			{
				return null;
			}
			if (!MathHelpers.DoubleBetweenMinEquals(390.0, 455.0, planet.SurfaceTemperature))
			{
				return null;
			}
		}
		return base.GetPredictions(planet);
	}
}
