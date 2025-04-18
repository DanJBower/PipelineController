﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SimpleSourceGenerators;

// Rough debug instructions: https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022
// Doesn't need to be a console app to debug, any `<TargetFramework>net8.0-windows</TargetFramework>`
// app or class library will work.
//   ^ Doesn't have to be .NET 8, it's just this project is
// No longer need to restart visual studio for every change

[Generator(LanguageNames.CSharp)]
public class AutoDependencyPropertyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxContexts = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "SimpleSourceGeneratorAttributes.AutoDependencyPropertyAttribute`1",
            predicate: (node, _) => node is ClassDeclarationSyntax,
            transform: (syntaxContext, _) => syntaxContext)
            .Where(attributeSyntax => attributeSyntax.TargetNode is ClassDeclarationSyntax);

        // Note to future reader, without calling register line nothing happens. No idea why.
        // Spent ages debugging it with seemingly nothing happening.
        // Even broke out all the predicate and transform lines into a separate function and nothing
        context.RegisterSourceOutput(syntaxContexts, (sourceProductionContext, attributeSyntax) =>
        {
            var generatedBy =
                $"[global::System.CodeDom.Compiler.GeneratedCode(\"{typeof(AutoDependencyPropertyGenerator).FullName}\"," +
                $" \"{typeof(AutoDependencyPropertyGenerator).Assembly.GetName().Version}\")]";
            var excludeFromCoverage = "[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]";

            var classDeclarationSyntax = (ClassDeclarationSyntax)attributeSyntax.TargetNode;
            var fileName = $"{classDeclarationSyntax.GetFullyQualifiedName()}.g.cs";

            List<string> generatedSections = [];

            foreach (var attributeData in attributeSyntax.Attributes)
            {
                var namedParameters = attributeData.GetAttributeNamedParameters();

                var dependencyPropertyFieldName = namedParameters["Name"].Value;
                var dependencyPropertyName = $"{dependencyPropertyFieldName}Property";

                var type = attributeData.AttributeClass!.TypeArguments.First().ToDisplayString();
                var includeFrameworkPropertyMetadata = false;
                var defaultSet = false;
                string defaultValue = "";

                if (namedParameters.TryGetValue("DefaultValue", out var defaultValueInfo))
                {
                    defaultValue = defaultValueInfo.ToCSharpStringWithFixes(castNonStandardNumericTypes: true);
                    defaultSet = true;
                    includeFrameworkPropertyMetadata = true;
                }
                else if (namedParameters.TryGetValue("DefaultValueLiteral", out var defaultValueLiteralInfo))
                {
                    defaultValue = $"{defaultValueLiteralInfo.Value}";
                    defaultSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var validateValueCallbackName = "null";
                var validateValueCallback = "";
                if (namedParameters.TryGetValue("IncludeValidateValueCallback", out var includeValidateValueCallbackInfo) &&
                    includeValidateValueCallbackInfo.Value is true)
                {
                    validateValueCallback = @$"

    {generatedBy}
    {excludeFromCoverage}
    public static bool IsValid{dependencyPropertyFieldName}(object? new{dependencyPropertyFieldName})
    {{
        var new{dependencyPropertyFieldName}Valid = true;

        IsValid{dependencyPropertyFieldName}((string?)new{dependencyPropertyFieldName}, ref new{dependencyPropertyFieldName}Valid);

        return new{dependencyPropertyFieldName}Valid;
    }}

    {generatedBy}
    static partial void IsValid{dependencyPropertyFieldName}(string? new{dependencyPropertyFieldName}, ref bool new{dependencyPropertyFieldName}Valid);";
                    validateValueCallbackName = $"IsValid{dependencyPropertyFieldName}";
                }

                var propertyChangedCallbackSet = false;
                var propertyChangedCallback = "";
                var propertyChangedCallbackName = "";
                if (namedParameters.TryGetValue("IncludePropertyChangedCallback", out var includePropertyChangedCallbackInfo) &&
                    includePropertyChangedCallbackInfo.Value is true)
                {
                    propertyChangedCallback = $@"

    {generatedBy}
    {excludeFromCoverage}
    private static void On{dependencyPropertyFieldName}Changed(System.Windows.DependencyObject sender, System.Windows.DependencyPropertyChangedEventArgs changeArgs)
    {{
        On{dependencyPropertyFieldName}Changed((SampleControl?)sender,
            changeArgs.Property,
            changeArgs.OldValue,
            changeArgs.NewValue);
    }}

    {generatedBy}
    static partial void On{dependencyPropertyFieldName}Changed(SampleControl? sender,
        System.Windows.DependencyProperty? {dependencyPropertyFieldName}Property,
        object? old{dependencyPropertyFieldName}Value,
        object? new{dependencyPropertyFieldName}Value);";
                    propertyChangedCallbackName = $"On{dependencyPropertyFieldName}Changed";
                    propertyChangedCallbackSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var coerceValueCallbackSet = false;
                var coerceValueCallback = "";
                var coerceValueCallbackName = "";
                if (namedParameters.TryGetValue("IncludeCoerceValueCallback", out var includeCoerceValueCallbackInfo) &&
                    includeCoerceValueCallbackInfo.Value is true)
                {
                    coerceValueCallback = @$"

    {generatedBy}
    {excludeFromCoverage}
    private static object? Coerce{dependencyPropertyFieldName}(System.Windows.DependencyObject sender, object? value)
    {{
        var new{dependencyPropertyFieldName} = (string?)value;
        Coerce{dependencyPropertyFieldName}((SampleControl?)sender, ref new{dependencyPropertyFieldName});
        return new{dependencyPropertyFieldName};
    }}

    {generatedBy}
    static partial void Coerce{dependencyPropertyFieldName}(SampleControl? sender, ref string? new{dependencyPropertyFieldName});";
                    coerceValueCallbackName = $"Coerce{dependencyPropertyFieldName}";
                    coerceValueCallbackSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var isAnimationProhibitedSet = false;
                var isAnimationProhibited = "";
                if (namedParameters.TryGetValue("IsAnimationProhibited", out var isAnimationProhibitedInfo))
                {
                    isAnimationProhibited = isAnimationProhibitedInfo.ToCSharpStringWithFixes();
                    isAnimationProhibitedSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var defaultUpdateSourceTriggerSet = false;
                var defaultUpdateSourceTrigger = "";
                if (namedParameters.TryGetValue("DefaultUpdateSourceTrigger", out var defaultUpdateSourceTriggerInfo))
                {
                    defaultUpdateSourceTrigger = defaultUpdateSourceTriggerInfo.ToCSharpStringWithFixes();
                    defaultUpdateSourceTriggerSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var metadataOptionFlagsSet = false;
                var metadataOptionFlags = "";
                if (namedParameters.TryGetValue("MetadataOptionFlags", out var metadataOptionFlagsInfo))
                {
                    metadataOptionFlags = metadataOptionFlagsInfo.ToCSharpStringWithFixes();
                    metadataOptionFlagsSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var frameworkPropertyMetadata = "null";
                if (includeFrameworkPropertyMetadata)
                {
                    List<string> frameworkProperties = [];

                    if (defaultSet)
                    {
                        frameworkProperties.Add($"defaultValue: {defaultValue}");
                    }

                    if (isAnimationProhibitedSet)
                    {
                        frameworkProperties.Add($"isAnimationProhibited: {isAnimationProhibited}");
                    }

                    if (propertyChangedCallbackSet)
                    {
                        frameworkProperties.Add($"propertyChangedCallback: {propertyChangedCallbackName}");
                    }

                    if (coerceValueCallbackSet)
                    {
                        frameworkProperties.Add($"coerceValueCallback: {coerceValueCallbackName}");
                    }

                    if (defaultUpdateSourceTriggerSet)
                    {
                        frameworkProperties.Add($"defaultUpdateSourceTrigger: {defaultUpdateSourceTrigger}");
                    }

                    if (metadataOptionFlagsSet)
                    {
                        frameworkProperties.Add($"flags: {metadataOptionFlags}");
                    }

                    frameworkPropertyMetadata = @$"new System.Windows.FrameworkPropertyMetadata(
                {string.Join(",\n                ", frameworkProperties)}
            )";
                }

                generatedSections.Add($@"{generatedBy}
    {excludeFromCoverage}
    public {type} {dependencyPropertyFieldName}
    {{
        get => ({type})GetValue({dependencyPropertyName});
        set => SetValue({dependencyPropertyName}, value);
    }}

    {generatedBy}
    public static readonly System.Windows.DependencyProperty {dependencyPropertyName}
        = System.Windows.DependencyProperty.Register(
            name: nameof({dependencyPropertyFieldName}),
            propertyType: typeof({type}),
            ownerType: typeof({classDeclarationSyntax.Identifier.Text}),
            typeMetadata: {frameworkPropertyMetadata},
            validateValueCallback: {validateValueCallbackName}
        );{validateValueCallback}{propertyChangedCallback}{coerceValueCallback}");
            }

            var partialClass = $@"// <auto-generated/>
#pragma warning disable
#nullable enable
namespace {classDeclarationSyntax.GetNamespace()};

// {generatedBy} // Don't actually include as per best practice :)
/// <inheritdoc/>
partial class {classDeclarationSyntax.Identifier.Text}
{{
    {string.Join("\n\n    ", generatedSections)}
}}
";

            sourceProductionContext.AddSource(fileName,
                SourceText.From(partialClass, Encoding.UTF8));
        });
    }
}
