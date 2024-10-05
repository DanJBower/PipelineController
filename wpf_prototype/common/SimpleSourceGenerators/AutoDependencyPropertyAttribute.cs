using System;

namespace SimpleSourceGenerators;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class AutoDependencyPropertyAttribute<T> : Attribute
{
    public string Name { get; set; }
    public T DefaultValue { get; set; }
    public bool OnlyChangeIfDifferent { get; set; } = true;
    public bool IncludeOnChanging { get; set; } = true;
    public bool IncludeOnChanged { get; set; } = true;
    public string OnChangingNameOverride { get; set; }
    public string OnChangedNameOverride { get; set; }
    public bool SupportINotifyPropertyChanged { get; set; } = true;
    public bool SupportINotifyPropertyChanging { get; set; } = true;
}
