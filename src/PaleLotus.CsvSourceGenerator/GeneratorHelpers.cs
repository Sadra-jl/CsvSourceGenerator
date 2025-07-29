using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PaleLotus.CsvSourceGenerator;

public static class GeneratorHelpers
{
    public static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
    {
        var nameSpace = string.Empty;
        
        var potentialNamespaceParent = syntax.Parent;
        
        while (potentialNamespaceParent is not null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is not BaseNamespaceDeclarationSyntax namespaceParent)
            return nameSpace;

        nameSpace = namespaceParent.Name.ToString();
        
        while (true)
        {
            if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                break;
            nameSpace = $"{namespaceParent.Name}.{nameSpace}";
            namespaceParent = parent;
        }

        return nameSpace;
    }
}