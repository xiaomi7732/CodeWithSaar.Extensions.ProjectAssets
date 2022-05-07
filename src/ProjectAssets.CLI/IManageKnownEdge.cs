namespace CodeWithSaar.ProjectAssets.CLI;

public interface IManageKnownEdge
{
    /// <summary>
    /// Checks if the given edge (no direction) is there or not. If not, added it, and returns false; otherwise, returns true.
    /// </summary>
    bool IsKnownOrAdd(string pointA, string pointB);
}