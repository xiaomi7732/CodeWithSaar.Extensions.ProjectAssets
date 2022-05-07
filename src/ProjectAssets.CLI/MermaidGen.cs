using System.Text;
using CodeWithSaar.ProjectAssets.Core;
using CodeWithSaar.ProjectAssets.Models;
using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

public class MermaidGen : IGenerateVisual<MermaidGenOptions>
{
    private readonly ILogger<MermaidGen> _logger;
    private readonly IDictionary<string, string> _typeEmojis;
    public MermaidGen(ILogger<MermaidGen> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _typeEmojis = new Dictionary<string, string>()
        {
            ["package"] = "📦",
            ["project"] = "🏗️",
        };
    }

    public async Task GenerateAsync(Stream outputStream, Assets assets, MermaidGenOptions options, CancellationToken cancellationToken)
    {
        using (StreamWriter writer = new StreamWriter(outputStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true))
        {
            await GenerateAsync(writer, assets, options, cancellationToken);
        }
    }

    private async Task GenerateAsync(StreamWriter writer, Assets assets, MermaidGenOptions options, CancellationToken cancellationToken)
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

        if (string.IsNullOrEmpty(options.TargetProject))
        {
            await GenerateFull(writer, assets, cancellationToken);
        }
        else
        {
            await GenerateForTargetProject(writer, assets, options.TargetProject, cancellationToken);
        }

        _logger.LogTrace("Finish generating mermaid.");
    }

    private async Task GenerateForTargetProject(StreamWriter writer, Assets assets, string targetLibrary, CancellationToken cancellationToken)
    {
        foreach (string frameworkName in assets.TryGetTargetFrameworks())
        {
            (string packageSignature, AssetPackageInfo targetLibraryInfo) = assets.TryFindTargetLibrary(frameworkName, targetLibrary);

            if (targetLibraryInfo is null)
            {
                _logger.LogWarning("No target found by framework {frameworkName} and library name: {targetLibrary}", frameworkName, targetLibrary);
                return;
            }

            // TODO: Get all parents.

            // Getting dependencies.
            await GenerateChildProjectsAsync(writer, assets, frameworkName, packageSignature, targetLibraryInfo, new HashSet<string>(), cancellationToken);
        }
    }

    private async Task GenerateChildProjectsAsync(StreamWriter writer, Assets assets, string frameworkName, string currentPackageSignature, AssetPackageInfo current, HashSet<string> knownLeaves, CancellationToken cancellationToken)
    {
        string from = GetLibraryEmoji(assets.GetLibraryType(currentPackageSignature)) + currentPackageSignature;
        if (current.Dependencies is null || !current.Dependencies.Any())
        {
            if (!knownLeaves.Contains(from))
            {
                string to = "[*]";
                await writer.WriteLineAsync(GetState(from, to));
                knownLeaves.Add(from);
            }
            return; // End recursion
        }
        foreach (KeyValuePair<string, string> dependency in current.Dependencies)
        {
            AssetPackageInfo? child = assets.TryFindTargetLibrary(frameworkName, dependency.Key, dependency.Value);
            if (child is not null)
            {
                string childSignature = $"{dependency.Key}/{dependency.Value}";
                string to = GetLibraryEmoji(assets.GetLibraryType(childSignature)) + childSignature;
                await writer.WriteLineAsync(GetState(from, to));
                await GenerateChildProjectsAsync(writer, assets, frameworkName, childSignature, child, knownLeaves, cancellationToken);
            }
        }
    }

    private async Task GenerateFull(StreamWriter writer, Assets assets, CancellationToken cancellationToken)
    {
        foreach (KeyValuePair<string, IDictionary<string, AssetPackageInfo>> framework in assets.Targets!)
        {
            string fxName = framework.Key;
            foreach (KeyValuePair<string, AssetPackageInfo> package in framework.Value)
            {
                string packageName = GetPackageName(package.Key);
                if (IsHeader(packageName, fxName, assets))
                {
                    await writer.WriteLineAsync(GetState("🪟" + fxName, GetLibraryEmoji(assets.GetLibraryType(package.Key)) + package.Key));
                }
                if (package.Value.Dependencies is null)
                {
                    continue;
                }
                foreach (KeyValuePair<string, string> dependency in package.Value.Dependencies)
                {
                    string from = GetLibraryEmoji(assets.GetLibraryType(package.Key)) + package.Key;
                    string toLibrarySignature = $"{dependency.Key}/{dependency.Value}";
                    string to = GetLibraryEmoji(assets.GetLibraryType(toLibrarySignature)) + toLibrarySignature;
                    await writer.WriteLineAsync(GetState(from, to));
                }
            }
        }
    }

    private string GetPackageName(string packageSignature)
    {
        return packageSignature.Split("/", StringSplitOptions.RemoveEmptyEntries)[0];
    }

    private string GetLibraryEmoji(string type)
    {
        if (_typeEmojis.ContainsKey(type))
        {
            return _typeEmojis[type];
        }
        return string.Empty;
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