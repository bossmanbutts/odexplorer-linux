namespace ODUtils.Exobiology;

public sealed class ExoEnglishNames(string genus, string genusCodex, string species, string speciesCodex, string variant, string variantCodex)
{
	public string Genus { get; } = genus;

	public string GenusCodex { get; } = genusCodex;

	public string Species { get; } = species;

	public string SpeciesCodex { get; } = speciesCodex;

	public string Variant { get; } = variant;

	public string VariantCodex { get; } = variantCodex;
}
