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
        var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(AutoDependencyPropertyAttribute<>).FullName!,
            predicate: (node, _) => node is ClassDeclarationSyntax,
            transform: (syntaxContext, _) => syntaxContext.TargetNode as ClassDeclarationSyntax)
            .Where(static declaration => declaration is not null);

        // Note to future reader, without calling register line nothing happens. No idea why.
        // Spent ages debugging it with seemingly nothing happening.
        // Even broke out all the predicate and transform lines into a separate function and nothing
        context.RegisterSourceOutput(classDeclarations, (sourceProductionContext, classDeclarationSyntax) =>
        {

        });
    }
}
