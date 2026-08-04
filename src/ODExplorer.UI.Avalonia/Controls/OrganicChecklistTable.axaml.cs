using Avalonia;
using Avalonia.Controls;
using ODExplorer.ViewModels.ModelVMs;
using System.Collections.Generic;

namespace ODExplorer.UI.Avalonia.Controls;

public partial class OrganicChecklistTable : UserControl
{
    public OrganicChecklistTable()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<OrganicChecklistTable, string>(nameof(Title), string.Empty);

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<IReadOnlyList<OrganicCheckListItemViewModel>?> SpeciesProperty =
        AvaloniaProperty.Register<OrganicChecklistTable, IReadOnlyList<OrganicCheckListItemViewModel>?>(nameof(Species));

    public IReadOnlyList<OrganicCheckListItemViewModel>? Species
    {
        get => GetValue(SpeciesProperty);
        set => SetValue(SpeciesProperty, value);
    }

    public static readonly StyledProperty<OrganicCheckListItemViewModel?> SelectedItemProperty =
        AvaloniaProperty.Register<OrganicChecklistTable, OrganicCheckListItemViewModel?>(nameof(SelectedItem));

    public OrganicCheckListItemViewModel? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}
