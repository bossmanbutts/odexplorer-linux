using Avalonia.Data.Converters;
using Avalonia.Media;
using ODUtils.Models;
using System;
using System.Globalization;

namespace ODExplorer.UI.Avalonia.Converters;

/// <summary>
/// Maps an OrganicScanState to a row background/foreground brush.
/// ConverterParameter "Background" or "Foreground".
/// </summary>
public sealed class OrganicStateToBrushConverter : IValueConverter
{
    private static readonly IBrush Transparent = new SolidColorBrush(Colors.Transparent);
    private static readonly IBrush AnalysedBackground = new SolidColorBrush(Color.Parse("#5DA6E0"));
    private static readonly IBrush DisabledForeground = new SolidColorBrush(Color.Parse("#7FFFC500"));
    private static readonly IBrush DefaultForeground = new SolidColorBrush(Color.Parse("#FFC500"));
    private static readonly IBrush DarkForeground = new SolidColorBrush(Color.Parse("#1E1E1E"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var state = value is OrganicScanState s ? s : OrganicScanState.Unavailable;
        var isBackground = parameter as string == "Background";

        return state switch
        {
            OrganicScanState.Analysed => isBackground ? AnalysedBackground : DarkForeground,
            _ => isBackground ? Transparent : state == OrganicScanState.Unavailable ? DisabledForeground : DefaultForeground,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
