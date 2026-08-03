using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ODExplorer.UI.Avalonia.Converters;

public class VisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ODExplorer.Models.Visibility { } v
            ? v == ODExplorer.Models.Visibility.Visible
            : true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
