using CodeWithSaar.ProjectAssets.Models;
using Microsoft.Extensions.Logging;

namespace CodeWithSaar.ProjectAssets.CLI;

public class Engine
{
    private readonly CmdOptions _cmdOptions;
    private readonly ILocateAssetJson _assetFileProvider;
    private readonly IDeserializeAssets _assetsDeserializer;
    private readonly IGenerateVisual<MermaidGenOptions> _mermaidGen;
    private readonly ILogger<Engine> _logger;

    public Engine(
        CmdOptions cmdOptions,
        IAssetFileProvider assetFileProvider,
        IDeserializeAssets assetsDeserializer,
        IGenerateVisual<MermaidGenOptions> mermaidGen,
        ILogger<Engine> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _cmdOptions = cmdOptions ?? throw new ArgumentNullException(nameof(cmdOptions));
        _assetFileProvider = assetFileProvider ?? throw new ArgumentNullException(nameof(assetFileProvider));
        _assetsDeserializer = assetsDeserializer ?? throw new ArgumentNullException(nameof(assetsDeserializer));
        _mermaidGen = mermaidGen ?? throw new ArgumentNullException(nameof(mermaidGen));
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if(!_assetFileProvider.TryLocateFile(_cmdOptions.AssetFilePath, out string assetJsonFilePath))
        {
            throw new InvalidOperationException($"Can't locate asset file. Location hint: {_cmdOptions.AssetFilePath}");
        }

        Assets? assets = await _assetsDeserializer.DeserializeAsync(assetJsonFilePath, cancellationToken);
        if (assets is null)
        {
            throw new InvalidOperationException("Deserialized assets as null. This should not happen.");
        }

        MermaidGenOptions options = new MermaidGenOptions()
        {
            TargetProject = _cmdOptions.TargetPackage,
            SearchDirection = _cmdOptions.SearchDirection,
        };

        using (Stream outputStream = File.Open(_cmdOptions.OutputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            await _mermaidGen.GenerateAsync(outputStream, assets, options, cancellationToken);
        }
        _logger.LogInformation("Finish generating mermaid file: {filePath}", Path.GetFullPath(_cmdOptions.OutputFilePath));
    }
}