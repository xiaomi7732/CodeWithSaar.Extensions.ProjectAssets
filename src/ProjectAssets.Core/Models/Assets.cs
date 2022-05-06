using System.Collections.Generic;

namespace CodeWithSaar.ProjectAssets.Models;

public record Assets
{
    public double Version { get; init; }

    public IDictionary<string, IDictionary<string, AssetPackageInfo>>? Targets { get; init; }

    public IDictionary<string, AssetLibraryInfo>? Libraries { get; init; }

    public IDictionary<string, IEnumerable<string>>? ProjectFileDependencyGroups { get; init; }
}