using System;
using System.Collections.Generic;
using System.Linq;

namespace ODExplorer.Stores
{
    // NotificationStore previously depended on ToastNotifications and WPF.
    // For core library we remove the direct dependency and make this a no-op/facade.
    public sealed class NotificationStore
    {
        private readonly SettingsStore settingsStore;

        public NotificationStore(SettingsStore settingsStore)
        {
            this.settingsStore = settingsStore;
        }

        internal void ChangeNotifierSetting(Models.NotificationSettings settings)
        {
            // No-op in core. UI layer should implement real notifier and subscribe to settings changes.
            if (settingsStore.NotificationSettings.NotificationsEnabled)
                ShowTestNotification();
        }

        internal void ShowTestNotification() { /* noop */ }
        internal void ShowWorthMappingNotification(object body) { /* noop */ }
        internal void ShowExoBioNotification(object item, string header) { /* noop */ }
        internal void ShowHighValueExoBodyNotification(string bodyName, string value, string bioCount) { /* noop */ }
        internal void ShowNewCodexEntriesNotification(string bodyName, Dictionary<string, bool> entries, string? currentSystemRegion) { /* noop */ }
        internal void ShowNewSpeciesEntriesNotification(string bodyName, Dictionary<string, bool> entries, string? currentSystemRegion) { /* noop */ }

        internal void CopyToClipBoard(string message)
        {
            // Attempt to use the host-provided OdUtils adapter to copy to clipboard
            try
            {
                ODExplorer.Adapters.OdUtilsAdapterProvider.Current?.CopyToClipboard(message);
            }
            catch
            {
                // swallow — UI may not have wired adapter
            }
        }

        internal void FleetCarrierNotification(string message) { /* noop */ }
        internal void EDSMValuableBodiesNotification(object system) { /* noop */ }
        internal void ShowSpanshNotification(object type) { /* noop */ }

        internal void CheckForNotableNotifications(object body) { /* noop */ }

        internal void Dispose() { }
    }
}
