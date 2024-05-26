public class PathNode
{
    private GridPosition _gridPosition;
    /// <summary>
    /// Reference to the previous node where this node is connected to,
    /// linking the final node back to starting node.
    /// </summary>
    private PathNode _cameFromPathNode;
    /// <summary>
    /// Walking cost from the Start node
    /// </summary>
    private int _gCost;
    /// <summary>
    /// Heuristic to reach End node
    /// </summary>
    private int _hCost;
    /// <summary>
    /// G cost + H cost
    /// </summary>
    private int _fCost;
    private bool _isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public bool IsWalkable() { return _isWalkable; }
    public void SetIsWalkable(bool isWalkable) { _isWalkable = isWalkable; }
    public int GetGCost() { return _gCost; }
    public void SetGCost(int gCost) { _gCost = gCost; }
    public int GetHCost() { return _hCost; }
    public void SetHCost(int hCost) { _hCost = hCost; }
    public int GetFCost() { return _fCost; }
    public void CalculateFCost() { _fCost = _gCost + _hCost; }
    public void ResetCameFromPathNode() { _cameFromPathNode = null; }
    public PathNode GetCameFromPathNode() { return _cameFromPathNode; }
    public void SetCameFromPathNode(PathNode pathNode) { _cameFromPathNode = pathNode; }
    public GridPosition GetGridPosition() { return _gridPosition; }

    public override string ToString()
    {
        return _gridPosition.ToString();
    }
}