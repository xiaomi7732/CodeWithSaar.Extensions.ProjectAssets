using System.Text;
using CodeWithSaar.ProjectAssets.Core;
using CodeWithSaar.ProjectAssets.Models;
using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

public class MermaidGen : IGenerateVisual<MermaidGenOptions>
{
    private readonly IManageKnownEdge _edgeManager;
    private readonly ILogger<MermaidGen> _logger;
    private readonly IDictionary<string, string> _typeEmojis;
    public MermaidGen(
        IManageKnownEdge edgeManager,
        ILogger<MermaidGen> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _edgeManager = edgeManager ?? throw new ArgumentNullException(nameof(edgeManager));
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
            await GenerateForTargetProject(writer, assets, options.TargetProject, options.SearchDirection, cancellationToken);
        }

        _logger.LogTrace("Finish generating mermaid.");
    }

    private async Task GenerateForTargetProject(StreamWriter writer, Assets assets, string targetLibrary, SearchDirection searchDirection, CancellationToken cancellationToken)
    {
        foreach (string frameworkName in assets.TryGetTargetFrameworks())
        {
            (string packageSignature, AssetPackageInfo targetLibraryInfo) = assets.TryFindTargetLibrary(frameworkName, targetLibrary);

            if (targetLibraryInfo is null)
            {
                _logger.LogWarning("No target found by framework {frameworkName} and library name: {targetLibrary}", frameworkName, targetLibrary);
                return;
            }

            // Get all parents.
            if (searchDirection.HasFlag(SearchDirection.Up))
            {
                await GenerateParentProjectsAsync(writer, assets, frameworkName, packageSignature, targetLibraryInfo, cancellationToken);
            }

            // Getting dependencies.
            if (searchDirection.HasFlag(SearchDirection.Down))
            {
                await GenerateChildProjectsAsync(writer, assets, frameworkName, packageSignature, targetLibraryInfo, cancellationToken);
            }
        }
    }

    private async Task GenerateParentProjectsAsync(StreamWriter writer, Assets assets, string frameworkName, string currentPackageSignature, AssetPackageInfo current, CancellationToken cancellationToken)
    {
        if (assets.Targets is null)
        {
            throw new InvalidOperationException("Give assets has no target.");
        }

        string to = currentPackageSignature;

        IEnumerable<KeyValuePair<string, AssetPackageInfo>> parents = assets.Targets[frameworkName].Where(item =>
            item.Value.Dependencies is not null &&
            item.Value.Dependencies.Any(p => string.Equals($"{p.Key}/{p.Value}", currentPackageSignature, StringComparison.OrdinalIgnoreCase)));
        if (parents is not null && parents.Any())
        {
            foreach (KeyValuePair<string, AssetPackageInfo> parent in parents)
            {
                await GenerateParentProjectsAsync(writer, assets, frameworkName, parent.Key, parent.Value, cancellationToken);
                await RenderLineAsync(parent.Key, to, assets, writer, cancellationToken);
            }
        }
        else
        {
            if (assets.IsHeaderProject(GetPackageName(to), frameworkName))
            {
                await RenderLineAsync(frameworkName, to, assets, writer, cancellationToken);
            }
        }
    }

    private async Task GenerateChildProjectsAsync(StreamWriter writer, Assets assets, string frameworkName, string currentPackageSignature, AssetPackageInfo current, CancellationToken cancellationToken)
    {
        string from = currentPackageSignature;
        if (current.Dependencies is null || !current.Dependencies.Any())
        {
            await RenderLineAsync(from, string.Empty, assets, writer, cancellationToken);
            return; // End recursion
        }
        foreach (KeyValuePair<string, string> dependency in current.Dependencies)
        {
            AssetPackageInfo? child = assets.TryFindTargetLibrary(frameworkName, dependency.Key, dependency.Value);
            if (child is not null)
            {
                string childSignature = $"{dependency.Key}/{dependency.Value}";
                await RenderLineAsync(from, childSignature, assets, writer, cancellationToken);
                await GenerateChildProjectsAsync(writer, assets, frameworkName, childSignature, child, cancellationToken);
            }
        }
    }

    private async Task RenderLineAsync(string fromPackageSignature, string toPackageSignature, Assets assets, StreamWriter writer, CancellationToken cancellationToken)
    {
        fromPackageSignature = NormalizePackageSignatureForRendering(fromPackageSignature, assets);
        toPackageSignature = NormalizePackageSignatureForRendering(toPackageSignature, assets);

        if (!_edgeManager.IsKnownOrAdd(fromPackageSignature, toPackageSignature))
        {
            await writer.WriteLineAsync(GetState(fromPackageSignature, toPackageSignature));
        }
    }

    private string NormalizePackageSignatureForRendering(string packageSignature, Assets assets)
    {
        if (string.IsNullOrEmpty(packageSignature))
        {
            return "[*]";
        }
        else
        {
            return GetLibraryEmoji(assets.GetLibraryType(packageSignature)) + packageSignature;
        }
    }

    private async Task GenerateFull(StreamWriter writer, Assets assets, CancellationToken cancellationToken)
    {
        foreach (string frameworkName in assets.TryGetTargetFrameworks())
        {
            if (assets.ProjectFileDependencyGroups is not null && assets.ProjectFileDependencyGroups.ContainsKey(frameworkName))
            {
                foreach (string projectNameWithVersionInfo in assets.ProjectFileDependencyGroups[frameworkName])
                {
                    string projectName = projectNameWithVersionInfo.Split(" ")[0];
                    await GenerateForTargetProject(writer, assets, projectName, SearchDirection.Up | SearchDirection.Down, cancellationToken);
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



    private string GetState(string from, string to)
    {
        return $"{EscapeName(from)} --> {EscapeName(to)}";
    }

    private string EscapeName(string value)
    {
        return value.Replace('-', '_');
    }
}