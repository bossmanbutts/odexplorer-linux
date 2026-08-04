// ODUtils.Journal pipeline protocol types used by the store implementations.
// Mirror the real ODUtils.Journal surface so the real stores can be dropped in later.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EliteJournalReader;

namespace ODUtils.Journal
{
    public enum JournalTypeEnum
    {
        Fileheader = 0,
        LoadGame,
        Location,
        FSDJump,
        CarrierJump,
        CarrierStats,
        CarrierLocation,
        CarrierJumpRequest,
        CarrierJumpCancelled,
        StartJump,
        FSDTarget,
        NavRoute,
        NavRouteClear,
        SupercruiseEntry,
        FSSDiscoveryScan,
        FSSAllBodiesFound,
        FSSBodySignals,
        ScanBaryCentre,
        Scan,
        SAAScanComplete,
        SAASignalsFound,
        SellExplorationData,
        MultiSellExplorationData,
        Disembark,
        Embark,
        Died,
        ApproachBody,
        CodexEntry,
        ScanOrganic,
        SellOrganicData,
        Other
    }

    public static class JournalTypeHelpers
    {
        public static JournalTypeEnum FromString(string? eventName)
        {
            return eventName switch
            {
                "Fileheader" => JournalTypeEnum.Fileheader,
                "LoadGame" => JournalTypeEnum.LoadGame,
                "Location" => JournalTypeEnum.Location,
                "FSDJump" => JournalTypeEnum.FSDJump,
                "CarrierJump" => JournalTypeEnum.CarrierJump,
                "CarrierStats" => JournalTypeEnum.CarrierStats,
                "CarrierLocation" => JournalTypeEnum.CarrierLocation,
                "CarrierJumpRequest" => JournalTypeEnum.CarrierJumpRequest,
                "CarrierJumpCancelled" => JournalTypeEnum.CarrierJumpCancelled,
                "StartJump" => JournalTypeEnum.StartJump,
                "FSDTarget" => JournalTypeEnum.FSDTarget,
                "NavRoute" => JournalTypeEnum.NavRoute,
                "NavRouteClear" => JournalTypeEnum.NavRouteClear,
                "SupercruiseEntry" => JournalTypeEnum.SupercruiseEntry,
                "FSSDiscoveryScan" => JournalTypeEnum.FSSDiscoveryScan,
                "FSSAllBodiesFound" => JournalTypeEnum.FSSAllBodiesFound,
                "FSSBodySignals" => JournalTypeEnum.FSSBodySignals,
                "ScanBaryCentre" => JournalTypeEnum.ScanBaryCentre,
                "Scan" => JournalTypeEnum.Scan,
                "SAAScanComplete" => JournalTypeEnum.SAAScanComplete,
                "SAASignalsFound" => JournalTypeEnum.SAASignalsFound,
                "SellExplorationData" => JournalTypeEnum.SellExplorationData,
                "MultiSellExplorationData" => JournalTypeEnum.MultiSellExplorationData,
                "Disembark" => JournalTypeEnum.Disembark,
                "Embark" => JournalTypeEnum.Embark,
                "Died" => JournalTypeEnum.Died,
                "ApproachBody" => JournalTypeEnum.ApproachBody,
                "CodexEntry" => JournalTypeEnum.CodexEntry,
                "ScanOrganic" => JournalTypeEnum.ScanOrganic,
                "SellOrganicData" => JournalTypeEnum.SellOrganicData,
                _ => JournalTypeEnum.Other
            };
        }
    }

    // Reconstructs the typed event-args object for a stored journal JSON line.
    public static class JournalWatcher
    {
        public static object GetEventData(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new object();
            return ODExplorer.Journal.JournalEventMapper.Map(json, string.Empty, 0)?.EventData ?? new object();
        }
    }

    public interface IProcessJournalLogs
    {
        void ParseJournalEvent(JournalEntry e);
        void ClearData();
        JournalHistoryArgs GetEventsToParse(DateTime defaultAge);
        void RunBeforeParsingLogs(int currentCmdrId);
        Task ParseHistoryStream(JournalEntry entry);
        Task ParseHistoryStream(IEnumerable<JournalEntry> journalEntries, int currentCmdrId);
        void ParseHistory(IEnumerable<JournalEntry> journalEntries, int commanderId);
        void Dispose();
    }

    public sealed class JournalHistoryArgs
    {
        public JournalHistoryArgs(IEnumerable<JournalTypeEnum> types, DateTime age, IProcessJournalLogs owner, Func<JournalEntry, Task> parseStream)
        {
            Types = types.ToList();
            Age = age;
            Owner = owner;
            ParseStream = parseStream;
        }

        public List<JournalTypeEnum> Types { get; }
        public DateTime Age { get; }
        public IProcessJournalLogs Owner { get; }
        public Func<JournalEntry, Task> ParseStream { get; }
    }

    public sealed class SystemInRoute
    {
        public long SystemAddress { get; set; }
        public string StarSystem { get; set; } = string.Empty;
        public string StarClass { get; set; } = string.Empty;
        public double[] StarPos { get; set; } = Array.Empty<double>();
    }

    public sealed class NavigationRoute
    {
        public List<SystemInRoute?> Route { get; set; } = [];
    }

    public sealed class StatusFileDestination
    {
        public long Body { get; set; }
        public string BodyName { get; set; } = string.Empty;
    }

    public sealed class StatusFileEvent
    {
        public string BodyName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double PlanetRadius { get; set; }
        public StatusFileDestination Destination { get; set; } = new();
        public bool IsOnFoot { get; set; }
    }
}
