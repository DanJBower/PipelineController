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

    public static string ToCSharpStringWithFixes(this TypedConstant typedConstant, bool castNonStandardNumericTypes = false)
    {
        switch (typedConstant.Type?.SpecialType)
        {
            case SpecialType.System_Single:
                {
                    var val = (float)typedConstant.Value!;
                    return $"{val:G99}f";
                }
            case SpecialType.System_Double:
                {
                    var val = (double)typedConstant.Value!;
                    return $"{val:G99}d";
                }
            case SpecialType.System_Decimal:
                {
                    var val = (decimal)typedConstant.Value!;
                    return $"{val}m";
                }
            case SpecialType.System_UInt32:
                {
                    var val = (uint)typedConstant.Value!;
                    return $"{val}u";
                }
            case SpecialType.System_UInt64:
                {
                    var val = (ulong)typedConstant.Value!;
                    return $"{val}ul";
                }
            case SpecialType.System_Int64:
                {
                    var val = (long)typedConstant.Value!;
                    return $"{val}l";
                }
        }

        if (castNonStandardNumericTypes)
        {
            switch (typedConstant.Type?.SpecialType)
            {
                case SpecialType.System_SByte:
                    {
                        var val = (sbyte)typedConstant.Value!;
                        return $"(sbyte){val}";
                    }
                case SpecialType.System_Byte:
                    {
                        var val = (byte)typedConstant.Value!;
                        return $"(byte){val}";
                    }
                case SpecialType.System_Int16:
                    {
                        var val = (short)typedConstant.Value!;
                        return $"(short){val}";
                    }
                case SpecialType.System_UInt16:
                    {
                        var val = (ushort)typedConstant.Value!;
                        return $"(ushort){val}";
                    }
                case SpecialType.System_IntPtr:
                    {
                        var val = (nint)typedConstant.Value!;
                        return $"(nint){val}";
                    }
                case SpecialType.System_UIntPtr:
                    {
                        var val = (nuint)typedConstant.Value!;
                        return $"(nuint){val}";
                    }
            }
        }

        return typedConstant.ToCSharpString();
    }
}
