using System.Collections.Generic;

namespace ODUtils.Exobiology;

public sealed class Tussock : GenusBase
{
	public override string Codex => "$Codex_Ent_Tussocks_Genus_Name;";

	public override string Genus => "Tussock";

	public override List<ExoPrediction>? GetExoPredictions(ExoPlanet planet)
	{
		if (planet.BiologicalCount < 2)
		{
			return null;
		}
		return base.GetExoPredictions(planet);
	}
}
