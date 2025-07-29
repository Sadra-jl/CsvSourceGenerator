using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PaleLotus.CsvSourceGenerator;

internal sealed class TypeMetadata
{
    public string TypeName { get; }
    public string Namespace { get; }
    public string AccessModifier { get; }
    public string TypeKind { get; }
    public IReadOnlyList<string> Properties { get; }

    private TypeMetadata(
        string typeName,
        string @namespace,
        string accessModifier,
        string typeKind,
        IReadOnlyList<string> properties)
    {
        TypeName = typeName;
        Namespace = @namespace;
        AccessModifier = accessModifier;
        TypeKind = typeKind;
        Properties = properties;
    }

    public static TypeMetadata? FromSyntaxContext(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetNode is not BaseTypeDeclarationSyntax typeDeclarationSyntax)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol typeSymbol)
            return null;

        var properties = new List<string>();
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                properties.Add(member.Name);
            }
        }

        return new TypeMetadata(
            typeName: typeSymbol.Name,
            @namespace: typeDeclarationSyntax.GetNamespace(),
            accessModifier: GetAccessModifier(typeSymbol),
            typeKind: GetTypeKind(typeSymbol)
            ,properties: properties);
    }

    private static string GetTypeKind(INamedTypeSymbol symbol)
    {
        if (symbol.IsRecord) return "record";
        if (symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Class) return "class";
        return symbol.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct ? "struct" : "unknown";
    }

    private static string GetAccessModifier(INamedTypeSymbol symbol) =>
        symbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => "unknown"
        };
}
