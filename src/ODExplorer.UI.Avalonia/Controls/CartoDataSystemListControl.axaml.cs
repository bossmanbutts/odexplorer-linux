using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ODExplorer.ViewModels.ModelVMs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ODExplorer.UI.Avalonia.Controls;

public partial class CartoDataSystemListControl : UserControl
{
    public CartoDataSystemListControl()
    {
        InitializeComponent();
        SystemsProperty.Changed.AddClassHandler<CartoDataSystemListControl>((c, _) => c.OnSystemsChanged());
        SelectedSystemProperty.Changed.AddClassHandler<CartoDataSystemListControl>((c, _) => c.OnSelectedSystemChanged());
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private ObservableCollection<StarSystemViewModel>? observedSystems;

    public ObservableCollection<SystemBodyViewModel>? SelectedSystemBodies { get; private set; }

    public SystemBodyViewModel? SelectedBody
    {
        get => GetValue(SelectedBodyProperty);
        set => SetValue(SelectedBodyProperty, value);
    }

    public static readonly StyledProperty<SystemBodyViewModel?> SelectedBodyProperty =
        AvaloniaProperty.Register<CartoDataSystemListControl, SystemBodyViewModel?>(nameof(SelectedBody));

    public string TotalValue { get; private set; } = "0";

    public string IgnoreSystemText => SelectedSystem is null ? "No System Selected" : $"Add {SelectedSystem.Name} To Ignore List";

    public static readonly StyledProperty<ObservableCollection<StarSystemViewModel>?> SystemsProperty =
        AvaloniaProperty.Register<CartoDataSystemListControl, ObservableCollection<StarSystemViewModel>?>(nameof(Systems));

    public ObservableCollection<StarSystemViewModel>? Systems
    {
        get => GetValue(SystemsProperty);
        set => SetValue(SystemsProperty, value);
    }

    public static readonly StyledProperty<StarSystemViewModel?> SelectedSystemProperty =
        AvaloniaProperty.Register<CartoDataSystemListControl, StarSystemViewModel?>(nameof(SelectedSystem));

    public StarSystemViewModel? SelectedSystem
    {
        get => GetValue(SelectedSystemProperty);
        set => SetValue(SelectedSystemProperty, value);
    }

    public static readonly StyledProperty<bool> ShowIgnoreButtonProperty =
        AvaloniaProperty.Register<CartoDataSystemListControl, bool>(nameof(ShowIgnoreButton));

    public bool ShowIgnoreButton
    {
        get => GetValue(ShowIgnoreButtonProperty);
        set => SetValue(ShowIgnoreButtonProperty, value);
    }

    private void OnSystemsChanged()
    {
        Unsubscribe();
        observedSystems = Systems;
        Subscribe();
        RecomputeTotal();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        observedSystems = Systems;
        Subscribe();
        RecomputeTotal();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (observedSystems is not null)
        {
            observedSystems.CollectionChanged += OnSystemsCollectionChanged;
        }
    }

    private void Unsubscribe()
    {
        if (observedSystems is not null)
        {
            observedSystems.CollectionChanged -= OnSystemsCollectionChanged;
        }
    }

    private void OnSystemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RecomputeTotal();
    }

    private void RecomputeTotal()
    {
        TotalValue = (observedSystems?.Sum(x => x.DataValue) ?? 0).ToString("N0");
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(TotalValue)));
    }

    private void OnSelectedSystemChanged()
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(IgnoreSystemText)));

        if (SelectedSystem is null)
        {
            SelectedSystemBodies = null;
        }
        else
        {
            SelectedSystemBodies = new ObservableCollection<SystemBodyViewModel>(
                SelectedSystem.Bodies.Where(x => !string.Equals(x.Name, "BARYCENTRE", System.StringComparison.OrdinalIgnoreCase)));
            SelectedBody = SelectedSystemBodies.FirstOrDefault();
        }

        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedSystemBodies)));
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedBody)));
    }

    public new event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
