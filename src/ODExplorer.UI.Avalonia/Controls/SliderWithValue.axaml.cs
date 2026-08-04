using Avalonia;
using Avalonia.Controls;
using System.Globalization;

namespace ODExplorer.UI.Avalonia.Controls;

public partial class SliderWithValue : UserControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SliderWithValue, string>(nameof(Header), string.Empty);

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<SliderWithValue, double>(nameof(Minimum), 0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<SliderWithValue, double>(nameof(Maximum), 100);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<SliderWithValue, double>(nameof(Value), 0);

    public static readonly StyledProperty<double> TickFrequencyProperty =
        AvaloniaProperty.Register<SliderWithValue, double>(nameof(TickFrequency), 0);

    /// <summary>Numeric format for the value label, e.g. "N0", "N1", "N2".</summary>
    public static readonly StyledProperty<string> ValueStringFormatProperty =
        AvaloniaProperty.Register<SliderWithValue, string>(nameof(ValueStringFormat), "N0");

    public SliderWithValue()
    {
        InitializeComponent();
        ValueProperty.Changed.AddClassHandler<SliderWithValue>((o, e) => o.UpdateLabel());
        ValueStringFormatProperty.Changed.AddClassHandler<SliderWithValue>((o, e) => o.UpdateLabel());
    }

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double TickFrequency
    {
        get => GetValue(TickFrequencyProperty);
        set => SetValue(TickFrequencyProperty, value);
    }

    public string ValueStringFormat
    {
        get => GetValue(ValueStringFormatProperty);
        set => SetValue(ValueStringFormatProperty, value);
    }

    private void UpdateLabel()
    {
        if (ValueLabel is null)
        {
            return;
        }

        try
        {
            ValueLabel.Text = Value.ToString(ValueStringFormat, CultureInfo.CurrentCulture);
        }
        catch
        {
            ValueLabel.Text = Value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
