using System.Collections.Generic;

namespace ODExplorer.Adapters
{
    // Minimal adapter to expose small helpers core needs from ODUtils.
    public interface IOdUtilsAdapter
    {
        // Parse a string into a GalacticRegions enum (core uses stub enum). Noop returns Unknown.
        ODExplorer.Stubs.GalacticRegions ParseGalacticRegion(string value);

        // Copy text to system clipboard (UI layer recommended implementation)
        void CopyToClipboard(string text);

        // Open a URL in the system browser
        void OpenUrl(string url);

        // Other helper methods can be added as needed.
    }

    // No-op implementation that returns safe defaults. UI can provide real implementation via DI.
    public class NoOpOdUtilsAdapter : IOdUtilsAdapter
    {
        public ODExplorer.Stubs.GalacticRegions ParseGalacticRegion(string value) => ODExplorer.Stubs.GalacticRegions.Unknown;
        public void CopyToClipboard(string text) { /* noop */ }
        public void OpenUrl(string url) { /* noop */ }
    }
}
