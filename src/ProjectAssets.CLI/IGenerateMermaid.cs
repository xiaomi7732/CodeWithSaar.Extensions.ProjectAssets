using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public interface IGenerateMermaid
{
    Task GenerateAsync(Stream outputStream, Assets assets, CancellationToken cancellationToken);
}