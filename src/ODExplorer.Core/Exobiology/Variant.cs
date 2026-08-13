using EliteJournalReader;

namespace ODUtils.Exobiology;

public sealed class Variant
{
	public string Codex { get; }

	public string FullName { get; }

	public VariantColours Colour { get; }

	public StarType StarType { get; }

	public StarType StarType2 { get; }

	public PlanetMaterial Material { get; }

	public Variant(string codex, string fullName, VariantColours colour, StarType starType, PlanetMaterial material, StarType starType2 = StarType.Unknown)
	{
		Codex = codex;
		FullName = fullName;
		Colour = colour;
		StarType = starType;
		StarType2 = starType2;
		Material = material;
	}

	public bool CheckVariant(ExoPlanet planet)
	{
		if (Material != 0 && !planet.ContainsMaterial(Material))
		{
			return false;
		}
		if (StarType != 0 && !planet.ContainsStar(StarType))
		{
			return false;
		}
		return true;
	}

	public bool CheckUnlikelyVariant(ExoPlanet planet)
	{
		if (StarType == StarType.Unknown)
		{
			return false;
		}
		return planet.ParentStars.Contains(StarType) || planet.ParentStars.Contains(StarType2);
	}
}
