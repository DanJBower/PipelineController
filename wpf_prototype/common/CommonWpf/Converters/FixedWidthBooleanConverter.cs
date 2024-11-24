using System.Globalization;

namespace CommonWpf.Converters;

public class FixedWidthBooleanConverter : MarkupValueConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "true " : "false";
    }
}
