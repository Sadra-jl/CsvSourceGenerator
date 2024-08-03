using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static PaleLotus.CsvSourceGenerator.EmbeddedSources;

namespace PaleLotus.CsvSourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class CsvSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(incContext =>
        {
            incContext.AddSource(
                $"{nameof(CsvSerializableAttribute)}.g.cs",
                SourceText.From(CsvSerializableAttribute, Encoding.UTF8));
            
            incContext.AddSource($"{nameof(ICsvSerializableInterface)}.g.cs",
                ICsvSerializableInterface);
        });

        var typesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName($"FreeDoomBridge.SourceGenerators.{nameof(CsvSerializableAttribute)}",
                predicate: static (_, _) => true,
                transform: static (attContext, _) =>
                    GetTypeToGenerate(attContext.SemanticModel, attContext.TargetNode))
            .Where(static source => source is not null);
        
        context.RegisterSourceOutput(typesToGenerate,
            static (spc, source) => Execute(source, spc));
    }
    
    private static void Execute(TypeToGenerate? typeToGenerate, SourceProductionContext context)
    {
        if (typeToGenerate is null) return;
        
        context.AddSource($"{typeToGenerate.TypeName}.g.cs",
            SourceText.From(typeToGenerate.Generate(), Encoding.UTF8));
    }

    private static TypeToGenerate? GetTypeToGenerate(SemanticModel semanticModel, SyntaxNode typeDeclarationSyntax)
    {
        if (semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not INamedTypeSymbol typeSymbol)
            return null;

        var name = typeSymbol.Name;

        var typeMembers = typeSymbol.GetMembers();
        var members = new List<string>(typeMembers.Length);

        foreach (var member in typeMembers)
            if (member is IPropertySymbol)
                members.Add(member.Name);
        
        return new TypeToGenerate(name, members,GetAccessModifier(typeSymbol),
            GetTypeKind(typeSymbol),GetNamespace(typeSymbol.ContainingNamespace));    
    }
    
      private static string GetTypeKind(INamedTypeSymbol symbol)
    {
        if (symbol.IsRecord)
            return "record";
        if (symbol.TypeKind == TypeKind.Class)
            return "class";
        return symbol.TypeKind == TypeKind.Struct ? "struct" : "unknown";
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
            _ => "unknown" // Handle other cases if needed
        };

    private static string GetNamespace(INamespaceSymbol symbol) =>
        symbol.ContainingNamespace is null
            ? symbol.Name
            : new StringBuilder()
                .Append(GetNamespace(symbol.ContainingNamespace))
                .Append(".")
                .Append(symbol.Name)
                .ToString()
                .Trim('.');
}