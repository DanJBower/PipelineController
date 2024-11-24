using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace SimpleSourceGenerators;

public static class Utilities
{
    public static string GetNamespace(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        var fullyQualifiedName = new StringBuilder("");

        var parent = classDeclarationSyntax.Parent;
        while (parent is not null)
        {
            switch (parent)
            {
                case NamespaceDeclarationSyntax namespaceDeclaration:
                    fullyQualifiedName.Insert(0, $"{namespaceDeclaration.Name}.");
                    break;
                case FileScopedNamespaceDeclarationSyntax fileScopedNamespace:
                    fullyQualifiedName.Insert(0, $"{fileScopedNamespace.Name}.");
                    break;
            }

            parent = parent.Parent;
        }

        return fullyQualifiedName.ToString().Remove(fullyQualifiedName.Length - 1);
    }

    public static string GetFullyQualifiedName(this ClassDeclarationSyntax classDeclarationSyntax)
    {
        var fullyQualifiedName = new StringBuilder(classDeclarationSyntax.Identifier.Text);

        var parent = classDeclarationSyntax.Parent;
        while (parent is not null)
        {
            switch (parent)
            {
                case NamespaceDeclarationSyntax namespaceDeclaration:
                    fullyQualifiedName.Insert(0, $"{namespaceDeclaration.Name}.");
                    break;
                case FileScopedNamespaceDeclarationSyntax fileScopedNamespace:
                    fullyQualifiedName.Insert(0, $"{fileScopedNamespace.Name}.");
                    break;
                case ClassDeclarationSyntax containingClass:
                    fullyQualifiedName.Insert(0, $"{containingClass.Identifier.Text}.");
                    break;
            }

            parent = parent.Parent;
        }

        return fullyQualifiedName.ToString();
    }

    public static Dictionary<string, TypedConstant> GetAttributeNamedParameters(this AttributeData attributeData)
    {
        Dictionary<string, TypedConstant> attributeInfo = [];

        foreach (var namedArgument in attributeData.NamedArguments)
        {
            attributeInfo[namedArgument.Key] = namedArgument.Value;
        }

        return attributeInfo;
    }

    public static string ToCSharpStringWithFixes(this TypedConstant typedConstant)
    {
        if (typedConstant.Type?.SpecialType is SpecialType.System_Single)
        {
            var val = (float)typedConstant.Value!;
            return $"{val:G99}f";
        }

        return typedConstant.ToCSharpString();
    }
}
