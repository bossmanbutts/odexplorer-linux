using System;
using System.Collections.Generic;
using System.Linq;
using ODExplorer.Models;
using ODUtils.Models;

namespace ODExplorer.Stores
{
    // Facade between core stores and the UI toast host. Each Show* method formats
    // a ToastMessage and raises OnToast; the UI subscribes and renders it.
    public sealed class NotificationStore
    {
        private readonly SettingsStore settingsStore;

        public event Action<ToastMessage>? OnToast;

        public NotificationStore(SettingsStore settingsStore)
        {
            this.settingsStore = settingsStore;
        }

        private bool Enabled => settingsStore.NotificationSettings.NotificationsEnabled;

        public void Dispose()
        {
            OnToast = null;
        }

        private void Emit(string title, string message)
        {
            if (Enabled == false)
                return;

            OnToast?.Invoke(new ToastMessage(title, message));
        }

        public void ChangeNotifierSetting(NotificationSettings settings)
        {
            if (settings.NotificationsEnabled)
                ShowTestNotification();
        }

        public void ShowTestNotification()
        {
            Emit("OD Explorer", "Test notification");
        }

        public void ShowWorthMappingNotification(SystemBody body)
        {
            Emit("Worth Mapping", $"{body.BodyName} is worth mapping");
        }

        public void ShowExoBioNotification(OrganicScanItem item, string header)
        {
            var title = string.IsNullOrEmpty(header) ? "Exobiology" : header;
            var species = string.IsNullOrEmpty(item.SpeciesLocalised) ? item.SpeciesEnglish : item.SpeciesLocalised;
            var message = string.IsNullOrEmpty(species)
                ? $"New exobiology on {item.Body.BodyName}"
                : $"{species} on {item.Body.BodyName}";
            Emit(title, message);
        }

        public void ShowHighValueExoBodyNotification(string bodyName, string value, string bioCount)
        {
            Emit("Valuable Exobiology Body", $"{bodyName.ToUpper()}\n{value} - {bioCount}");
        }

        public void ShowNewCodexEntriesNotification(string bodyName, Dictionary<string, bool> entries, string? currentSystemRegion)
        {
            var newEntries = entries.Where(x => x.Value).Select(x => x.Key).ToList();
            var message = newEntries.Count == 0 ? bodyName.ToUpper() : string.Join("\n", newEntries);
            Emit("Possible New Personal Codex", $"{bodyName.ToUpper()}\n{message}");
        }

        public void ShowNewSpeciesEntriesNotification(string bodyName, Dictionary<string, bool> entries, string? currentSystemRegion)
        {
            var newEntries = entries.Where(x => x.Value).Select(x => x.Key).ToList();
            var message = newEntries.Count == 0 ? bodyName.ToUpper() : string.Join("\n", newEntries);
            Emit("Possible New Species Codex", $"{bodyName.ToUpper()}\n{message}");
        }

        public void CopyToClipBoard(string message)
        {
            // Host adapter performs the actual clipboard write.
            try
            {
                ODExplorer.Adapters.OdUtilsAdapterProvider.Current?.CopyToClipboard(message);
            }
            catch
            {
                // swallow — UI may not have wired adapter
            }

            if (settingsStore.NotificationOptions.HasFlag(NotificationOptions.CopyToClipboard))
            {
                Emit("Copied to Clipboard", message);
            }
        }

        public void FleetCarrierNotification(string message)
        {
            Emit("Fleet Carrier", message);
        }

        public void EDSMValuableBodiesNotification(StarSystem system)
        {
            Emit("Valuable Bodies", $"Valuable bodies found in {system.Name}");
        }

        public void ShowSpanshNotification(ODExplorer.Notifications.SpanshNotificationType type)
        {
            Emit("Spansh", type == ODExplorer.Notifications.SpanshNotificationType.Refuel ? "Refuel at next system" : "Spansh CSV update");
        }

