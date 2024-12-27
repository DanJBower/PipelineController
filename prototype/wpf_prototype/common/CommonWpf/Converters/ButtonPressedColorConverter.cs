using ModernWpf.Controls.Primitives;
using System.Globalization;
using System.Windows.Media;

namespace CommonWpf.Converters;

public class ButtonActiveColorConverter : MarkupMultiValueConverter
{
    public override object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is [BindingProxy { Value: SolidColorBrush notPressedBrush }, SolidColorBrush pressedBrush, bool isPressed])
        {
            return isPressed ? pressedBrush : notPressedBrush;
        }

        // throw new ArgumentException($"Unexpected values passed to {nameof(ButtonActiveColorConverter)} converter", nameof(values));
        return null; // Occasionally designer will pass bad values at design time causing a crash.
                     // So return null instead of exception so it doesn't crash.
    }
}
