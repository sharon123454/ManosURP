using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Transform _pathfindingDebugObjectPrefab;
    [SerializeField] private bool _pathDebugEnabled = false;

    private int _width;
    private int _height;
    private float _cellSize;
    GridSystem<PathNode> _gridSystem;

    private void Awake()
    {
        _gridSystem = new GridSystem<PathNode>(10, 10, 2f,
            (GridSystem<PathNode> gridSystem, GridPosition gridPosition) => new PathNode(gridPosition));
        
        if (_pathDebugEnabled)
            _gridSystem.CreateDebugObjects(_pathfindingDebugObjectPrefab, transform);
    }

}