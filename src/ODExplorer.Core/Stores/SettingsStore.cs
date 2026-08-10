using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using ODExplorer.Models;
using ODUtils.Database.DTOs;
using ODUtils.Database.Interfaces;
using ODUtils.Models;

namespace ODExplorer.Stores
{
    // Simplify SettingsStore to remove direct WPF and external dependencies.
    // This refactor keeps persistence logic but replaces UI/platform types with core-friendly equivalents.
    public enum WindowState { Normal, Minimized, Maximized }

    public sealed class SettingsStore
    {
        public event EventHandler<bool>? MinimiseToTrayChaned;
        public event EventHandler? MinExoValueChanged;

        public SettingsStore(object odToolsDatabaseProvider)
        {
            // databaseProvider is intentionally typed as object to avoid hard dependency on ODUtils.
            databaseProvider = odToolsDatabaseProvider;
            Instance ??= this;
        }

        // Empty ctor used by the JSON persistence layer when restoring a snapshot.
        public SettingsStore()
        {
        }

        private readonly object databaseProvider;

        private static SettingsStore? instance;
        public static SettingsStore? Instance { get => instance; set => instance = value; }
        public int SelectedCommanderID { get; set; } = 0;
        // Use simple types for window position; ViewModels can map as needed.
        public ViewModels.ModelVMs.WindowPositionViewModel WindowPosition { get; set; } = new();
        public DateTime JournalAgeDateTime => DateTime.UtcNow;
        public Dictionary<int, List<Models.PopOutParams>> PopOutParams { get; set; } = new();

        // ── Settings properties used by ViewModels ──────────────────────────
        public SystemGridSettings SystemGridSetting { get; set; } = SystemGridSettings.DefaultValues();
        public NotificationSettings NotificationSettings { get; set; } = NotificationSettings.GetDefault();
        public NotificationOptions NotificationOptions { get; set; }
        public NotableNotificationOptions NotableSettings { get; set; } = new();
        public SpanshCSVSettings SpanshCSVSettings { get; set; } = new();
        public ExoBiologyViewState BiologyViewState { get; set; }
        public CartoDetailsViewState CartoDetailsViewState { get; set; }
        public CartoViewState CartoViewState { get; set; } = CartoViewState.DetailedExo;
        public CodexEntryHistory CodexEntryHistory { get; set; }
        public JournalLogAge JournalAge { get; set; }
        public GalacticRegions ExoCheckListRegion { get; set; }
        public double UiScale { get; set; } = 1.0;
        public GridSize CartoHorizontalGridSize { get; set; } = new();
        public GridSize CartoDetailedGridSize { get; set; } = new();
        public GridSize ExtendedBodyInfoGridSize { get; set; } = new();
        public GridSize CurrentExoGridSize { get; set; } = new();
        public DateTime IgnoredCartoDate { get; set; } = DateTime.MinValue;
        public DateTime IgnoredExoDate { get; set; } = DateTime.MinValue;
        public bool MinimiseToTray { get; set; }
        public bool DeveloperMode { get; set; }
        public bool OnBoardingComplete { get; set; }

        #region Persistance
        private const string SettingsRowId = "SettingsStoreState";

        public void LoadSettings()
        {
            try
            {
                if (databaseProvider is IOdToolsDatabaseProvider provider)
                {
                    var saved = provider.GetAllSettings().FirstOrDefault(x => x.Id == SettingsRowId);

                    if (saved?.StringValue is { Length: > 0 } json)
                    {
                        var restored = JsonSerializer.Deserialize<SettingsStore>(json);

                        if (restored != null)
                        {
                            foreach (var prop in typeof(SettingsStore).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                             .Where(p => p.CanRead && p.CanWrite))
                            {
                                prop.SetValue(this, prop.GetValue(restored));
                            }

                            WindowPosition ??= new ViewModels.ModelVMs.WindowPositionViewModel();
                            PopOutParams ??= new Dictionary<int, List<Models.PopOutParams>>();
                        }
                    }
                }
            }
            catch
            {
                // ignore malformed/legacy settings rows; keep defaults
            }

            if (WindowPosition.IsZero)
            {
                ResetWindowPosition();
            }
        }

