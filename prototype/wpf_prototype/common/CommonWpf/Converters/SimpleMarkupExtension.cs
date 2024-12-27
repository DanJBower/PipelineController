using System.Windows.Markup;

namespace CommonWpf.Converters;

public abstract class SimpleMarkupExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
