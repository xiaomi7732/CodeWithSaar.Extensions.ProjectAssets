using CommandLine;

namespace CodeWithSaar.ProjectAssets.CLI;

public class CmdOptions
{
    [Option('i', "input", Required = true, HelpText = "Path to project.assets.json. Support full path for the path to the obj directory. For example: ./obj/")]
    public string AssetFilePath { get; set; } = string.Empty;

    [Option('o', "output", Required = false, HelpText = "Output file path", Default = "output.mmd")]
    public string OutputFilePath { get; set; } = "output.mmd";

    [Option('t', "target-package", Required = false, HelpText = "Only output target package", Default = null)]
    public string? TargetPackage { get; set; }

    [Option('d', "search-direction", Required = false, HelpText = "Search direction. Supports value: Up(1), Down(2), or Both(3).", Default = SearchDirection.Up | SearchDirection.Down)]
    public SearchDirection SearchDirection { get; set; }
}