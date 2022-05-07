using CodeWithSaar.ProjectAssets.Core;
using CodeWithSaar.ProjectAssets.Models;
using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

public class Engine
{
    private readonly CmdOptions _cmdOptions;
    private readonly ILocateAssetJson _assetJsonLocator;
    private readonly IDeserializeAssets _assetsDeserializer;
    private readonly IGenerateMermaid _mermaidGen;
    private readonly ILogger<Engine> _logger;

    public Engine(
        CmdOptions cmdOptions,
        ILocateAssetJson assetJsonLocator,
        IDeserializeAssets assetsDeserializer,
        IGenerateMermaid mermaidGen,
        ILogger<Engine> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _cmdOptions = cmdOptions ?? throw new ArgumentNullException(nameof(cmdOptions));
        _assetJsonLocator = assetJsonLocator ?? throw new ArgumentNullException(nameof(assetJsonLocator));
        _assetsDeserializer = assetsDeserializer ?? throw new ArgumentNullException(nameof(assetsDeserializer));
        _mermaidGen = mermaidGen ?? throw new ArgumentNullException(nameof(mermaidGen));
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        string assetJsonFilePath = _assetJsonLocator.LocateFile(_cmdOptions.AssetFilePath);
        Assets? assets = await _assetsDeserializer.DeserializeAsync(assetJsonFilePath, cancellationToken);
        if (assets is null)
        {
            throw new InvalidOperationException("Deserialized assets as null. This should not happen.");
        }

        using (Stream outputStream = File.Open(_cmdOptions.OutputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            await _mermaidGen.GenerateAsync(outputStream, assets, cancellationToken);
        }
        _logger.LogInformation("Finish generating mermaid file: {filePath}", Path.GetFullPath(_cmdOptions.OutputFilePath));
    }
}