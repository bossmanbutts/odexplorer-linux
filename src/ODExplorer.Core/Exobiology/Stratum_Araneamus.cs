using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EliteJournalReader;
using ODUtils.Helpers;

namespace ODUtils.Exobiology;

public sealed class Stratum_Araneamus : SpeciesBase
{
	private readonly List<StarType> _acceptedStars;

	public override string Genus => "Stratum";

	public override string SpeciesCodex => "$Codex_Ent_Stratum_04_Name;";

	public override string SpeciesName => "Araneamus";

	public override double MinGravity => 0.26;

	public override double MaxGravity => 0.55;

	public override double MinPressure => 0.0;

	public override double MaxPressure => 0.0;

	public override double MinTemperature => 165.0;

	public override double MaxTemperature => 375.0;

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
			span[num2] = AtmosphereDescription.thin_sulfur_dioxide_atmosphere;
			num2++;
			span[num2] = AtmosphereDescription.thin_sulphur_dioxide_atmosphere;
			return list;
		}
	}

	public override List<Variant> Variants
	{
		get
		{
			int num = 1;
			List<Variant> list = new List<Variant>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Variant> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = new Variant("$Codex_Ent_Stratum_04_F_Name;", "Stratum Araneamus - Emerald", VariantColours.Emerald, StarType.F, PlanetMaterial.None);
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
		if (!MathHelpers.DoubleBetweenMinEquals(MinTemperature, MaxTemperature, planet.SurfaceTemperature))
		{
			return null;
		}
		if (!planet.ContainsStars(_acceptedStars))
		{
			return null;
		}
		VariantResult variantResult = null;
		StarType starType = StarType.F;
		StarType starType2 = planet.StarsInSystem.FirstOrDefault();
		if (starType2 != 0)
		{
			starType = starType2;
		}
		variantResult = new VariantResult(new Variant("$Codex_Ent_Stratum_04_F_Name;", "Stratum Araneamus - Emerald", VariantColours.Emerald, starType, PlanetMaterial.None));
		Variant variant = Variants[0];
		OrganicInfo organicInfo = OrganicValues.GetOrganicInfo(SpeciesCodex, SpeciesName, planet.Timestamp);
		ExoPrediction exoPrediction = new ExoPrediction
		{
			SpeciesCodex = SpeciesCodex,
			SpeciesEnglishName = SpeciesName,
			ColonyRange = organicInfo.ColonyRange,
			Value = organicInfo.Value
		};
		variantResult.Chance = VariantChance.Likely;
		exoPrediction.Variants.Add(variantResult);
		return exoPrediction;
	}

	public Stratum_Araneamus()
	{
		int num = 4;
		List<StarType> list = new List<StarType>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<StarType> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = StarType.B;
		num2++;
		span[num2] = StarType.A;
		num2++;
		span[num2] = StarType.N;
		span[num2 + 1] = StarType.T;
		_acceptedStars = list;
	}
}
