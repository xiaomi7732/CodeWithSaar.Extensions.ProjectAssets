using CommandLine;

namespace CodeWithSaar.ProjectAssets.CLI;

public class CmdOptions
{
    [Option('i', "input", Required = true, HelpText = "Path to project.assets.json.")]
    public string AssetFilePath { get; set; } = string.Empty;

    [Option('o', "output", Required = false, HelpText = "Output file path", Default = "output.mmd")]
    public string OutputFilePath { get; set; } = "output.mmd";
}