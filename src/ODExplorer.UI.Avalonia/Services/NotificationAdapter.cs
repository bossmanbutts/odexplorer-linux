using System.Diagnostics;
using ODExplorer.Adapters;

namespace ODExplorer.UI.Avalonia.Services
{
    public class NotificationAdapter : INotificationAdapter
    {
        public void ShowToast(NotificationModel model)
        {
            try
            {
                var psi = new ProcessStartInfo("notify-send", $"\"{model.Title}\" \"{model.Message}\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(psi);
            }
            catch
            {
                // fallback: nothing
            }
        }
    }
}
