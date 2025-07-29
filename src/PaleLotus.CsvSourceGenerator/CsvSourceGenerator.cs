using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static PaleLotus.CsvSourceGenerator.EmbeddedResources;

namespace PaleLotus.CsvSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class CsvSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register attribute and interface sources
        context.RegisterPostInitializationOutput(ctx => 
        {
            ctx.AddSource(
                $"{nameof(CsvSerializableAttribute)}.g.cs",
                SourceText.From(CsvSerializableAttribute, Encoding.UTF8));
            
            ctx.AddSource(
                $"{nameof(ICsvSerializable)}.g.cs",
                SourceText.From(ICsvSerializable, Encoding.UTF8));
        });

        // Only process type declarations (classes, structs, records)
        var typeDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                $"{typeof(CsvSourceGenerator).Assembly.GetName().Name}.{nameof(CsvSerializableAttribute)}",
                predicate: static (node, _) => node is ClassDeclarationSyntax 
                    or StructDeclarationSyntax 
                    or RecordDeclarationSyntax,
                transform: static (ctx, _) => TypeMetadata.FromSyntaxContext(ctx))
            .Where(static m => m is not null);

        // Generate output
        context.RegisterSourceOutput(
            typeDeclarations,
            static (ctx, metadata) => ctx.AddSource(
                $"{metadata!.TypeName}.g.cs",
                SourceText.From(Generate(metadata), Encoding.UTF8)));
    }

    private static string Generate(TypeMetadata metadata)
    {
        var builder = new StringBuilder();
        builder.AppendLine(GetGeneratedHeader());

        var hasNamespace = !string.IsNullOrEmpty(metadata.Namespace);
        if (hasNamespace)
        {
            builder.AppendLine($"namespace {metadata.Namespace}");
            builder.AppendLine("{");
        }

        var indentation = hasNamespace ? "    " : "";
        
        var properties = string.Join(",", metadata.Properties.Select(p => $"{{{p}}}"));
        var headers = string.Join(",", metadata.Properties);

        builder.AppendLine($"{indentation}{metadata.AccessModifier} partial {metadata.TypeKind} {metadata.TypeName} : ICsvSerializable");
        builder.AppendLine($"{indentation}{{");
        builder.AppendLine($"{indentation}    public string ToCsv() => $\"{properties}\";");
        builder.AppendLine($"{indentation}    public string GetCsvHeader() => $\"{headers}\";");
        builder.AppendLine($"{indentation}}}");

        if (hasNamespace)
            builder.AppendLine("}");

        return builder.ToString();
    }
}