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
                // Try platform-specific clipboard helpers
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    var psi = new ProcessStartInfo("powershell", $"-Command \"Set-Clipboard -Value \"{text.Replace("\"", "\"\"")}\"\"") { UseShellExecute = false };
                    Process.Start(psi);
                    return;
                }

                // Try wl-copy (Wayland)
                try
                {
                    var psi = new ProcessStartInfo("wl-copy") { UseShellExecute = false, RedirectStandardInput = true };
                    var p = Process.Start(psi);
                    if (p != null)
                    {
                        p.StandardInput.Write(text);
                        p.StandardInput.Close();
                    }
                    return;
                }
                catch { }

                // Try xclip (X11)
                try
                {
                    var psi = new ProcessStartInfo("xclip", "-selection clipboard") { UseShellExecute = false, RedirectStandardInput = true };
                    var p = Process.Start(psi);
                    if (p != null)
                    {
                        p.StandardInput.Write(text);
                        p.StandardInput.Close();
                    }
                    return;
                }
                catch { }

                // macOS pbcopy
                try
                {
                    var psi = new ProcessStartInfo("pbcopy") { UseShellExecute = false, RedirectStandardInput = true };
                    var p = Process.Start(psi);
                    if (p != null)
                    {
                        p.StandardInput.Write(text);
                        p.StandardInput.Close();
                    }
                    return;
                }
                catch { }
            }
            catch
            {
                // swallow
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
