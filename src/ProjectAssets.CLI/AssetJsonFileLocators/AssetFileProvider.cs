namespace CodeWithSaar.ProjectAssets.CLI;

public class AssetFileProvider : IAssetFileProvider
{
    private readonly IEnumerable<ILocateAssetJson> _locators;

    public AssetFileProvider(IEnumerable<ILocateAssetJson> locators)
    {
        _locators = locators ?? throw new ArgumentNullException(nameof(locators));
    }

    public bool TryLocateFile(string assetFilePathHint, out string locatedFilePath)
    {
        foreach (ILocateAssetJson locator in _locators)
        {
            string hit;
            if (locator.TryLocateFile(assetFilePathHint, out hit))
            {
                locatedFilePath = hit;
                return true;
            }
        }
        locatedFilePath = string.Empty;
        return false;
    }
}