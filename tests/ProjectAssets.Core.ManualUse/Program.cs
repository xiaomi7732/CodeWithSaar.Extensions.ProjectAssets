using System.Text.Json;
using CodeWithSaar.ProjectAssets.Models;

JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

Dictionary<string, AssetPackageInfo> info = new Dictionary<string, AssetPackageInfo>()
{
    ["Microsoft.NetCore.Platforms/1.0.0"] = new AssetPackageInfo()
    {
        Type = "package"
    }
};

Assets model = new Assets()
{
    Version = 3,
    Targets = new Dictionary<string, IDictionary<string, AssetPackageInfo>>()
    {
        [".NETStandard,Version=v2.0"] = info
    }
};

string result = JsonSerializer.Serialize(model, options);

Console.WriteLine(result);