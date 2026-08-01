using System;
using System.Threading.Tasks;

namespace ODExplorer.Models
{
    // Core-independent message box event args. UI layer must map these enums to WPF/Avalonia equivalents.
    public enum MessageBoxButton
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }

    public sealed class MessageBoxEventArgsAsync(string title, string message, MessageBoxButton buttons, Func<Task>? callbackYes = null, Func<Task>? callbackNo = null)
    {
        public string Title { get; } = title;
        public string Message { get; } = message;
        public MessageBoxButton Buttons { get; } = buttons;
        public Func<Task>? CallbackYes { get; } = callbackYes;
        public Func<Task>? CallbackNo { get; } = callbackNo;
    }
}
