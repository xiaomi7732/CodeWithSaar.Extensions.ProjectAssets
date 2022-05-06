using System.Text.Json;
using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public class AssetsDeserializer : IDeserializeAssets
{
    public Assets? Deserialize(string assetFilePath)
    {
        JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        return JsonSerializer.Deserialize<Assets>(assetFilePath);
    }
}
