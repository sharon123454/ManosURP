using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridObject
{
    private List<Unit> _gridUnitList;
    private GridSystem _gridSystem;
    private GridPosition _gridPosition;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _gridUnitList = new List<Unit>();
    }

    public Unit GetUnit()
    {
        if (HasAnyUnits())
            return _gridUnitList[0];
        return null;
    }
    public List<Unit> GetUnitList() { return _gridUnitList; }
    public bool HasAnyUnits() { return _gridUnitList.Count > 0; }

    public void AddUnit(Unit unit) { _gridUnitList.Add(unit); }
    public void RemoveUnit(Unit unit) { _gridUnitList.Remove(unit); }

    public override string ToString()
    {
        string unitString = string.Empty;
        foreach (Unit unit in _gridUnitList)
            unitString += unit.gameObject.name + "\n";

        return _gridPosition.ToString() + "\n" + unitString;
    }

}