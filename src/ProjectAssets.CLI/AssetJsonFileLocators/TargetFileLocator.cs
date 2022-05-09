using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI
{


    internal class TargetFileLocator : ILocateAssetJson
    {
        private readonly ILogger<TargetFileLocator> _logger;
        private readonly IFileExistCheck _fileExist;

        public TargetFileLocator(
            ILogger<TargetFileLocator> logger,
            IFileExistCheck fileExist)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileExist = fileExist ?? throw new ArgumentNullException(nameof(fileExist));
        }

        public bool TryLocateFile(string assetFilePathHint, out string locatedFilePath)
        {
            _logger.LogDebug("Searching for file: {fileHint}", assetFilePathHint);
            
            locatedFilePath = string.Empty;
            if(string.IsNullOrEmpty(assetFilePathHint))
            {
                return false;
            }

            if (_fileExist.Check(assetFilePathHint))
            {
                _logger.LogDebug("Given asset file exists at {filePath}", assetFilePathHint);
                locatedFilePath = assetFilePathHint;
                return true;
            }
            return false;
        }
    }
}