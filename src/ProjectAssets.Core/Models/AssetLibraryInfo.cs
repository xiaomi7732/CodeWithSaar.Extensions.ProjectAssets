using System.Collections.Generic;
using System.Linq;

namespace CodeWithSaar.ProjectAssets.Models;

public class AssetLibraryInfo
{
    public string? Sha512 { get; set; }
    public string? Type { get; set; }
    public string? Path { get; set; }
    public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();
}