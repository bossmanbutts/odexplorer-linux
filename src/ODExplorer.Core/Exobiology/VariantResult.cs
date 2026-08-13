using EliteJournalReader;
using ODUtils.EliteDangerousHelpers;

namespace ODUtils.Exobiology;

public sealed class VariantResult
{
	public string VariantCodex;

	public string EnglishName;

	public VariantColours Colour;

	public PlanetMaterial Material;

	public StarType StarType;

	public VariantChance Chance { get; set; }

	public bool NewCodexEntry { get; set; }

	public VariantResult(Variant v)
	{
		VariantCodex = v.Codex;
		EnglishName = v.FullName;
		Colour = v.Colour;
		Material = v.Material;
		StarType = v.StarType;
	}

	public override string ToString()
	{
		if (Material == PlanetMaterial.None)
		{
			return $"{Colour} [{StarType}]";
		}
		return $"{Colour} [{Elements.GetSymbol(Material.ToString())}]";
	}
}
