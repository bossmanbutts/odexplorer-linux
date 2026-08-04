// Comprehensive stubs for ODUtils and related namespaces.
// These allow ViewModels to compile in ODExplorer.Core without the full ODUtils NuGet package.
// TODO: Replace by referencing the real ODUtils package once available on Linux/NuGet.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public event System.EventHandler? CanExecuteChanged;
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
        public System.Threading.Tasks.Task<System.Collections.Generic.List<ODExplorer.Models.EdAstroPoi>> GetPois()
            => System.Threading.Tasks.Task.FromResult(new System.Collections.Generic.List<ODExplorer.Models.EdAstroPoi>());
    }

    public sealed class EdsmApiService
    {
        public System.Threading.Tasks.Task<string?> GetSystemUrlAsync(long address)
            => System.Threading.Tasks.Task.FromResult<string?>(null);
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
        void AddCommander(ODUtils.Journal.JournalCommander commander);
        System.Threading.Tasks.Task<System.Collections.Generic.List<ODUtils.Journal.JournalCommander>> GetAllJournalCommanders(bool includeHidden);
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
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ODUtils.Helpers
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Helpers
{
    public static class EnumUtility
    {
        public static bool ContainsAllShipMaterials(object mats, object required) => false;
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
// ODUtils.Exobiology
// ────────────────────────────────────────────────────────────────────────────
namespace ODUtils.Exobiology
{
    public sealed class OrganicInfo
    {
        public string EnglishName { get; set; } = string.Empty;
        public long Value { get; set; }
        public int ColonyRange { get; set; }
    }

    public static class OrganicValues
    {
        public static readonly System.DateTime NewPriceDate = new(2023, 4, 25, 0, 0, 0, System.DateTimeKind.Utc);
    }

    public sealed class ExoData
    {
        public static readonly Dictionary<string, List<ODUtils.Models.GalacticRegions>> SpeciesRegions = new();
        public static ExoNames? GetNamesFromSpecies(string codex) => null;
    }

    public sealed class ExoNames
    {
        public string Genus { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
    }
}

// ────────────────────────────────────────────────────────────────────────────
// ToastNotifications.Position — already defined in ToastNotificationsStubs.cs
// ────────────────────────────────────────────────────────────────────────────

// ────────────────────────────────────────────────────────────────────────────
// ODExplorer.Database — IOdExplorerDBContextFactory interface stub
// ────────────────────────────────────────────────────────────────────────────
namespace ODExplorer.Database
{
    public interface IOdExplorerDBContextFactory
    {
        Microsoft.EntityFrameworkCore.DbContext CreateDbContext();
    }
}

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
    }
}

// ─── OdExplorerDatabaseProvider stub ─────────────────────────────────────────
namespace ODExplorer.Database
{
    public sealed class OdExplorerDatabaseProvider : ODUtils.Database.Interfaces.IOdToolsDatabaseProvider
    {
        private readonly System.Collections.Generic.List<ODUtils.Journal.JournalCommander> commanders = [];

        public void AddCommander(ODUtils.Journal.JournalCommander commander)
        {
            var existing = commanders.FirstOrDefault(x => x.Id == commander.Id);
            if (existing is not null)
            {
                commanders[commanders.IndexOf(existing)] = commander;
            }
            else
            {
                commanders.Add(commander);
            }
        }

        public ODUtils.Journal.JournalCommander? GetCommander(int id)
            => commanders.FirstOrDefault(x => x.Id == id);

        public System.Threading.Tasks.Task<System.Collections.Generic.List<ODUtils.Journal.JournalCommander>> GetAllJournalCommanders(bool includeHidden)
            => System.Threading.Tasks.Task.FromResult(commanders.ToList());

        public void DeleteCommander(int commanderId)
        {
            commanders.RemoveAll(x => x.Id == commanderId);
        }

        public void ClearCommanders() => commanders.Clear();

        public void AddEdAstroPois(System.Collections.Generic.List<ODExplorer.Models.EdAstroPoi> pois) { }
        public void AddIgnoreSystem(long address, string name, int commanderId) { }
        public void RemoveIgnoreSystem(long address, int commanderId) { }
        public System.Collections.Generic.List<ODExplorer.Models.IgnoredSystem> GetIgnoredSystems(int cmdrId)
            => new();
        public System.Threading.Tasks.Task DeleteCommanderAsync(int commanderID)
            => System.Threading.Tasks.Task.CompletedTask;
        public System.Threading.Tasks.Task ResetDataBaseAsync()
        {
            commanders.Clear();
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
