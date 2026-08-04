using Avalonia.Controls;
using ODUtils.Models;
using System;

namespace ODExplorer.UI.Avalonia.Views;

public partial class OrganicView : UserControl
{
    public OrganicView()
    {
        InitializeComponent();

        RegionComboBox.ItemsSource = Enum.GetValues(typeof(GalacticRegions));
    }
}
