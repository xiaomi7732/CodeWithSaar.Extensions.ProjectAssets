namespace CodeWithSaar.ProjectAssets.Core;

internal class DefaultFileExistCheck : IFileExistCheck
{
    public bool Check(string? filePath) => File.Exists(filePath);
}