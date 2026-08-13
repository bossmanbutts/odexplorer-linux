using EliteJournalReader;

namespace ODUtils.Exobiology;

public abstract class VariantBase
{
	public abstract string Genus { get; }

	public abstract string Species { get; }

	public abstract string Codex { get; }

	public abstract string FullName { get; }

	public abstract VariantColours Colour { get; }

	public abstract StarType StarType { get; }

	public abstract PlanetMaterial Material { get; }

	public abstract bool CheckVariant(ExoPlanet planet, double distance);
}
