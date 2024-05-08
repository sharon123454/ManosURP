using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform _gridDebugObjectPrefab;
    [SerializeField] private bool _gridDebugEnabled = false;
    [SerializeField] private int _levelGridWidth = 10;
    [SerializeField] private int _levelGridHeight = 10;
    [SerializeField] private float _levelGridCellSize = 2f;

    private GridSystem _gridSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one LevelGrid! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _gridSystem = new GridSystem(_levelGridWidth, _levelGridHeight, _levelGridCellSize);

        if (_gridDebugEnabled)
            _gridSystem.CreateDebugObjects(_gridDebugObjectPrefab, transform);
    }

    /// <summary>
    /// Accesses the GridObject in the position and returns the Units
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    #region grid validation methods
    /// <summary>
    /// Returns true if grid position is inside the grid systems' area.
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.IsValidGridPosition(gridPosition);
    }
    /// <summary>
    /// Returns true if grid position is occupied by other unit
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnits();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
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
    }
    //Need to change to private v ?
    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
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
        return _gridSystem.GetGridPosition(worldPosition);
    }
    /// <summary>
    /// Getting gridPosition in worldPosition vector from GridSystem
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetWorldPosition(gridPosition);
    }
    /// <summary>
    /// Getting this levels' gridSystem width
    /// </summary>
    /// <returns></returns>
    public int GetGridWidth() { return _gridSystem.GetGridWidth(); }
    /// <summary>
    /// Getting this levels' gridSystem height
    /// </summary>
    /// <returns></returns>
    public int GetGridHeight() { return _gridSystem.GetGridHeight(); }
    #endregion

}