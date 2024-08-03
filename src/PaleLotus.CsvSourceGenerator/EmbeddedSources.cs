using System.Reflection;
using System.Text;

namespace PaleLotus.CsvSourceGenerator;

internal static class EmbeddedSources
{
    private static readonly Assembly ThisAssembly = typeof(EmbeddedSources).Assembly;
    internal static readonly string ICsvSerializableInterface = LoadEmbedded("ICsvSerializable.txt");
    internal static readonly string CsvSerializableAttribute = LoadEmbedded("CsvSerializableAttribute.txt");

    private static string LoadEmbedded(string templateName)
        => LoadEmbeddedResource($"FreeDoomBridge.SourceGenerators.Templates.{templateName}");

    private static string LoadEmbeddedResource(string resourceName)
    {
        var resourceStream = ThisAssembly.GetManifestResourceStream(resourceName);
        if (resourceStream is null)
        {
            var existingResources = ThisAssembly.GetManifestResourceNames();
            throw new ArgumentException(
                $"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", existingResources)}");
        }

        using var reader = new StreamReader(resourceStream, Encoding.UTF8);

        return reader.ReadToEnd();
    }

}