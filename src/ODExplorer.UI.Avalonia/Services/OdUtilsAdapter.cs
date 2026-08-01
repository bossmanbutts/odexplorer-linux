using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ODExplorer.Adapters;
using ODExplorer.Stubs;
using Avalonia;

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

        public void CopyToClipboard(string text)
        {
            try
            {
                var app = Application.Current;
                app?.Clipboard?.SetTextAsync(text);
            }
            catch
            {
                // fallback: nothing
            }
        }

        public void OpenUrl(string url)
        {
            try
            {
                var psi = new ProcessStartInfo(url) { UseShellExecute = true };
                Process.Start(psi);
            }
            catch
            {
                // fallback: nothing
            }
        }
    }
}
