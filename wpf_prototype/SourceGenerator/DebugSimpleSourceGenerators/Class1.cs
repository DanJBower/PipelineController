using SimpleSourceGenerators;
using System.Windows;
using System.Windows.Controls;

namespace DebugSimpleSourceGenerators;

[AutoDependencyProperty<string>(Name = "TestProp", DefaultValue = "Hi")]
[AutoDependencyProperty<string>(Name = "TestPropTwo", DefaultValueLiteral = "\"Test\"")]
public partial class SampleControl : UserControl
{
    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public string TemplateProp
    {
        get => (string)GetValue(TemplatePropProperty);
        set => SetValue(TemplatePropProperty, value);
    }

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    public static readonly System.Windows.DependencyProperty TemplatePropProperty
        = System.Windows.DependencyProperty.Register(
            name: nameof(TemplateProp),
            propertyType: typeof(string),
            ownerType: typeof(SampleControl),
            typeMetadata: new FrameworkPropertyMetadata(
                defaultValue: "Hi",
                propertyChangedCallback: OnTemplatePropChanged,
                coerceValueCallback: CoerceTemplateProp
            ),
            validateValueCallback: IsValidTemplateProp
        );

    public static bool IsValidTemplateProp(object? newTemplateProp)
    {
        var newTemplatePropValid = true;

        IsValidTemplateProp((string?)newTemplateProp, ref newTemplatePropValid);

        return newTemplatePropValid;
    }

    static partial void IsValidTemplateProp(string? newTemplateProp, ref bool newTemplatePropValid);

    private static void OnTemplatePropChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
    {
        OnTemplatePropChanged((SampleControl?)sender,
            changeArgs.Property,
            changeArgs.OldValue,
            changeArgs.NewValue);
    }

    static partial void OnTemplatePropChanged(SampleControl? sender,
        DependencyProperty? templatePropProperty,
        object? oldTemplatePropValue,
        object? newTemplatePropValue);

    private static object? CoerceTemplateProp(DependencyObject sender, object? value)
    {
        var newTemplateProp = (string?)value;
        CoerceTemplateProp((SampleControl?)sender, ref newTemplateProp);
        return newTemplateProp;
    }

    static partial void CoerceTemplateProp(SampleControl? sender, ref string? newTemplateProp);
}
