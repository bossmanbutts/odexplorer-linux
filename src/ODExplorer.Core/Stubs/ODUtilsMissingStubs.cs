// Comprehensive stubs for ODUtils and related namespaces.
// These allow ViewModels to compile in ODExplorer.Core without the full ODUtils NuGet package.
// TODO: Replace by referencing the real ODUtils package once available on Linux/NuGet.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Dialogs.ViewModels
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Dialogs.ViewModels
{
    public abstract class OdViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public virtual void Dispose() { }
    }
}

namespace ODUtils.Dialogs
{
    // Placeholder for any dialog-service types referenced from ViewModels.
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Commands
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Commands
{
    public sealed class RelayCommand : ICommand
    {
        private readonly System.Action<object?> execute;
        private readonly System.Func<object?, bool>? canExecute;
        public RelayCommand(System.Action<object?> execute, System.Func<object?, bool>? canExecute = null)
        { this.execute = execute; this.canExecute = canExecute; }
        public RelayCommand(System.Action execute) : this(_ => execute()) { }
        public event System.EventHandler? CanExecuteChanged;
        public bool CanExecute(object? p) => canExecute?.Invoke(p) ?? true;
        public void Execute(object? p) => execute(p);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public sealed class RelayCommand<T> : ICommand
    {
        private readonly System.Action<T?> execute;
        private readonly System.Func<T?, bool>? canExecute;
        public RelayCommand(System.Action<T?> execute, System.Func<T?, bool>? canExecute = null)
        { this.execute = execute; this.canExecute = canExecute; }
        public event System.EventHandler? CanExecuteChanged;
        public bool CanExecute(object? p) => canExecute?.Invoke(p is T t ? t : default) ?? true;
        public void Execute(object? p) => execute(p is T t ? t : default);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly System.Func<System.Threading.Tasks.Task> execute;
        private readonly System.Func<bool>? canExecute;
        public AsyncRelayCommand(System.Func<System.Threading.Tasks.Task> execute, System.Func<bool>? canExecute = null)
        { this.execute = execute; this.canExecute = canExecute; }
        public event System.EventHandler? CanExecuteChanged;
        public bool CanExecute(object? p) => canExecute?.Invoke() ?? true;
        public void Execute(object? p) => execute();
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }

    public sealed class AsyncRelayCommand<T> : ICommand
    {
        private readonly System.Func<T?, System.Threading.Tasks.Task> execute;
        private readonly System.Func<T?, bool>? canExecute;
        public AsyncRelayCommand(System.Func<T?, System.Threading.Tasks.Task> execute, System.Func<T?, bool>? canExecute = null)
        { this.execute = execute; this.canExecute = canExecute; }
        public event System.EventHandler? CanExecuteChanged;
        public bool CanExecute(object? p) => canExecute?.Invoke(p is T t ? t : default) ?? true;
        public void Execute(object? p) => execute(p is T t ? t : default);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.ViewModelNavigation
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.ViewModelNavigation
{
    public abstract class ViewModelBase : ODUtils.Dialogs.ViewModels.OdViewModelBase { }

    public sealed class OdNavigationStore
    {
        public ODUtils.Dialogs.ViewModels.OdViewModelBase? CurrentViewModel { get; private set; }
        public event System.Action? CurrentViewModelChanged;
        public void Navigate(ODUtils.Dialogs.ViewModels.OdViewModelBase vm)
        {
            CurrentViewModel = vm;
            CurrentViewModelChanged?.Invoke();
        }
    }

    public sealed class OdNavigationService<TViewModel> where TViewModel : ODUtils.Dialogs.ViewModels.OdViewModelBase
    {
        private readonly System.Func<TViewModel> factory;
        private readonly OdNavigationStore store;
        public OdNavigationService(OdNavigationStore store, System.Func<TViewModel> factory)
        { this.store = store; this.factory = factory; }
        public void Navigate() => store.Navigate(factory());
    }

    public sealed class OdNavigateCommand<TViewModel> : ICommand
        where TViewModel : ODUtils.Dialogs.ViewModels.OdViewModelBase
    {
        private readonly OdNavigationService<TViewModel> service;
        public OdNavigateCommand(OdNavigationService<TViewModel> service) => this.service = service;
#pragma warning disable CS0067
        public event System.EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object? p) => true;
        public void Execute(object? p) => service.Navigate();
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Journal
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Journal
{
    public sealed class JournalCommander
    {
        public JournalCommander() { }

        public JournalCommander(int id, string name, string? journalPath, string? lastFile, bool isHidden)
        {
            Id = id;
            Name = name;
            JournalPath = journalPath;
            LastFile = lastFile;
            IsHidden = isHidden;
        }

        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? JournalPath { get; set; }
        public string? LastFile { get; set; }
        public bool IsHidden { get; set; }
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.APis
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.APis
{
    public sealed class UpdateInfo
    {
        public System.Version Version { get; set; } = new(0, 0, 0);
        public string? DownloadUrl { get; set; }
        public string? ChangeLog { get; set; }
    }

    public sealed class EdAstroApiService
    {
        private const string GecUrl = "https://edastro.com/gec/json/all";

        public async Task<List<ODExplorer.Models.EdAstroPoi>> GetPois()
        {
            try
            {
                using var http = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(60) };
                http.DefaultRequestHeaders.UserAgent.ParseAdd("ODExplorer");
                var json = await http.GetStringAsync(GecUrl).ConfigureAwait(false);
                return ParsePois(json);
            }
            catch
            {
                return [];
            }
        }

        public static List<ODExplorer.Models.EdAstroPoi> ParsePois(string json)
        {
            var result = new List<ODExplorer.Models.EdAstroPoi>();
            if (string.IsNullOrWhiteSpace(json))
                return result;

            var array = JArray.Parse(json);
            foreach (var item in array)
            {
                var id = ReadInt(item, "id");
                if (id is null)
                    continue;

                double x = 0, y = 0, z = 0;
                if (item["coordinates"] is JArray coordinates && coordinates.Count >= 3)
                {
                    x = coordinates[0].ToObject<double>();
                    y = coordinates[1].ToObject<double>();
                    z = coordinates[2].ToObject<double>();
                }

                var poiUrl = item["poiUrl"]?.ToString();
                if (string.IsNullOrWhiteSpace(poiUrl))
                    poiUrl = $"https://edastro.com/gec/view/{id}";

                var dto = new ODExplorer.Database.DTOs.EdAstroPoiDTO
                {
                    Id = id.Value,
                    Name = item["name"]?.ToString() ?? string.Empty,
                    GalMapName = item["galMapSearch"]?.ToString() ?? string.Empty,
                    SystemAddress = ReadLong(item, "id64") ?? 0,
                    X = x,
                    Y = y,
                    Z = z,
                    Type = (int)ParseType(item["type"]?.ToString()),
                    Type2 = (int)ParseType(item["type2"]?.ToString()),
                    Summary = item["summary"]?.ToString() ?? string.Empty,
                    DistanceFromSol = ReadDouble(item, "solDistance") ?? 0,
                    PoiUrl = poiUrl,
                    MarkDown = item["descriptionMardown"]?.ToString() ?? string.Empty,
                };

                result.Add(new ODExplorer.Models.EdAstroPoi(dto));
            }

            return result;
        }

        private static ODUtils.Models.EdAstro.EDAstroType ParseType(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return ODUtils.Models.EdAstro.EDAstroType.Unknown;

            var normalized = Normalize(value);
            foreach (var member in Enum.GetNames<ODUtils.Models.EdAstro.EDAstroType>())
            {
                if (string.Equals(Normalize(member), normalized, StringComparison.Ordinal))
                    return Enum.Parse<ODUtils.Models.EdAstro.EDAstroType>(member);
            }

            return ODUtils.Models.EdAstro.EDAstroType.Unknown;
        }

        private static string Normalize(string value)
        {
            var buffer = new System.Text.StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (char.IsLetterOrDigit(c))
                    buffer.Append(char.ToLowerInvariant(c));
            }
            return buffer.ToString();
        }

        private static int? ReadInt(JToken? token, string name)
        {
            var t = token?[name];
            if (t is null) return null;
            if (t.Type == JTokenType.Integer) return t.ToObject<int>();
            if (t.Type == JTokenType.Float) return (int)t.ToObject<double>();
            if (t.Type == JTokenType.String && long.TryParse(t.ToString(), out var l)) return (int)l;
            return null;
        }

        private static long? ReadLong(JToken? token, string name)
        {
            var t = token?[name];
            if (t is null) return null;
            if (t.Type == JTokenType.Integer) return t.ToObject<long>();
            if (t.Type == JTokenType.Float) return (long)t.ToObject<double>();
            if (t.Type == JTokenType.String && long.TryParse(t.ToString(), out var l)) return l;
            return null;
        }

        private static double? ReadDouble(JToken? token, string name)
        {
            var t = token?[name];
            if (t is null) return null;
            if (t.Type == JTokenType.Float || t.Type == JTokenType.Integer) return t.ToObject<double>();
            if (t.Type == JTokenType.String && double.TryParse(t.ToString(), out var d)) return d;
            return null;
        }
    }

    public sealed class EdsmSystemValue
    {
        public System.Collections.Generic.List<EdsmBody> ValuableBodies { get; set; } = new();
        public string Url { get; set; } = string.Empty;
        public long EstimatedValueMapped { get; set; }
    }

    public sealed class EdsmBody
    {
        public long BodyId { get; set; }
        public string BodyName { get; set; } = string.Empty;
    }

    public class EdsmApiService
    {
        private const string SystemUrl = "https://www.edsm.net/api-v1/system";
        private const string SystemBodiesUrl = "https://www.edsm.net/api-v1/system-bodies";

        public virtual System.Threading.Tasks.Task<string?> GetSystemUrlAsync(long address)
            => System.Threading.Tasks.Task.FromResult<string?>(null);

        public virtual async Task<ODUtils.Models.StarType> GetPrimaryStarClassAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return ODUtils.Models.StarType.Unknown;

            try
            {
                var query = $"?systemName={Uri.EscapeDataString(systemName)}&showPrimaryStar=1&showInformation=1";
                var obj = await GetJsonObject(SystemUrl, query).ConfigureAwait(false);
                var type = obj?["primaryStar"]?["type"]?.ToString();
                if (string.IsNullOrWhiteSpace(type))
                    return ODUtils.Models.StarType.Unknown;

                return ODExplorer.Journal.JournalEventMapper.GetStarType(ReduceToSpectralClass(type));
            }
            catch
            {
                return ODUtils.Models.StarType.Unknown;
            }
        }

        public virtual async Task<EdsmSystemValue?> GetSystemValueAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            try
            {
                var query = $"?systemName={Uri.EscapeDataString(systemName)}&showInformation=1&showEstimatedValue=1&showBodies=1&showId=1";
                var obj = await GetJsonObject(SystemUrl, query).ConfigureAwait(false);
                if (obj is null)
                    return null;

                var value = new EdsmSystemValue
                {
                    Url = obj["url"]?.ToString() ?? string.Empty,
                    EstimatedValueMapped = ReadLong(obj, "estimatedValueMapped") ?? 0
                };

                if (obj["bodies"] is JArray bodies)
                {
                    foreach (var body in bodies)
                    {
                        // Bodies worth mapping/exobiology for the carto grid are the landable ones.
                        if (body["isLandable"]?.ToObject<bool>() != true)
                            continue;

                        value.ValuableBodies.Add(new EdsmBody
                        {
                            BodyId = ReadLong(body, "id") ?? 0,
                            BodyName = body["name"]?.ToString() ?? string.Empty
                        });
                    }
                }

                return value;
            }
            catch
            {
                return null;
            }
        }

        public virtual async Task<(int Count, int Scanned)> GetBodyCountAsync(long systemAddress)
        {
            if (systemAddress <= 0)
                return (0, 0);

            try
            {
                var query = $"?systemId64={systemAddress}&showId=1";
                var obj = await GetJsonObject(SystemBodiesUrl, query).ConfigureAwait(false);
                if (obj is null)
                    return (0, 0);

                int count = 0, scanned = 0;
                if (obj["bodies"] is JArray bodies)
                {
                    count = bodies.Count;
                    foreach (var body in bodies)
                    {
                        if (body["discovery"] is not null && body["discovery"]?.Type != JTokenType.Null)
                            scanned++;
                    }
                }

                return (count, scanned);
            }
            catch
            {
                return (0, 0);
            }
        }

        private static string? ReduceToSpectralClass(string type)
        {
            // EDSM often returns a bare class letter ("K") but sometimes "K7Va".
            if (char.IsLetter(type[0]) && type.Length > 1 && char.IsDigit(type[1]))
                return type[0].ToString();

            return type;
        }

        private static long? ReadLong(JToken? token, string name)
        {
            var t = token?[name];
            if (t is null) return null;
            if (t.Type == JTokenType.Integer) return t.ToObject<long>();
            if (t.Type == JTokenType.Float) return (long)t.ToObject<double>();
            if (t.Type == JTokenType.String && long.TryParse(t.ToString(), out var l)) return l;
            return null;
        }

        private static async Task<JObject?> GetJsonObject(string baseUrl, string query)
        {
            using var http = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("ODExplorer");
            var json = await http.GetStringAsync(baseUrl + query).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var obj = JObject.Parse(json);
            // EDSM returns { "error": true, "message": ... } for unknown systems.
            if (obj["error"]?.ToObject<bool>() == true)
                return null;

            return obj;
        }
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.IO
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.IO
{
    public static class Json
    {
        public static async Task<T?> GetJsonFromUrlAndDeserialise<T>(string baseUrl, string path)
        {
            try
            {
                using var http = new System.Net.Http.HttpClient();
                var json = await http.GetStringAsync(baseUrl + path).ConfigureAwait(false);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch { return default; }
        }
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Database.Interfaces
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Database.Interfaces
{
    public interface IOdToolsDatabaseProvider
    {
        System.Threading.Tasks.Task<System.Collections.Generic.List<ODUtils.Journal.JournalCommander>> GetAllJournalCommanders(bool includeHidden = false);
        ODUtils.Journal.JournalCommander AddCommander(ODUtils.Journal.JournalCommander commander);
        ODUtils.Journal.JournalCommander? GetCommander(int id);
        System.Collections.Generic.HashSet<string> GetAllReadFilenames();
        void AddJournalEntries(System.Collections.Generic.List<ODUtils.Journal.JournalEntry> journalEntries);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ODUtils.Journal.JournalEntry>> GetAllJournalEntries(int cmdrId);
        long GetMaxJournalOffset(int cmdrId, string filename);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ODUtils.Journal.JournalEntry>> GetJournalEntriesOfType(int cmdrId, System.Collections.Generic.List<ODUtils.Journal.JournalTypeEnum> types);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ODUtils.Journal.JournalEntry>> GetJournalEntriesOfType(int cmdrId, System.Collections.Generic.List<ODUtils.Journal.JournalTypeEnum> types, System.DateTime age);
        System.Threading.Tasks.Task GetJournalsStream(int cmdrId, System.Collections.Generic.List<ODUtils.Journal.JournalTypeEnum> types, System.DateTime age, System.Func<ODUtils.Journal.JournalEntry, System.Threading.Tasks.Task> callBack);
        System.Threading.Tasks.Task ParseJournalEventsOfType(int cmdrId, System.Collections.Generic.List<ODUtils.Journal.JournalTypeEnum> types, System.Action<ODUtils.Journal.JournalEntry> callback, System.DateTime age);
        System.Threading.Tasks.Task AddIgnoreSystem(long address, string name, int cmdrId);
        System.Threading.Tasks.Task RemoveIgnoreSystem(long address, int cmdrId);
        System.Collections.Generic.Dictionary<long, string> GetIgnoredSystemsDictionary(int cmdrId);
        System.Collections.Generic.List<ODExplorer.Models.IgnoredSystem> GetIgnoredSystems(int cmdrId);
        void AddEdAstroPois(System.Collections.Generic.List<ODExplorer.Models.EdAstroPoi> pois);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ODExplorer.Models.EdAstroPoi>> GetAstroPoisAsync();
        System.Collections.Generic.List<ODExplorer.Models.EdAstroPoi> GetAstroPois();
        System.Collections.Generic.List<ODUtils.Database.DTOs.SettingsDTO> GetAllSettings();
        void AddSettings(System.Collections.Generic.List<ODUtils.Database.DTOs.SettingsDTO> settings);
        void AddSetting(ODUtils.Database.DTOs.SettingsDTO settings);
        System.Threading.Tasks.Task ResetDataBaseAsync();
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Extensions
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Extensions
{
    public static class NumberExtensions
    {
        public static string FormatNumber(this long value) => value.ToString("N0");
        public static string FormatNumber(this double value) => value.ToString("N0");
    }

    public static class EnumExtensions
    {
        public static string GetEnumDescription(this System.Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null) return value.ToString();
            var attr = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            return attr.Length > 0 ? ((System.ComponentModel.DescriptionAttribute)attr[0]).Description : value.ToString();
        }

        public static string GetDescription(this System.Enum value) => GetEnumDescription(value);
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.EliteDangerousHelpers
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.EliteDangerousHelpers
{
    public static class BodyHelpers
    {
        public static string FormatMeters(double meters)
        {
            if (meters >= 1_000_000) return $"{meters / 1_000_000:N2} Mm";
            if (meters >= 1_000) return $"{meters / 1_000:N2} km";
            return $"{meters:N0} m";
        }

        // Great-circle distance in metres between two (latitude, longitude) points
        // on a sphere of the given radius (metres).
        public static double DistanceBetweenLongLats(double lat1, double lon1, double lat2, double lon2, double planetRadius)
        {
            const double DegToRad = Math.PI / 180;

            double dLat = (lat2 - lat1) * DegToRad;
            double dLon = (lon2 - lon1) * DegToRad;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(lat1 * DegToRad) * Math.Cos(lat2 * DegToRad)
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return 2 * planetRadius * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Helpers
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Helpers
{
    public static class EnumUtility
    {
        public static bool ContainsAllShipMaterials(ODUtils.Models.PlanetMaterial mats, ODUtils.Models.PlanetMaterial required)
            => (mats & required) == required;

        public static bool ContainsAllShipMaterials(EliteJournalReader.PlanetMaterial a, EliteJournalReader.PlanetMaterial b)
            => (a & b) == b;

        public static object[] GetValuesAndDescriptions(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<object>()
                .Select(value => (object)new
                {
                    Value = value,
                    Description = value.GetType().GetMember(value.ToString() ?? "?")[0]
                        .GetCustomAttributes(true).OfType<DescriptionAttribute>().First().Description
                })
                .ToArray();
        }

        public static string GetEnumDescription(Enum? enumObj)
        {
            if (enumObj == null)
                return string.Empty;

            var field = enumObj.GetType().GetField(enumObj.ToString());
            if (field == null)
                return string.Empty;

            var attribute = field.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
            return attribute?.Description ?? enumObj.ToString();
        }

        public static T? GetEnumValueFromDescription<T>(string description) where T : struct
        {
            foreach (var memberInfo in typeof(T).GetFields())
            {
                var attributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length != 0
                    && ((DescriptionAttribute)attributes[0]).Description.ToLower() == description.ToLower())
                {
                    return (T)Enum.Parse(typeof(T), memberInfo.Name);
                }
            }
            return default;
        }
    }

    public static class OperatingSystem
    {
        public static void OpenUrl(string url)
        {
            try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }); }
            catch { }
        }
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Exobiology — see Exobiology.cs for the full functional implementation
// ────────────────────────────────────────────────────────────────────────────

// ────────────────────────────────────────────────────────────────────────────
// ToastNotifications.Position — already defined in ToastNotificationsStubs.cs
// ────────────────────────────────────────────────────────────────────────────

// ─── App class stub ───────────────────────────────────────────────────────────
namespace ODExplorer
{
    public static class App
    {
        public static System.Version AppVersion { get; } = new(0, 0, 0, 0);
        public static NLog.Logger Logger { get; } = NLog.LogManager.GetCurrentClassLogger();
        public static AppCurrentShim Current { get; } = new();
    }

    public sealed class AppCurrentShim
    {
        public void Shutdown() { }
    }
}

// ─── PatchDates stub in ODExplorer.Models ────────────────────────────────────
namespace ODExplorer.Models
{
    public static class PatchDates
    {
        public static System.DateTime Type11PatchDate { get; } = new System.DateTime(2023, 4, 11);
        public static System.DateTime SquadCarrierPatchDate { get; } = new System.DateTime(2021, 4, 13);
    }
}

// ─── Spansh notification types ────────────────────────────────────────────────
namespace ODExplorer.Notifications
{
    public enum SpanshNotificationType
    {
        Refuel = 0,
    }
}
