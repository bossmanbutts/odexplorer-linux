using System;
using Avalonia.Threading;
using ODExplorer.Models;

namespace ODExplorer.UI.Avalonia.Services
{
    public class DispatcherAdapter : IDispatcher
    {
        public void Invoke(Action action)
        {
            try
            {
                if (Dispatcher.UIThread.CheckAccess())
                {
                    action();
                }
                else
                {
                    Dispatcher.UIThread.Post(action);
                }
            }
            catch
            {
                // Fallback: run synchronously if dispatcher is not available
                action();
            }
        }
    }
}
