using System.Collections.Generic;

namespace ODUtils.Exobiology;

public sealed class Recepta : GenusBase
{
	public override string Codex => "$Codex_Ent_Recepta_Genus_Name;";

	public override string Genus => "Recepta";

	public override List<ExoPrediction>? GetExoPredictions(ExoPlanet planet)
	{
		return base.GetExoPredictions(planet);
	}
}
