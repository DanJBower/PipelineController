using System.Globalization;
using System.Windows.Media;

namespace CommonWpf.Converters;

/*public class ButtonActiveColorConverter : MarkupMultiValueConverter
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is [SolidColorBrush notPressedBrush, SolidColorBrush pressedBrush, bool isPressed])
        {
            return isPressed ? pressedBrush : notPressedBrush;
        }

        throw new ArgumentException($"Unexpected values passed to {nameof(ButtonActiveColorConverter)} converter", nameof(values));
    }
}*/

public class ButtonActiveColorConverter : MarkupValueConverter
{
    public SolidColorBrush PressedBrush { get; set; }
    public SolidColorBrush NotPressedBrush { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isPressed)
        {
            return isPressed ? PressedBrush : NotPressedBrush;
        }

        throw new ArgumentException($"Unexpected value passed to {nameof(ButtonActiveColorConverter)} converter", nameof(value));
    }
}