        public void SaveSettings()
        {
            try
            {
                if (databaseProvider is IOdToolsDatabaseProvider provider)
                {
                    var json = JsonSerializer.Serialize(this);

                    provider.AddSettings(
                    [
                        new SettingsDTO
                        {
                            Id = SettingsRowId,
                            StringValue = json,
                        }
                    ]);
                }
            }
            catch
            {
                // ignore — provider not available or serialization failed
            }
        }
        #endregion

        #region Window Position 
        public void ResetWindowPosition()
        {
            ResetWindowPositionActual(WindowPosition);
        }

        public static void ResetWindowPositionActual(ViewModels.ModelVMs.WindowPositionViewModel windowPosition, double windowWidth = 1800, double windowHeight = 1050)
        {
            // Avoid SystemParameters/WPF. Use simple defaults across platforms.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Best effort: center using provided defaults
                var left = (1920.0 / 2) - (windowWidth / 2);
                var top = (1080.0 / 2) - (windowHeight / 2);

                windowPosition.Top = top;
                windowPosition.Left = left;
                windowPosition.Width = windowWidth > 1920 ? 1920 : windowWidth;
                windowPosition.Height = windowHeight > 1080 ? 1080 : windowHeight;
                windowPosition.State = Models.WindowState.Normal;
                return;
            }

            windowPosition.Top = 10;
            windowPosition.Left = 10;
            windowPosition.Width = 800;
            windowPosition.Height = 600;
            windowPosition.State = Models.WindowState.Normal;
        }
        #endregion

        #region Popouts
        public List<Models.PopOutParams> GetCommanderPopOutParams(int commanderId)
        {
            if (PopOutParams.TryGetValue(commanderId, out var outParams))
            {
                return outParams;
            }
            return new List<Models.PopOutParams>();
        }

        public Models.PopOutParams GetParams(Models.PopOutBase popOut, int knownCount, int commanderId)
        {
            var popOutParams = GetCommanderPopOutParams(commanderId);

            var count = popOutParams.Count(x => x.Title == popOut.Title);

            if (count == 0)
            {
                var ret = Models.PopOutParams.CreateParams(popOut, 1, true);
                ResetWindowPositionActual(ret.Position, 800, 450);
                popOutParams.Add(ret);
                return ret;
            }

            if (knownCount > 0)
            {
                var known = popOutParams.FirstOrDefault(x => x.Title == popOut.Title && x.Count == knownCount);

                if (known != null)
                {
                    return known;
                }
            }
            var haveParams = popOutParams.FirstOrDefault(x => x.Title == popOut.Title && x.Active == false);

            if (haveParams != null)
            {
                if (haveParams.Position.IsZero)
                    ResetWindowPositionActual(haveParams.Position, 800, 450);
                haveParams.Active = true;
                return haveParams;
            }

            haveParams = Models.PopOutParams.CreateParams(popOut, count + 1, true);
            if (haveParams.Position.IsZero)
                ResetWindowPositionActual(haveParams.Position, 800, 450);
            popOutParams.Add(haveParams);
            PopOutParams.TryAdd(commanderId, popOutParams);
            return haveParams;
        }

        public void SaveParams(Models.PopOutBase popOut, bool active, int commanderId)
        {
            var popOutParams = GetCommanderPopOutParams(commanderId);

            var known = popOutParams.FirstOrDefault(x => x.Title == popOut.Title && x.Count == popOut.Count);

            if (known != null)
            {
                known.UpdateParams(popOut, active);
                return;
            }

            known = Models.PopOutParams.CreateParams(popOut, popOut.Count, active);
            popOutParams.Add(known);
            PopOutParams.TryAdd(commanderId, popOutParams);
        }

        public event EventHandler? OnSystemGridSettingsUpdatedEvent;
        internal void OnSystemGridSettingsUpdated()
        {
            OnSystemGridSettingsUpdatedEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public void SetMinimiseToTray(bool value)
        {
            MinimiseToTray = value;
            MinimiseToTrayChaned?.Invoke(this, value);
        }

        internal void OnExoMinValueChanged()
        {
            MinExoValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public ActiveViewModel ActiveView { get; set; } = ActiveViewModel.Carto;
    }
}
