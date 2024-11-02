﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SimpleSourceGenerators;

// Rough debug instructions: https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022
// Doesn't need to be a console app to debug, any `<TargetFramework>net8.0-windows</TargetFramework>`
// app or class library will work.
//   ^ Doesn't have to be .NET 8, it's just this project is
// No longer need to restart visual studio for every change

// TODO: Look into using proper rosyln syntax to generate code rather than static strings.
// Will make things like the logic for FrameworkPropertyMetadata easier
// See other online sample for this like https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.Mvvm.SourceGenerators/ComponentModel/ObservablePropertyGenerator.Execute.cs

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

            var partialClass = new StringBuilder($@"// <auto-generated/>
#pragma warning disable
#nullable enable
namespace {classDeclarationSyntax.GetNamespace()};

/// <inheritdoc/>
// {generatedBy} // Don't actually include as per best practice? :)
partial class {classDeclarationSyntax.Identifier.Text}
{{
");

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
                    defaultValue = defaultValueInfo.ToCSharpString();
                    defaultSet = true;
                    includeFrameworkPropertyMetadata = true;
                }
                else if (namedParameters.TryGetValue("DefaultValueLiteral", out var defaultValueLiteralInfo))
                {
                    defaultValue = $"{defaultValueLiteralInfo.Value}";
                    defaultSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var validateValueCallback = "";
                if (namedParameters.TryGetValue("IncludeValidateValueCallback", out var includeValidateValueCallbackInfo) &&
                    includeValidateValueCallbackInfo.Value is true)
                {
                    validateValueCallback = "";
                }

                var propertyChangedCallbackSet = false;
                var propertyChangedCallback = "";
                if (namedParameters.TryGetValue("IncludePropertyChangedCallback", out var includePropertyChangedCallbackInfo) &&
                    includePropertyChangedCallbackInfo.Value is true)
                {
                    propertyChangedCallback = "";
                    propertyChangedCallbackSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var coerceValueCallbackSet = false;
                var coerceValueCallback = "";
                if (namedParameters.TryGetValue("IncludeCoerceValueCallback", out var includeCoerceValueCallbackInfo) &&
                    includeCoerceValueCallbackInfo.Value is true)
                {
                    coerceValueCallback = "";
                    coerceValueCallbackSet = true;
                    includeFrameworkPropertyMetadata = true;
                }

                var frameworkPropertyMetadata = "null";
                if (includeFrameworkPropertyMetadata)
                {
                    StringBuilder frameworkPropertyMetadataBuilder = new(@"new System.Windows.FrameworkPropertyMetadata(
                ");

                    if (defaultSet)
                    {
                        frameworkPropertyMetadataBuilder.Append($"                defaultValue: {defaultValue}");
                    }

                    if (propertyChangedCallbackSet)
                    {
                        frameworkPropertyMetadataBuilder.Append($"                defaultValue: {defaultValue}");
                    }

                    if (coerceValueCallbackSet)
                    {
                        frameworkPropertyMetadataBuilder.Append($"                defaultValue: {defaultValue}");
                    }

                    frameworkPropertyMetadataBuilder.Append(@"
            )");
                    frameworkPropertyMetadata = $"{frameworkPropertyMetadataBuilder}";
                }

                partialClass.AppendLine($@"    {generatedBy}
    {excludeFromCoverage}
    public {type} {dependencyPropertyFieldName}
    {{
        get => ({type})GetValue({dependencyPropertyName});
        set => SetValue({dependencyPropertyName}, value);
    }}

    {generatedBy}
    public static readonly System.Windows.DependencyProperty {dependencyPropertyName}
        = System.Windows.DependencyProperty.Register(
            nameof({dependencyPropertyFieldName}),
            typeof({type}),
            typeof({classDeclarationSyntax.Identifier.Text}),
            {frameworkPropertyMetadata}
        );
{validateValueCallback}{propertyChangedCallback}{coerceValueCallback}");
            }

            partialClass.AppendLine("}");
            var partialClassContents = partialClass.ToString();
            sourceProductionContext.AddSource(fileName,
                SourceText.From(partialClassContents, Encoding.UTF8));
        });
    }
}
