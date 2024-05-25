using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PathNode
{
    private GridPosition _gridPosition;
    /// <summary>
    /// Reference to the previous node where this node is connected to,
    /// linking the final node back to starting node.
    /// </summary>
    private PathNode cameFromPathNode;
    /// <summary>
    /// Walking cost from the Start node
    /// </summary>
    private int gCost;
    /// <summary>
    /// Heuristic to reach End node
    /// </summary>
    private int hCost;
    /// <summary>
    /// G cost + H cost
    /// </summary>
    private int fCost;

    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public int GetGCost() { return gCost; }
    public int GetHCost() { return hCost; }
    public int GetFCost() { return fCost; }

    public override string ToString()
    {
        return _gridPosition.ToString();
    }

}