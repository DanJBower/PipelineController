using System.Globalization;

namespace CommonWpf.Converters;

public class JoystickPositionConverter : MarkupValueConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is float position)
        {
            return (position * 50.0) + 50.0;
        }

        return 50.0;
    }
}
