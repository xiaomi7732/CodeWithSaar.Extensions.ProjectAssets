using System.Text.Json;
using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public class AssetsDeserializer : IDeserializeAssets
{
    public async Task<Assets?> DeserializeAsync(string assetFilePath, CancellationToken cancellationToken)
    {
        JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        using (Stream inputStream = File.OpenRead(assetFilePath))
        {
            return await JsonSerializer.DeserializeAsync<Assets>(inputStream, options, cancellationToken);
        }
    }
}
