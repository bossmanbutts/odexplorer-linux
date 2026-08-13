using System.Collections.Generic;

namespace ODUtils.Exobiology;

public class ExoPrediction
{
	public string GenusEnglishName { get; set; } = string.Empty;

	public string GenusCodex { get; set; } = string.Empty;

	public string SpeciesEnglishName { get; set; } = string.Empty;

	public string SpeciesCodex { get; set; } = string.Empty;

	public List<VariantResult> Variants { get; set; } = new List<VariantResult>();

	public int ColonyRange { get; set; }

	public long Value { get; set; }

	public bool IsNewSpecies { get; set; }
}
