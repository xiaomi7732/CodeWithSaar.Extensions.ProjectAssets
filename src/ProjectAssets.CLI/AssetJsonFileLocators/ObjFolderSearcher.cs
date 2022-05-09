using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

internal class ObjFolderSearcher : ILocateAssetJson
{
    const string FileName = "project.assets.json";
    private readonly IDirectoryExistCheck _directoryExist;
    private readonly IFileExistCheck _fileExist;
    private readonly ILogger _logger;

    public ObjFolderSearcher(IDirectoryExistCheck directoryExist, IFileExistCheck fileExist, ILogger<ObjFolderSearcher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _directoryExist = directoryExist ?? throw new ArgumentNullException(nameof(directoryExist));
        _fileExist = fileExist ?? throw new ArgumentNullException(nameof(fileExist));
    }

    public bool TryLocateFile(string assetFilePathHint, out string locatedFilePath)
    {
        _logger.LogDebug("Searching for asset file. Treat as obj folder: {fileHint}", assetFilePathHint);

        locatedFilePath = string.Empty;

        if (string.IsNullOrEmpty(assetFilePathHint))
        {
            return false;
        }

        if (!_directoryExist.Check(assetFilePathHint))
        {
            return false;
        }

        string targetFileLocation = Path.Combine(assetFilePathHint, FileName);
        if (_fileExist.Check(targetFileLocation))
        {
            _logger.LogDebug("Found target file: {targetFile}", targetFileLocation);
            locatedFilePath = targetFileLocation;
            return true;
        }

        return false;
    }
}