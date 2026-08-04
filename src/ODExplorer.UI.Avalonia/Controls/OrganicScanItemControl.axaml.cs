using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ODExplorer.ViewModels.ModelVMs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ODExplorer.UI.Avalonia.Controls;

public partial class OrganicScanItemControl : UserControl, INotifyPropertyChanged
{
    public OrganicScanItemControl()
    {
        InitializeComponent();
        OrganicDetailsProperty.Changed.AddClassHandler<OrganicScanItemControl>((c, _) => c.OnOrganicDetailsChanged());
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private ObservableCollection<OrganicScanItemViewModel>? observedDetails;

    public static readonly StyledProperty<ObservableCollection<OrganicScanItemViewModel>?> OrganicDetailsProperty =
        AvaloniaProperty.Register<OrganicScanItemControl, ObservableCollection<OrganicScanItemViewModel>?>(nameof(OrganicDetails));

    public ObservableCollection<OrganicScanItemViewModel>? OrganicDetails
    {
        get => GetValue(OrganicDetailsProperty);
        set => SetValue(OrganicDetailsProperty, value);
    }

    public ObservableCollection<OrganicTotalsViewModel> Totals { get; } = [];
    public string TotalValue { get; private set; } = "0";
    public string TotalCount { get; private set; } = "0";
    public string TotalBonus { get; private set; } = "0";

    private void OnOrganicDetailsChanged()
    {
        Unsubscribe();
        observedDetails = OrganicDetails;
        Subscribe();
        BuildTotals();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        observedDetails = OrganicDetails;
        Subscribe();
        BuildTotals();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (observedDetails is not null)
        {
            observedDetails.CollectionChanged += OnOrganicDetailsCollectionChanged;
        }
    }

    private void Unsubscribe()
    {
        if (observedDetails is not null)
        {
            observedDetails.CollectionChanged -= OnOrganicDetailsCollectionChanged;
        }
    }

    private void OnOrganicDetailsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        BuildTotals();
    }

    private void BuildTotals()
    {
        Totals.Clear();

        if (observedDetails is null || observedDetails.Count == 0)
        {
            TotalValue = "0";
            TotalCount = "0";
            TotalBonus = "0";
            RaiseTotalsChanged();
            return;
        }

        foreach (var group in observedDetails.GroupBy(x => x.EnglishName).OrderBy(x => x.Key, System.StringComparer.OrdinalIgnoreCase))
        {
            var first = group.First();
            Totals.Add(new OrganicTotalsViewModel
            {
                EnglishName = group.Key.Contains("Unknown") ? first.SpeciesEnglish : group.Key,
                Count = group.Count(),
                Value = group.Sum(x => x.Value),
                Bonus = group.Sum(x => x.Bonus)
            });
        }

        TotalValue = Totals.Sum(x => x.Value).ToString("N0");
        TotalCount = Totals.Sum(x => x.Count).ToString("N0");
        TotalBonus = Totals.Sum(x => x.Bonus).ToString("N0");
        RaiseTotalsChanged();
    }

    private void RaiseTotalsChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Totals)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalValue)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalCount)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalBonus)));
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
}
