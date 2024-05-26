using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class LevelGrid : MonoBehaviour
{
    public const float FLOOR_HEIGHT = 2f;

    public static LevelGrid Instance { get; private set; }
    public event EventHandler OnAnyUnitMovedGridPosition;

    [SerializeField] private Transform _gridDebugObjectPrefab;
    [SerializeField] private bool _gridDebugEnabled = false;
    [SerializeField] private int _levelGridWidth = 10;
    [SerializeField] private int _levelGridHeight = 10;
    [SerializeField] private float _levelGridCellSize = 2f;
    [SerializeField] private int _levelGridFloorAmount = 3;

    private List<GridSystem<GridObject>> _gridSystemList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one LevelGrid! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _gridSystemList = new List<GridSystem<GridObject>>();
        for (int floorLevel = 0; floorLevel < _levelGridFloorAmount; floorLevel++)
        {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(_levelGridWidth, _levelGridHeight, _levelGridCellSize, floorLevel, FLOOR_HEIGHT,
                (GridSystem<GridObject> gridSystem, GridPosition gridPosition) => new GridObject(gridSystem, gridPosition));

            if (_gridDebugEnabled)
                gridSystem.CreateDebugObjects(_gridDebugObjectPrefab, transform);

            _gridSystemList.Add(gridSystem);
        }
    }
    private void Start()
    {
        Pathfinding.Instance.SetUp(_levelGridWidth, _levelGridHeight, _levelGridCellSize);
    }

    /// <summary>
    /// Accesses the GridObject in the position and returns the Units
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floorLevel).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }
    /// <summary>
    /// Returns floor level through world position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public int GetFloorLevel(Vector3 worldPosition) { return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT); }

    #region grid validation methods
    /// <summary>
    /// Returns true if grid position is inside the grid systems' area.
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floorLevel).IsValidGridPosition(gridPosition);
    }
    /// <summary>
    /// Returns true if grid position is occupied by other unit
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floorLevel).GetGridObject(gridPosition);
        return gridObject.HasAnyUnits();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floorLevel).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }
    #endregion

    /// <summary>
    /// Concise Move unit method
    /// </summary>
    /// <param name="fromGridPosition"></param>
    /// <param name="unit"></param>
    /// <param name="toGridPosition"></param>
    public void UnitMovedGridPosition(GridPosition fromGridPosition, Unit unit, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    //Need to change to private v ?
    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floorLevel).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floorLevel).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    #region Passing data from local active gridSystem
    /// <summary>
    /// Getting numerical GridPosition from the GridSystem
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        int floorLevel = GetFloorLevel(worldPosition);
        return GetGridSystem(floorLevel).GetGridPosition(worldPosition);
    }
    /// <summary>
    /// Getting gridPosition in worldPosition vector from GridSystem
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floorLevel).GetWorldPosition(gridPosition);
    }
    /// <summary>
    /// Getting this levels' gridSystem width
    /// </summary>
    /// <returns></returns>
    public int GetGridWidth() { return GetGridSystem(0).GetGridWidth(); }
    /// <summary>
    /// Getting this levels' gridSystem height
    /// </summary>
    /// <returns></returns>
    public int GetGridHeight() { return GetGridSystem(0).GetGridHeight(); }
    #endregion

    private GridSystem<GridObject> GetGridSystem(int floorLevel) { return _gridSystemList[floorLevel]; }

}