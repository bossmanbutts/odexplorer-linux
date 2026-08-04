// Functional spansh.co.uk CSV parser, ported from ODeliteTracker's SpanshCSVParser.
// Spansh CSVs have no marker line - the first line is the header row. The route type is
// detected by matching that header against known templates (positionally, by column order).
// On an unmatched header ParseCsv returns null and the UI falls back to a manual type
// selector which drives ForceParse.

using ODExplorer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ODUtils.Spansh
{
    public static class SpanshCSVParser
    {
        // Header templates ordered to match the CsvType enum values (positional mapping).
        private static readonly List<string[]> csvHeaders =
        [
            ["System Name", "Body Name", "Body Subtype", "Is Terraformable", "Distance To Arrival", "Estimated Scan Value", "Estimated Mapping Value", "Jumps"], // RoadToRiches
            ["System Name", "Distance", "Distance Remaining", "Tritium in tank", "Tritium in market", "Fuel Used", "Icy Ring", "Pristine", "Restock Tritium"],   // FleetCarrier
            ["System Name", "Distance To Arrival", "Distance Remaining", "Neutron Star", "Jumps"],                                                              // NeutronRoute
            ["System Name", "Distance", "Distance Remaining", "Fuel Left", "Fuel Used", "Refuel", "Neutron Star", "Inject"],                                     // GalaxyPlotter
            ["System Name", "Body Name", "Distance To Arrival", "Jumps"],                                                                                        // WorldTypeRoute
            ["System Name", "Jumps"],                                                                                                                            // TouristRoute
            ["System Name", "Body Name", "Body Subtype", "Distance To Arrival", "Landmark Subtype", "Value", "Count", "Jumps"],                                 // Exobiology
            ["System Name", "Body Name", "Body Subtype", "Distance To Arrival", "Landmark Type", "Value", "Jumps"],                                              // ExobiologyOld
            ["System Name", "Distance", "Distance Remaining"],                                                                                                   // Colonisation
            ["System Name", "Distance", "Distance Remaining", "Fuel Left", "Fuel Used", "Refuel", "Neutron Star"],                                               // GalaxyPlotterOld
        ];

        private static readonly CultureInfo enGB = CultureInfo.CreateSpecificCulture("en-GB");

        public static SpanshCsvContainer? ParseCsv(string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename) || File.Exists(filename) == false)
                {
                    return null;
                }

                var (csvType, result) = CheckCsvType(filename);

                if (result == false || csvType == CsvType.None)
                {
                    return null;
                }

                return ParseInternal(filename, csvType);
            }
            catch
            {
                return null;
            }
        }

        public static SpanshCsvContainer? ForceParse(string filename, CsvType csvType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename) || File.Exists(filename) == false)
                {
                    return null;
                }

                return ParseInternal(filename, csvType);
            }
            catch
            {
                return null;
            }
        }

        private static (CsvType CsvType, bool Result) CheckCsvType(string filename)
        {
            foreach (var raw in File.ReadLines(filename))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith('#'))
                {
                    continue;
                }

                var header = SplitCsvLine(line);

                for (int i = 0; i < csvHeaders.Count; i++)
                {
                    if (HeaderEquals(header, csvHeaders[i]))
                    {
                        return ((CsvType)i, true);
                    }
                }

                break;
            }

            return (CsvType.None, false);
        }

        private static bool HeaderEquals(string[] actual, string[] expected)
        {
            if (actual.Length != expected.Length)
            {
                return false;
            }

            for (int i = 0; i < actual.Length; i++)
            {
                if (string.Equals(actual[i], expected[i], StringComparison.OrdinalIgnoreCase) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private static SpanshCsvContainer? ParseInternal(string filename, CsvType csvType)
        {
            var ret = new List<ExplorationTarget>();

            bool first = true;

            foreach (var raw in File.ReadLines(filename))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith('#'))
                {
                    continue;
                }

                if (first)
                {
                    first = false;
                    continue;
                }

                var fields = SplitCsvLine(line);

                switch (csvType)
                {
                    case CsvType.RoadToRiches:
                        ProcessRoadToRichesRoute(ret, fields);
                        break;
                    case CsvType.FleetCarrier:
                        ProcessFleetCarrierRoute(ret, fields);
                        break;
                    case CsvType.NeutronRoute:
                        ProcessNeutronRoute(ret, fields);
                        break;
                    case CsvType.GalaxyPlotter:
                    case CsvType.GalaxyPlotterOld:
                        ProcessGalaxyPlotterRoute(ret, fields);
                        break;
                    case CsvType.WorldTypeRoute:
                        ProcessWorldTypeRoute(ret, fields);
                        break;
                    case CsvType.TouristRoute:
                        ProcessTouristRoute(ret, fields);
                        break;
                    case CsvType.Exobiology:
                    case CsvType.ExobiologyOld:
                        ProcessExoRoute(ret, fields);
                        break;
                    case CsvType.Colonisation:
                        ProcessColonisationRoute(ret, fields);
                        break;
                }
            }

            if (ret.Count == 0)
            {
                return null;
            }

            return new SpanshCsvContainer(ret, 0)
            {
                CsvType = csvType,
            };
        }

        private static void ProcessRoadToRichesRoute(List<ExplorationTarget> ret, string[] fields)
        {
            string systemName = fields[0];
            ExplorationTarget? target = FindTarget(ret, systemName);

            if (target is null)
            {
                target = new ExplorationTarget { SystemName = systemName.ToUpperInvariant(), Property1 = Field(fields, 7), BodiesInfo = [] };
                ret.Add(target);
            }

            target.BodiesInfo!.Add(new BodiesInfo
            {
                Body = GetBodyName(Field(fields, 1), target.SystemName),
                Distance = $"{ParseN0(Field(fields, 4))} ls",
                Property1 = ParseN0(Field(fields, 6)),
            });
        }

        private static void ProcessFleetCarrierRoute(List<ExplorationTarget> ret, string[] fields)
        {
            string systemName = fields[0];
            ExplorationTarget? target = FindTarget(ret, systemName);

            if (target is null)
            {
                target = new ExplorationTarget
                {
                    SystemName = systemName.ToUpperInvariant(),
                    Property1 = ParseN0(Field(fields, 1)),
                    Property2 = ParseN0(Field(fields, 2)),
                    Property3 = GetRingInfo(Field(fields, 6), Field(fields, 7)),
                };
                ret.Add(target);
            }
        }

        private static void ProcessNeutronRoute(List<ExplorationTarget> ret, string[] fields)
        {
            ret.Add(new ExplorationTarget
            {
                SystemName = fields[0].ToUpperInvariant(),
                Property1 = ParseN0(Field(fields, 1)),
                Property2 = ParseN0(Field(fields, 2)),
                Property3 = Field(fields, 4),
                Property4 = Field(fields, 3),
            });
        }

        private static void ProcessGalaxyPlotterRoute(List<ExplorationTarget> ret, string[] fields)
        {
            ret.Add(new ExplorationTarget
            {
                SystemName = fields[0].ToUpperInvariant(),
                Property1 = ParseN0(Field(fields, 1)),
                Property2 = ParseN0(Field(fields, 2)),
                Property3 = Field(fields, 5),
                Property4 = Field(fields, 6),
            });
        }

        private static void ProcessWorldTypeRoute(List<ExplorationTarget> ret, string[] fields)
        {
            string systemName = fields[0];
            ExplorationTarget? target = FindTarget(ret, systemName);

            if (target is null)
            {
                target = new ExplorationTarget { SystemName = systemName.ToUpperInvariant(), BodiesInfo = [] };
                ret.Add(target);
            }

            target.BodiesInfo!.Add(new BodiesInfo
            {
                Body = GetBodyName(Field(fields, 1), target.SystemName),
                Distance = $"{ParseN0(Field(fields, 2))} ls",
                Property1 = ParseN0(Field(fields, 3)),
            });
        }

        private static void ProcessTouristRoute(List<ExplorationTarget> ret, string[] fields)
        {
            ret.Add(new ExplorationTarget
            {
                SystemName = fields[0].ToUpperInvariant(),
                Property1 = ParseN0(Field(fields, 1)),
            });
        }

        private static void ProcessExoRoute(List<ExplorationTarget> ret, string[] fields)
        {
            string systemName = fields[0];
            ExplorationTarget? target = FindTarget(ret, systemName);

            if (target is null)
            {
                target = new ExplorationTarget { SystemName = systemName.ToUpperInvariant(), BodiesInfo = [] };
                ret.Add(target);
            }

            target.BodiesInfo!.Add(new BodiesInfo
            {
                Body = GetBodyName(Field(fields, 1), target.SystemName),
                Distance = Field(fields, 4).ToUpperInvariant(),
                Property1 = ParseN0(Field(fields, 5)),
            });
        }

        private static void ProcessColonisationRoute(List<ExplorationTarget> ret, string[] fields)
        {
            string systemName = fields[0];
            ExplorationTarget? target = FindTarget(ret, systemName);

            if (target is null)
            {
                target = new ExplorationTarget
                {
                    SystemName = systemName.ToUpperInvariant(),
                    Property1 = ParseN0(Field(fields, 1)),
                    Property2 = ParseN0(Field(fields, 2)),
                };
                ret.Add(target);
            }
        }

        private static ExplorationTarget? FindTarget(List<ExplorationTarget> ret, string systemName)
        {
            return ret.Find(x => x.SystemName.Contains(systemName, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetBodyName(string bodyName, string systemName)
        {
            if (bodyName.Length > systemName.Length)
            {
                return bodyName.Substring(systemName.Length).TrimStart();
            }

            return bodyName.ToUpperInvariant();
        }

        private static string GetRingInfo(string v1, string v2)
        {
            bool bool1 = v1.Contains("Yes", StringComparison.OrdinalIgnoreCase);
            bool bool2 = v2.Contains("Yes", StringComparison.OrdinalIgnoreCase);

            if (bool1 == false)
            {
                return "No";
            }

            if (bool2)
            {
                return "Pristine";
            }

            return "Yes";
        }

        private static string Field(string[] fields, int index)
        {
            if (index < 0 || index >= fields.Length)
            {
                return string.Empty;
            }

            return fields[index];
        }

        private static string ParseN0(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, enGB, out var number))
            {
                return $"{number:N0}";
            }

            return value;
        }

        private static string[] SplitCsvLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == ',' && inQuotes == false)
                {
                    fields.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(ch);
                }
            }

            fields.Add(current.ToString().Trim());
            return fields.ToArray();
        }
    }
}
