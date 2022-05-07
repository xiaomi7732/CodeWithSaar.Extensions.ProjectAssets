using System.Text;
using CodeWithSaar.ProjectAssets.Models;
using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

public class MermaidGen : IGenerateMermaid
{
    private readonly ILogger<MermaidGen> _logger;

    public MermaidGen(ILogger<MermaidGen> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task GenerateAsync(Stream outputStream, Assets assets, CancellationToken cancellationToken)
    {
        using (StreamWriter writer = new StreamWriter(outputStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true))
        {
            await GenerateAsync(writer, assets, cancellationToken);
        }
    }

    private async Task GenerateAsync(StreamWriter writer, Assets assets, CancellationToken cancellationToken)
    {
        if (assets.Targets is null)
        {
            _logger.LogError("There is no targets in assets file.");
            return;
        }
        // Write chart type
        writer.WriteLine("stateDiagram-v2");

        // Generating lines of states for assets
        _logger.LogDebug("Parsing assets.");
        foreach (KeyValuePair<string, IDictionary<string, AssetPackageInfo>> framework in assets.Targets)
        {
            string fxName = framework.Key;
            foreach (KeyValuePair<string, AssetPackageInfo> package in framework.Value)
            {
                string packageName = package.Key.Split("/", StringSplitOptions.RemoveEmptyEntries)[0];
                if (IsHeader(packageName, fxName, assets))
                {
                    await writer.WriteLineAsync(GetState("📦" + fxName, package.Key));
                }
                if (package.Value.Dependencies is null)
                {
                    continue;
                }
                foreach (KeyValuePair<string, string> dependency in package.Value.Dependencies)
                {
                    await writer.WriteLineAsync(GetState(package.Key, $"{dependency.Key}/{dependency.Value}"));
                }
            }
        }

        _logger.LogTrace("Finish generating mermaid.");
    }

    private bool IsHeader(string packageName, string framework, Assets assets)
    {
        if (assets.ProjectFileDependencyGroups is null)
        {
            _logger.LogWarning("No project file dependency groups.");
            return false;
        }
        if (assets.ProjectFileDependencyGroups.ContainsKey(framework))
        {
            IEnumerable<string> headerList = assets.ProjectFileDependencyGroups[framework];
            return headerList.Any(line => line.StartsWith(packageName, StringComparison.OrdinalIgnoreCase));
        }
        return false;
    }

    private string GetState(string from, string to)
    {
        return $"{EscapeName(from)} --> {EscapeName(to)}";
    }

    private string EscapeName(string value)
    {
        return value.Replace('-', '_');
    }
}