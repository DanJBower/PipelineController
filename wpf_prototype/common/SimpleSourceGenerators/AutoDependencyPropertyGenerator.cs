﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
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
            fullyQualifiedMetadataName: "CommonWpf.Attributes.AutoDependencyPropertyAttribute`1",
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
                string defaultValue;

                if (namedParameters.TryGetValue("DefaultValue", out var defaultValueInfo))
                {
                    defaultValue = $@",
            new System.Windows.PropertyMetadata({defaultValueInfo.ToCSharpString()})";
                }
                else if (namedParameters.TryGetValue("DefaultValueLiteral", out var defaultValueLiteralInfo))
                {
                    defaultValue = $@",
            new System.Windows.PropertyMetadata({defaultValueLiteralInfo.Value})";
                }
                else
                {
                    defaultValue = "";
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
            typeof(FourButtonPanel){defaultValue}
        );
");
            }

            partialClass.AppendLine("}");
            var partialClassContents = partialClass.ToString();
            sourceProductionContext.AddSource(fileName,
                SourceText.From(partialClassContents, Encoding.UTF8));
        });
    }
}
