using ODExplorer.Adapters;
using ODExplorer.Stubs;

namespace ODExplorer.UI.Avalonia.Services
{
    public class OdUtilsAdapter : IOdUtilsAdapter
    {
        public GalacticRegions ParseGalacticRegion(string value)
        {
            if (System.Enum.TryParse<GalacticRegions>(value, true, out var g))
                return g;
            return GalacticRegions.Unknown;
        }
    }
}
