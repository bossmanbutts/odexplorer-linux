using Avalonia.Controls;
using Avalonia.Layout;
using ODUtils.Spansh;

namespace ODExplorer.UI.Avalonia.Views;

public partial class SpanshCSVSelectorWindow : Window
{
    public CsvType Result { get; private set; } = CsvType.None;

    public SpanshCSVSelectorWindow()
    {
        InitializeComponent();
        BuildButtons();
    }

    private void BuildButtons()
    {
        var types = new (string Label, CsvType Type)[]
        {
            ("Road to Riches", CsvType.RoadToRiches),
            ("Neutron Route", CsvType.NeutronRoute),
            ("World Type Route", CsvType.WorldTypeRoute),
            ("Tourist Route", CsvType.TouristRoute),
            ("Fleet Carrier", CsvType.FleetCarrier),
            ("Galaxy Plotter", CsvType.GalaxyPlotter),
            ("Exobiology", CsvType.Exobiology),
        };

        foreach (var (label, type) in types)
        {
            var button = new Button
            {
                Content = label,
                MinWidth = 150,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            button.Click += (_, _) =>
            {
                Result = type;
                Close();
            };

            ButtonsPanel.Children.Add(button);
        }

        var cancel = new Button
        {
            Content = "Cancel",
            MinWidth = 150,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        cancel.Click += (_, _) =>
        {
            Result = CsvType.None;
            Close();
        };

        ButtonsPanel.Children.Add(cancel);
    }
}
