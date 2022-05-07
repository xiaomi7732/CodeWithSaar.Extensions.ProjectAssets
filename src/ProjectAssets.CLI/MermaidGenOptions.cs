namespace CodeWithSaar.ProjectAssets.CLI;

public class MermaidGenOptions
{
    public string? TargetProject { get; init; }
    public SearchDirection SearchDirection { get; init; } = SearchDirection.Up | SearchDirection.Down;
}