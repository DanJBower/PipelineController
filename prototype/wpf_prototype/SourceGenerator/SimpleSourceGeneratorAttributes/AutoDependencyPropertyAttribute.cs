using System.Windows;
using System.Windows.Data;

namespace SimpleSourceGeneratorAttributes;


[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class AutoDependencyPropertyAttribute<T> : Attribute
{
    public required string Name { get; set; }
    public T? DefaultValue { get; set; }
    public string? DefaultValueLiteral { get; set; }

    public bool IncludeValidateValueCallback { get; set; }
    public bool IncludePropertyChangedCallback { get; set; }
    public bool IncludeCoerceValueCallback { get; set; }

    public bool IsAnimationProhibited { get; set; }
    public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }
    public FrameworkPropertyMetadataOptions MetadataOptionFlags { get; set; }
}
