using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using EliteJournalReader.Events;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Concha_Aureolas : SpeciesBase
{
	public override string Genus => "Concha";

	public override string SpeciesCodex => "$Codex_Ent_Conchas_02_Name;";

	public override string SpeciesName => "Aureolas";

	public override double MinGravity => 0.039;

	public override double MaxGravity => 0.2755;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0133;

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

	public override List<Variant> Variants
	{
		get
		{
			int num = 10;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_N_Name;", "Concha Aureolas - Emerald", VariantColours.Emerald, StarType.N, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_D_Name;", "Concha Aureolas - Green", VariantColours.Green, StarType.D, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_F_Name;", "Concha Aureolas - Grey", VariantColours.Grey, StarType.F, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_B_Name;", "Concha Aureolas - Indigo", VariantColours.Indigo, StarType.B, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_W_Name;", "Concha Aureolas - Lime", VariantColours.Lime, StarType.W, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_L_Name;", "Concha Aureolas - Orange", VariantColours.Orange, StarType.L, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_K_Name;", "Concha Aureolas - Red", VariantColours.Red, StarType.K, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_A_Name;", "Concha Aureolas - Teal", VariantColours.Teal, StarType.A, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_G_Name;", "Concha Aureolas - Turquoise", VariantColours.Turquoise, StarType.G, PlanetMaterial.None);
			num2++;
			span[num2] = new Variant("$Codex_Ent_Conchas_02_Y_Name;", "Concha Aureolas - Yellow", VariantColours.Yellow, StarType.Y, PlanetMaterial.None);
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
		if (planet.SurfacePressure >= MaxPressure)
		{
			return null;
		}
		if (planet.AtmosphereComposition.FirstOrDefault((ScanItemComponent x) => string.Equals(x.Name, "Ammonia", StringComparison.OrdinalIgnoreCase)).Percent < 100.0)
		{
			return null;
		}
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		return base.GetPredictions(planet);
	}
}
