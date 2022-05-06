using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public interface IDeserializeAssets
{
    Assets? Deserialize(string assetFilePath);
}