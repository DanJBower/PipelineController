namespace SimpleSourceGenerators;

using System.Windows;

#if NET8_0_OR_GREATER

using System.Windows.Data;

#endif

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class AutoDependencyPropertyAttribute<T> : Attribute
{
    public required string Name { get; set; }
    public T? DefaultValue { get; set; }
    public string? DefaultValueLiteral { get; set; }

    public bool? IncludeValidateValueCallback { get; set; }
    public bool? IncludePropertyChangedCallback { get; set; }
    public bool? IncludeCoerceValueCallback { get; set; }

#if NET8_0_OR_GREATER

    // TODO: Should probably go in it's own library that's not referenced by this
	//       so that source analyser doesn't have to target net 8?
	// TODO: Will have to work out how to Parse flags

    public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }

    public FrameworkPropertyMetadataOptions MetadataOptionFlags { get; set; }

#endif
}
