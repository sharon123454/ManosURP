using UnityEngine;
using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z) { this.x = x; this.z = z; }
    public override string ToString() { return $"X: {x}; Z: {z}"; }
    public override int GetHashCode() { return HashCode.Combine(x, z); }
    public override bool Equals(object obj) { return obj is GridPosition position && x == position.x && z == position.z; }
    public bool Equals(GridPosition other) { return this == other; }
    public static bool operator !=(GridPosition a, GridPosition b) { return !(a == b); }
    public static bool operator ==(GridPosition a, GridPosition b) { return a.x == b.x && a.z == b.z; }
    public static GridPosition operator +(GridPosition a, GridPosition b) { return new GridPosition(a.x + b.x, a.z + b.z); }
    public static GridPosition operator -(GridPosition a, GridPosition b) { return new GridPosition(a.x - b.x, a.z - b.z); }
}

public class GridSystem
{
    private int _width;
    private int _height;
    private float _cellSize;
    private GridObject[,] _gridObjectArray;

    public GridSystem(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _gridObjectArray = new GridObject[width, height];

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                _gridObjectArray[x, z] = new GridObject(this, gridPosition);
            }
        }
    }

    public int GetGridWidth() { return _width; }
    public int GetGridHeight() { return _height; }
    /// <summary>
    /// Returns the Object which sits on given Grid
    /// Holds actual grid data
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return _gridObjectArray[gridPosition.x, gridPosition.z];
    }

    /// <summary>
    /// Returns true if grid position is inside the grid systems' area
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
                gridPosition.z >= 0 &&
                gridPosition.x < _width &&
                gridPosition.z < _height;
    }

    /// <summary>
    /// Returns mathematical World position by grid
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
    }
    //If you don't need mathematical, access the GridObjects' GridPosition
    /// <summary>
    /// Returns mathematical Grid position by Vector3
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize),
            Mathf.RoundToInt(worldPosition.z / _cellSize));
    }


    /// <summary>
    /// Creates debug Grid Objects (TESTING ONLY)
    /// </summary>
    /// <param name="debugPrefab"></param>
    public void CreateDebugObjects(Transform debugPrefab, Transform parentObject)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, parentObject);
                GridDebugObject debugObject = debugTransform.GetComponent<GridDebugObject>();
                debugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

}