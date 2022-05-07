namespace CodeWithSaar.ProjectAssets.CLI;

internal class KnownEdgeManager : IManageKnownEdge
{
    private readonly HashSet<string> _knownEdgeHolder;

    public KnownEdgeManager()
    {
        _knownEdgeHolder = new HashSet<string>(StringComparer.Ordinal);
    }

    public bool IsKnownOrAdd(string pointA, string pointB)
    {
        List<string> list = new List<string>(2){
            pointA.ToLower(),
            pointB.ToLower(),
        };
        list.Sort();
        string key = string.Concat(list);
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