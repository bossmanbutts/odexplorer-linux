using System.Collections.Generic;

namespace ODUtils.Exobiology;

internal abstract class ExobiologyGenus(string genus, string english)
{
	public string GenusFdevName { get; private set; } = genus;

	public string GenusEnglishName { get; private set; } = english;

	public abstract void GetResults(Dictionary<ExobiologyGenus, List<ExoPredictionResults>> dict, ExoPlanet planet);

	protected virtual void AddResult(ExobiologyGenus genus, Dictionary<ExobiologyGenus, List<ExoPredictionResults>> dict, ExoPredictionResults result)
	{
		if (result.Variants != 0)
		{
			if (dict.TryGetValue(genus, out var list))
			{
				list.Add(result);
			}
			else
			{
				dict.Add(genus, [result]);
			}
		}
	}
}
