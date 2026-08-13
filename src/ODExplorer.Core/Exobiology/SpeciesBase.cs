using System.Collections.Generic;
using EliteJournalReader;

namespace ODUtils.Exobiology;

public abstract class SpeciesBase
{
	public abstract string SpeciesCodex { get; }

	public abstract string SpeciesName { get; }

	public abstract string Genus { get; }

	public abstract double MinGravity { get; }

	public abstract double MaxGravity { get; }

	public abstract double MinPressure { get; }

	public abstract double MaxPressure { get; }

	public abstract double MinTemperature { get; }

	public abstract double MaxTemperature { get; }

	public abstract double MinDistance { get; }

	public abstract double MaxDistance { get; }

	public abstract List<PlanetClass> Planets { get; }

	public abstract List<AtmosphereDescription> Atmospheres { get; }

	public virtual List<Volcanism> Volcanism => new List<Volcanism>();

	public virtual List<GalacticRegions> Regions => ExoData.AllRegions;

	public abstract List<Variant> Variants { get; }

	public virtual ExoPrediction? GetPredictions(ExoPlanet planet)
	{
		List<Variant> list = new List<Variant>();
		foreach (Variant variant in Variants)
		{
			if (variant.CheckVariant(planet))
			{
				list.Add(variant);
			}
		}
		if (list.Count > 0)
		{
			OrganicInfo organicInfo = OrganicValues.GetOrganicInfo(SpeciesCodex, SpeciesName, planet.Timestamp);
			ExoPrediction exoPrediction = new ExoPrediction
			{
				SpeciesCodex = SpeciesCodex,
				SpeciesEnglishName = SpeciesName,
				ColonyRange = organicInfo.ColonyRange,
				Value = organicInfo.Value
			};
			foreach (Variant item in list)
			{
				exoPrediction.Variants.Add(new VariantResult(item)
				{
					Chance = VariantChance.Likely
				});
			}
			return exoPrediction;
		}
		foreach (Variant variant2 in Variants)
		{
			if (variant2.CheckUnlikelyVariant(planet))
			{
				list.Add(variant2);
			}
		}
		if (list.Count > 0)
		{
			OrganicInfo organicInfo2 = OrganicValues.GetOrganicInfo(SpeciesCodex, SpeciesName, planet.Timestamp);
			ExoPrediction exoPrediction2 = new ExoPrediction
			{
				SpeciesCodex = SpeciesCodex,
				SpeciesEnglishName = SpeciesName,
				ColonyRange = organicInfo2.ColonyRange,
				Value = organicInfo2.Value
			};
			foreach (Variant item2 in list)
			{
				exoPrediction2.Variants.Add(new VariantResult(item2)
				{
					Chance = VariantChance.Unlikely
				});
			}
			return exoPrediction2;
		}
		return null;
	}
}
