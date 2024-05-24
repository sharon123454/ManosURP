using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Unit> _unitList;
    private List<Unit> _enemyUnitList;
    private List<Unit> _friendlyUnitList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one UnitManager! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _unitList = new List<Unit>();
        _enemyUnitList = new List<Unit>();
        _friendlyUnitList = new List<Unit>();
    }
    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }
    private void OnDisable()
    {
        Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }

    public List<Unit> GetUnitList() { return _unitList; }
    public List<Unit> GetEnemyUnitList() { return _enemyUnitList; }
    public List<Unit> GetFriendlyUnitList() { return _friendlyUnitList; }

    private void Unit_OnAnyUnitDead(object sender, EventArgs empty)
    {
        Unit unit = sender as Unit;

        _unitList.Remove(unit);
        if (unit.IsEnemy())
        {
            _enemyUnitList.Remove(unit);
        }
        else
        {
            _friendlyUnitList.Remove(unit);
        }
    }
    private void Unit_OnAnyUnitSpawned(object sender, EventArgs empty)
    {
        Unit unit = sender as Unit;

        _unitList.Add(unit);
        if (unit.IsEnemy())
        {
            _enemyUnitList.Add(unit);
        }
        else
        {
            _friendlyUnitList.Add(unit);
        }
    }

}