        public void CheckForNotableNotifications(SystemBody body)
        {
            if (Enabled == false)
                return;

            var settings = settingsStore.NotableSettings;

            if (settings.BodyNotifications.HasFlag(BodyNotification.DiverseLife)
                && body.BiologicalSignals >= settings.DiverseLifeLimit)
            {
                Emit("Diverse Exobiology Body", $"{body.BodyName.ToUpper()}\n{body.BiologicalSignals} Signals");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.SmallPlanet) && body.Radius <= settings.SmallRadius)
            {
                Emit("Small Radius Body", $"{body.BodyName.ToUpper()}\nRadius: {FormatDistance(body.Radius)}");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.HighEccentricity) && body.Eccentricity >= settings.EccentricityMin)
            {
                Emit("High Eccentricity Body", $"{body.BodyName.ToUpper()}\nEccentricity: {body.Eccentricity:N4}");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.NestedMoon)
               && body.Parents?.Count > 1
               && body.Parents[0].Type == ParentType.Planet
               && body.Parents[1].Type == ParentType.Planet)
            {
                Emit("Nested Moon", body.BodyName.ToUpper());
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.FastRotation)
               && body.TidalLock == false
               && Math.Abs(body.RotationPeriod * 24) <= settings.FastRotationMin)
            {
                Emit("Fast Rotating Body", $"{body.BodyName.ToUpper()}\nPeriod: {Math.Abs(body.RotationPeriod * 24):N1} hours");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.FastOrbit)
                && Math.Abs(body.OrbitalPeriod * 24) <= settings.FastOrbit)
            {
                Emit("Body With Fast Orbit", $"{body.BodyName.ToUpper()}\nPeriod: {Math.Abs(body.OrbitalPeriod * 24):N1} hours");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.WideRings)
               && body.Rings?.Count > 0)
            {
                var rings = body.Rings.Where(x => !x.Name.Contains("Belt"));

                foreach (var ring in rings)
                {
                    var ringWidth = ring.OuterRad - ring.InnerRad;

                    if (ringWidth > body.Radius * 1000 * settings.RingWidthRadiusMultiplier)
                    {
                        Emit("Body With Wide Ring",
                            $"{body.BodyName.ToUpper()}\nWidth: {FormatDistance(ringWidth)}");
                    }
                }
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.ShepherdMoon)
                && body.Parents is not null && body.Parents.Count > 0
                && body.Parents[0].Type != ParentType.Null)
            {
                var parent = GetParent(body);

                var parentRings = parent?.Rings?.Where(x => !x.Name.Contains("Belt"))?.LastOrDefault();

                if (parentRings is { } pr && pr.OuterRad > body.SemiMajorAxis)
                {
                    Emit("Shepherd Moon", $"{body.BodyName.ToUpper()}\nOrbit inside parent ring");
                }
            }

            if (body.Landable == false)
            {
                return;
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.LandableTerraformable) && body.Terraformable)
            {
                var state = string.IsNullOrEmpty(body.TerraformState) ? "Terraformable" : body.TerraformState;
                Emit("Landable Terraformable Body", $"{body.BodyName.ToUpper()}\n{state}");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.LandableWithRings) && body.Rings?.Count > 0)
            {
                Emit("Landable Body With Rings", body.BodyName.ToUpper());
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.LandableHighGravity) && body.SurfaceGravity >= settings.HighSurfaceGravity)
            {
                Emit("Landable High Gravity Body", $"{body.BodyName.ToUpper()}\n{body.SurfaceGravity:N2} g");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.LandableLargeRadius) && body.Radius >= settings.LargeRadius)
            {
                Emit("Landable Large Radius Body", $"{body.BodyName.ToUpper()}\nRadius: {FormatDistance(body.Radius)}");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.BioSignals) && body.BiologicalSignals > 0)
            {
                var suffix = body.BiologicalSignals > 1 ? "Signals" : "Signal";
                Emit("Body with Biology Signals", $"{body.BodyName.ToUpper()}\n{body.BiologicalSignals} Biological {suffix}");
            }

            if (settings.BodyNotifications.HasFlag(BodyNotification.GeoSignals) && body.GeologicalSignals > 0)
            {
                var suffix = body.GeologicalSignals > 1 ? "Signals" : "Signal";
                Emit("Body with Geological Signals", $"{body.BodyName.ToUpper()}\n{body.GeologicalSignals} Geological {suffix}");
            }
        }

        #region Notification Helpers
        private static SystemBody? GetParent(SystemBody body)
        {
            var parentId = body.Parents?.FirstOrDefault()?.BodyID;

            if (parentId == null)
                return null;

            var parent = body.Owner.SystemBodies.FirstOrDefault(b => b.BodyID == parentId);

            if (parent == null)
                return null;

            return parent;
        }

        private string FormatDistance(double km)
        {
            return settingsStore.SystemGridSetting.DistanceUnit switch
            {
                Distance.Miles => $"{km * 0.62137:N0} mi",
                _ => $"{km:N0} km"
            };
        }
        #endregion
    }
}
