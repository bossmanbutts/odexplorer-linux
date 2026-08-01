using System.Collections.Generic;

namespace ODExplorer.Adapters
{
    // Minimal adapter to expose small helpers core needs from ODUtils.
    public interface IOdUtilsAdapter
    {
        // Parse a string into a GalacticRegions enum (core uses stub enum). Noop returns Unknown.
        ODExplorer.Stubs.GalacticRegions ParseGalacticRegion(string value);

        // Other helper methods can be added as needed.
    }

    // No-op implementation that returns safe defaults. UI can provide real implementation via DI.
    public class NoOpOdUtilsAdapter : IOdUtilsAdapter
    {
        public ODExplorer.Stubs.GalacticRegions ParseGalacticRegion(string value) => ODExplorer.Stubs.GalacticRegions.Unknown;
    }
}
