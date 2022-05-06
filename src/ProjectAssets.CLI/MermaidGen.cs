using System.Text;
using CodeWithSaar.ProjectAssets.Models;

namespace CodeWithSaar.ProjectAssets.CLI;

public class MermaidGen : IGenerateMermaid
{
    public async Task GenerateAsync(Stream outputStream, Assets assets, CancellationToken cancellationToken)
    {
        using (StreamWriter writer = new StreamWriter(outputStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true))
        {
            await GenerateAsync(writer, assets, cancellationToken);
        }
    }

    private Task GenerateAsync(StreamWriter writer, Assets assets, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}