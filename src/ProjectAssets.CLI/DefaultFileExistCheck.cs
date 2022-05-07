namespace CodeWithSaar.ProjectAssets.CLI;

internal class DefaultFileExistCheck : IFileExistCheck
{
    public bool Check(string? filePath) => File.Exists(filePath);
}