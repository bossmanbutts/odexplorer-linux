using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ODExplorer.Models
{
    public interface IDispatcher
    {
        void Invoke(System.Action action);
    }

    public static class DispatcherHelper
    {
        // UI layer may set this to marshal actions to the UI thread. If null, actions run synchronously.
        public static IDispatcher? Current { get; set; }

        public static void Invoke(System.Action action)
        {
            if (Current != null)
            {
                Current.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }

    public partial class PropertyChangeNotify : INotifyPropertyChanged
    {
        // Declare the event
        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            DispatcherHelper.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }
    }
}
