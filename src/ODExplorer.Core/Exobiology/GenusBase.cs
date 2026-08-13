using System.Collections.Generic;
using System.Linq;

namespace ODUtils.Exobiology;

public abstract class GenusBase
{
	public readonly List<SpeciesBase> Species = new List<SpeciesBase>();

	public abstract string Codex { get; }

	public abstract string Genus { get; }

	public virtual void Initialise(IEnumerable<SpeciesBase?> allSpecies)
	{
		Species.Clear();
		List<SpeciesBase> list = allSpecies.Where((SpeciesBase x) => x != null && x.Genus == Genus).ToList();
		foreach (SpeciesBase item in list)
		{
			if (item != null)
			{
				Species.Add(item);
			}
		}
	}

	public virtual List<ExoPrediction>? GetExoPredictions(ExoPlanet planet)
	{
		List<ExoPrediction> list = new List<ExoPrediction>();
		foreach (SpeciesBase item in Species)
		{
			ExoPrediction predictions = item.GetPredictions(planet);
			if (predictions != null)
			{
				predictions.GenusCodex = Codex;
				predictions.GenusEnglishName = Genus;
				list.Add(predictions);
			}
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}
}
