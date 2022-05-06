using CodeWithSaar.ProjectAssets.Core;
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

        public string LocateFile(string assetFilePathHint)
        {
            if (assetFilePathHint is null)
            {
                throw new ArgumentNullException(nameof(assetFilePathHint));
            }

            if (_fileExist.Check(assetFilePathHint))
            {
                _logger.LogDebug("Given asset file exists at {filePath}", assetFilePathHint);
                return assetFilePathHint;
            }

            throw new FileNotFoundException("Asset File not found. File path hint: {filePath}", assetFilePathHint);
        }
    }
}