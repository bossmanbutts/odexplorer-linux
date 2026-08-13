using System.Collections.Generic;

namespace ODUtils.Exobiology;

public sealed class Frutexa : GenusBase
{
	public override string Codex => "$Codex_Ent_Shrubs_Genus_Name;";

	public override string Genus => "Frutexa";

	public override List<ExoPrediction>? GetExoPredictions(ExoPlanet planet)
	{
		return base.GetExoPredictions(planet);
	}
}
