using Avalonia.Data.Converters;
using System;
using System.ComponentModel;
using System.Globalization;

namespace ODExplorer.UI.Avalonia.Converters;

/// <summary>Returns true when the bound value equals the converter parameter (used for state-driven visibility).</summary>
public sealed class EnumToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return parameter is null;
        }

        return value.Equals(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converts a collection/Count to visibility: returns true when the count is greater
/// than zero, or — when the parameter is "empty" — true when the count is zero.
/// </summary>
public sealed class CountToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = value switch
        {
            null => 0,
            int i => i,
            global::System.Collections.ICollection c => c.Count,
            _ => 0,
        };

        return string.Equals(parameter as string, "empty", StringComparison.OrdinalIgnoreCase)
            ? count == 0
            : count > 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Two-way converter that binds a CheckBox.IsChecked to a single bit of a [Flags] enum.
/// ConverterParameter is the flag value to test (e.g. {x:Static models:NotificationOptions.WorthMapping}).
/// </summary>
public sealed class EnumFlagConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            return false;
        }

        var valueBits = global::System.Convert.ToUInt64(value);
        var flagBits = global::System.Convert.ToUInt64(parameter);
        return (valueBits & flagBits) == flagBits && flagBits != 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is null)
        {
            return Activator.CreateInstance(targetType);
        }

        var flagBits = global::System.Convert.ToUInt64(parameter);
        var flag = Enum.ToObject(targetType, flagBits);
        var isChecked = value is bool b && b;

        var existing = Activator.CreateInstance(targetType);
        var existingBits = global::System.Convert.ToUInt64(existing ?? 0);

        return Enum.ToObject(targetType, isChecked ? existingBits | flagBits : existingBits & ~flagBits);
    }
}

/// <summary>Converts an enum value to its [Description] attribute text, falling back to ToString().</summary>
public sealed class EnumToDescriptionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum e)
        {
            return value?.ToString() ?? string.Empty;
        }

        var field = e.GetType().GetField(e.ToString());
        if (field is null)
        {
            return e.ToString();
        }

        var attr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attr?.Description ?? e.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
