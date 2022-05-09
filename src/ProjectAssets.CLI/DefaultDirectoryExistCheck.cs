namespace CodeWithSaar.ProjectAssets.CLI;

public class DefaultDirectoryExistCheck : IDirectoryExistCheck
{
    public bool Check(string? directoryPath) => Directory.Exists(directoryPath);
}