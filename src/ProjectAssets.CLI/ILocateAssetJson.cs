namespace CodeWithSaar.ProjectAssets.CLI;
public interface ILocateAssetJson
{
    bool TryLocateFile(string assetFilePathHint, out string locatedFilePath);
}