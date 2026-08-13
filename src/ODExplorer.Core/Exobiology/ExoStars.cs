using EliteJournalReader;

namespace ODUtils.Exobiology;

public readonly struct ExoStars
{
	public StarType Type { get; }

	public StarLuminosity Luminosity { get; }

	public ExoStars(StarType type, StarLuminosity luminosity)
	{
		Type = type;
		Luminosity = luminosity;
	}
}
