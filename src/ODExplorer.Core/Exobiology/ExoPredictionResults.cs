using System;

namespace ODUtils.Exobiology;

public struct ExoPredictionResults
{
	private readonly string _species;

	private readonly DateTime _timestamp;

	private readonly VariantColours _variants;

	public readonly string Species => _species;

	public readonly OrganicInfo Info => GetOrganicInfo(_species, _timestamp);

	public readonly VariantColours Variants => _variants;

	public ExoPredictionResults(string species, DateTime timestamp, VariantColours variants)
	{
		_species = species;
		_timestamp = timestamp;
		_variants = variants;
	}

	private static OrganicInfo GetOrganicInfo(string codex, DateTime timestamp)
	{
		return OrganicValues.GetOrganicInfo(codex, string.Empty, timestamp);
	}
}
