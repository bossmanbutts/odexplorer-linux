// Functional spansh.co.uk CSV parser.
// Handles all the export types ODExplorer supports. Column layout is resolved by
// header name (not fixed position) because spansh has reordered columns over time
// (e.g. estimated_scan_value moved earlier in newer exports).

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ODUtils.Spansh
{
    public static class SpanshCSVParser
    {
        public static ODExplorer.Models.SpanshCsvContainer? ParseCsv(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || File.Exists(filename) == false)
            {
                return null;
            }

            var csvType = SniffCsvType(filename);
            if (csvType is null)
            {
                return null;
            }

            return ParseInternal(filename, csvType.Value);
        }

        public static ODExplorer.Models.SpanshCsvContainer? ForceParse(string filename, CsvType csvType)
        {
            if (string.IsNullOrWhiteSpace(filename) || File.Exists(filename) == false)
            {
                return null;
            }

            return ParseInternal(filename, csvType);
        }

        // Spansh CSVs start with a comment line naming the export type, e.g. "# Road to Riches".
        private static CsvType? SniffCsvType(string filename)
        {
            foreach (var raw in File.ReadLines(filename))
            {
                var line = raw.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                if (line.StartsWith('#'))
                {
                    return line.TrimStart('#').Trim().ToLowerInvariant() switch
                    {
                        "road to riches" => CsvType.RoadToRiches,
                        // Galactic Mapping exports share the Road to Riches column layout.
                        "galactic mapping" => CsvType.RoadToRiches,
                        "neutron route" => CsvType.NeutronRoute,
                        "world type route" => CsvType.WorldTypeRoute,
                        "tourist route" => CsvType.TouristRoute,
                        "fleet carrier" => CsvType.FleetCarrier,
                        "galaxy plotter" => CsvType.GalaxyPlotter,
                        "exobiology" => CsvType.Exobiology,
                        _ => null,
                    };
                }

                return null;
            }

            return null;
        }

        private static ODExplorer.Models.SpanshCsvContainer? ParseInternal(string filename, CsvType csvType)
        {
            var targets = new List<ExplorationTarget>();

            using var reader = new StreamReader(filename);

            Dictionary<string, int>? columns = null;

            while (reader.ReadLine() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                {
                    continue;
                }

                if (columns is null)
                {
                    columns = BuildColumnMap(SplitCsvLine(line));
                    continue;
                }

                var target = BuildTarget(csvType, SplitCsvLine(line), columns);
                if (target is not null)
                {
                    targets.Add(target);
                }
            }

            if (targets.Count == 0)
            {
                return null;
            }

            return new ODExplorer.Models.SpanshCsvContainer(targets, 0)
            {
                CsvType = csvType,
            };
        }

        private static Dictionary<string, int> BuildColumnMap(string[] headers)
        {
            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < headers.Length; i++)
            {
                var name = headers[i].Trim().ToLowerInvariant();
                if (name.Length != 0 && columns.ContainsKey(name) == false)
                {
                    columns.Add(name, i);
                }
            }

            return columns;
        }

        private static ExplorationTarget? BuildTarget(CsvType csvType, string[] row, Dictionary<string, int> columns)
        {
            var system = Get(columns, row, "system_name");
            if (string.IsNullOrEmpty(system))
            {
                return null;
            }

            var target = new ExplorationTarget { SystemName = system };

            switch (csvType)
            {
                case CsvType.RoadToRiches:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "body");
                    target.Property3 = Get(columns, row, "distance_to_arrival");
                    target.Property4 = Get(columns, row, "estimated_scan_value");
                    break;
                case CsvType.NeutronRoute:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "class");
                    target.Property3 = Get(columns, row, "scoopable");
                    target.Property4 = Get(columns, row, "estimated_scan_value");
                    break;
                case CsvType.WorldTypeRoute:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "world_type") ?? Get(columns, row, "class");
                    target.Property3 = Get(columns, row, "estimated_scan_value");
                    break;
                case CsvType.TouristRoute:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "body");
                    target.Property3 = Get(columns, row, "distance_to_arrival");
                    target.Property4 = Get(columns, row, "description");
                    break;
                case CsvType.FleetCarrier:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "body");
                    target.Property3 = Get(columns, row, "distance_to_arrival");
                    target.Property4 = Get(columns, row, "notes");
                    break;
                case CsvType.GalaxyPlotter:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "class");
                    target.Property3 = Get(columns, row, "refuel");
                    target.Property4 = Get(columns, row, "scoopable");
                    break;
                case CsvType.Exobiology:
                    target.Property1 = Get(columns, row, "distance_from_arrival");
                    target.Property2 = Get(columns, row, "body");
                    target.Property3 = Get(columns, row, "distance_to_arrival");
                    target.Property4 = Get(columns, row, "species") ?? Get(columns, row, "genus");
                    break;
            }

            var body = Get(columns, row, "body");
            if (string.IsNullOrEmpty(body) == false)
            {
                target.BodiesInfo =
                [
                    new BodiesInfo
                    {
                        Body = body,
                        Distance = Get(columns, row, "distance_to_arrival"),
                        Property1 = Get(columns, row, "estimated_scan_value") ?? Get(columns, row, "species"),
                    },
                ];
            }

            return target;
        }

        private static string? Get(Dictionary<string, int> columns, string[] row, string header)
        {
            if (columns.TryGetValue(header, out int index) && index >= 0 && index < row.Length)
            {
                return string.IsNullOrEmpty(row[index]) ? null : row[index].Trim();
            }

            return null;
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
