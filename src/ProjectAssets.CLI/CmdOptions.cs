using CommandLine;

namespace CodeWithSaar.ProjectAssets.CLI;

public class CmdOptions
{
    [Option('f', "filepath", Required = true, HelpText = "Path to project.assets.json.")]
    public string AssetFilePath { get; set; } = string.Empty;

}