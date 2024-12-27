using System.Globalization;
using System.Windows.Data;

namespace CommonWpf.Converters;

public class EnumSelectedConverter : MarkupValueConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? parameter : Binding.DoNothing;
    }
}
