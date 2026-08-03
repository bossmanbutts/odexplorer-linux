using Avalonia.Controls;
using Avalonia.Threading;
using ODExplorer.Models;
using ODExplorer.UI.Avalonia.Views;

namespace ODExplorer.UI.Avalonia.Services;

public static class MessageBoxService
{
    public static void Show(Window? owner, MessageBoxEventArgsAsync args)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var dialog = new MessageBoxWindow(args);

            if (owner is not null)
            {
                dialog.ShowDialog(owner);
            }
            else
            {
                dialog.Show();
            }
        });
    }
}
