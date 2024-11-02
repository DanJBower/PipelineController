using SimpleSourceGeneratorAttributes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DebugSimpleSourceGenerators;

[AutoDependencyProperty<string>(Name = "TestProp", DefaultValue = "Hi")]
[AutoDependencyProperty<string>(Name = "TestPropTwo", DefaultValueLiteral = "\"Test\"", DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default, MetadataOptionFlags = FrameworkPropertyMetadataOptions.None)]
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
                flags: FrameworkPropertyMetadataOptions.None,
                propertyChangedCallback: OnTemplatePropChanged,
                coerceValueCallback: CoerceTemplateProp,
                isAnimationProhibited: false,
                defaultUpdateSourceTrigger: UpdateSourceTrigger.Default
            ),
            validateValueCallback: IsValidTemplateProp
        );

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static bool IsValidTemplateProp(object? newTemplateProp)
    {
        var newTemplatePropValid = true;

        IsValidTemplateProp((string?)newTemplateProp, ref newTemplatePropValid);

        return newTemplatePropValid;
    }

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    static partial void IsValidTemplateProp(string? newTemplateProp, ref bool newTemplatePropValid);

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private static void OnTemplatePropChanged(DependencyObject sender, DependencyPropertyChangedEventArgs changeArgs)
    {
        OnTemplatePropChanged((SampleControl?)sender,
            changeArgs.Property,
            changeArgs.OldValue,
            changeArgs.NewValue);
    }

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    static partial void OnTemplatePropChanged(SampleControl? sender,
        DependencyProperty? templatePropProperty,
        object? oldTemplatePropValue,
        object? newTemplatePropValue);

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private static object? CoerceTemplateProp(DependencyObject sender, object? value)
    {
        var newTemplateProp = (string?)value;
        CoerceTemplateProp((SampleControl?)sender, ref newTemplateProp);
        return newTemplateProp;
    }

    [global::System.CodeDom.Compiler.GeneratedCode("SimpleSourceGenerators.AutoDependencyPropertyGenerator", "1.0.0.0")]
    static partial void CoerceTemplateProp(SampleControl? sender, ref string? newTemplateProp);
}
