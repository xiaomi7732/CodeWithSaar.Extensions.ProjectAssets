using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public interface IGenerateMermaid
{
    void Generate(Stream outputStream, Assets assets);
}