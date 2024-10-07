namespace SimpleSourceGenerators;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class AutoDependencyPropertyAttribute<T> : Attribute
{
    public required string Name { get; set; }
    public T? DefaultValue { get; set; }
    public string? DefaultValueLiteral { get; set; }

    public string? TestProp { get; set; } = "Yooo!";
}
