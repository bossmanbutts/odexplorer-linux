namespace ODExplorer.Models
{
    // Raised by NotificationStore and rendered by the UI layer as a toast popup.
    public sealed record ToastMessage(string Title, string Message);
}
