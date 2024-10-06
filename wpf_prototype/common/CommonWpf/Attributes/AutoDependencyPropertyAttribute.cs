namespace CommonWpf.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class AutoDependencyPropertyAttribute<T> : Attribute
{
    public required string Name { get; set; }
    public T? DefaultValue { get; set; }
    public string DefaultValueOverride { get; set; }
    public bool? IncludeOnChanging { get; set; }
    public bool? IncludeOnChanged { get; set; }
    public string? OnChangingNameOverride { get; set; }
    public string? OnChangedNameOverride { get; set; }
    public bool? SupportINotifyPropertyChanged { get; set; }
    public bool? SupportINotifyPropertyChanging { get; set; }
}
