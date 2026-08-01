using System;

namespace ODExplorer.Adapters
{
    public sealed class NotificationModel
    {
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public int DisplaySeconds { get; init; } = 3;
    }

    public interface INotificationAdapter
    {
        void ShowToast(NotificationModel model);
    }

    public class NoOpNotificationAdapter : INotificationAdapter
    {
        public void ShowToast(NotificationModel model) { /* noop in core */ }
    }
}
