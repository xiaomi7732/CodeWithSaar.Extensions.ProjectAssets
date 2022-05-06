using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public interface IDeserializeAssets
{
    Task<Assets?> DeserializeAsync(string assetFilePath, CancellationToken cancellationToken);
}