using CodeWithSaar.ProjectAssets.Core;

namespace CodeWithSaar.ProjectAssets.CLI;

public class Engine
{
    private readonly IFileExistCheck _fileExistCheck;

    public Engine(IFileExistCheck fileExistCheck)
    {
        _fileExistCheck = fileExistCheck ?? throw new ArgumentNullException(nameof(fileExistCheck));
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}