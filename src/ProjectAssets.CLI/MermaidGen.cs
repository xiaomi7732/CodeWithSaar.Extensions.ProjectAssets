using System.Text;
using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public class MermaidGen : IGenerateMermaid
{
    public void Generate(Stream outputStream, Assets assets)
    {
        using(StreamWriter writer = new StreamWriter(outputStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true))
        {
            
        }
    }

    private void Generate(StreamWriter writer, Assets assets)
    {

    }
}