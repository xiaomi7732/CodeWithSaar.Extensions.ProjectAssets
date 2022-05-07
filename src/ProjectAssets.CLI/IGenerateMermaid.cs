using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public interface IGenerateVisual<TOptions>
{
    Task GenerateAsync(Stream outputStream, Assets assets, TOptions options, CancellationToken cancellationToken);
}