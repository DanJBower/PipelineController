using System.Globalization;
using System.Windows.Data;

namespace CommonWpf.Converters;

public abstract class MarkupValueConverter : SimpleMarkupExtension, IValueConverter
{
    public abstract object Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
