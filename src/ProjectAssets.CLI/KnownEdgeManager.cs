namespace CodeWithSaar.ProjectAssets.CLI;

internal class KnownEdgeManager : IManageKnownEdge
{
    private readonly HashSet<string> _knownEdgeHolder;

    public KnownEdgeManager()
    {
        _knownEdgeHolder = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public bool IsKnownOrAdd(string pointA, string pointB)
    {
        string key = pointA + "##4#3#9#" + pointB;
        if (_knownEdgeHolder.Contains(key))
        {
            // It is known
            return true;
        }
        // It is not known
        _knownEdgeHolder.Add(key);
        return false;
    }
}