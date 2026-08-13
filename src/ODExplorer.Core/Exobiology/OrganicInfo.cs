namespace ODUtils.Exobiology;

public class OrganicInfo(string englishName, long value, int colonyRange)
{
	public string EnglishName { get; } = englishName;

	public long Value { get; } = value;

	public int ColonyRange { get; } = colonyRange;
}
