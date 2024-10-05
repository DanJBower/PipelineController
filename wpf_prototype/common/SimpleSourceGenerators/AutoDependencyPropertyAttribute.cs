using System;

namespace SimpleSourceGenerators;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class AutoDependencyPropertyAttribute<T> : Attribute
{
    public bool HasDefaultValue { get; set; } = true;
    public T DefaultValue { get; set; }
    public bool OnlyChangeIfDifferent { get; set; } = true;
    public bool IncludeOnChanging { get; set; } = true;
    public bool IncludeOnChanged { get; set; } = true;
    public string OnChangingNameOverride { get; set; }
    public string OnChangedNameOverride { get; set; }
}
