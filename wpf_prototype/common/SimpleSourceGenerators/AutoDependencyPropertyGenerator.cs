using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimpleSourceGenerators;

// Rough debug instructions: https://github.com/JoanComasFdz/dotnet-how-to-debug-source-generator-vs2022
// Don't need to follow ReferenceOutputAssembly="false", it can be true
// Also, doesn't need to be a console app, any <TargetFramework>net8.0-windows</TargetFramework> app or class library.
//   ^ Doesn't have to be .NET 8, it's just this project is
// No longer need to restart visual studio for every change

[Generator]
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
            var classDeclarationSyntax = (ClassDeclarationSyntax)attributeSyntax.TargetNode;

            bool c = false;
        });
    }
}
