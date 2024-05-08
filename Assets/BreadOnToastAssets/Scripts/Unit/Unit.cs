using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Type of the position where the attack should hit if it, attack hits/crits/misses
/// </summary>
public enum HitPositionType { Normal, Crit, Miss }
/// <summary>
/// Data which holds what area is hit on the unit and its position
/// </summary>
[Serializable]
public struct HitPosition
{
    public Transform HitLocation;
    public HitPositionType Type;
}
public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;

    [SerializeField] private bool _isEnemy;
    [SerializeField] private List<HitPosition> _unitHitPositionList;

    private const int ACTION_POINT_MAX = 1;
    private const int BONUS_ACTION_POINT_MAX = 1;

    private GridPosition _currentGridPosition;
    private MoveAction _moveAction;
    private SpinAction _spinAction;
    private BaseAction[] _baseActionArray;
    private int _actionPoints = 1;
    private int _bonusActionPoints = 1;

    private void Awake()
    {
        _moveAction = GetComponent<MoveAction>();
        _spinAction = GetComponent<SpinAction>();
        _baseActionArray = GetComponents<BaseAction>();
    }
    private void Start()
    {
        _currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_currentGridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    //updates character movement
    void Update()
    {
        //Unit changed Grid Position check
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);//if null causes problems (can prolly move to line 44 at the end of movement)
        if (newGridPosition != _currentGridPosition)
        {
            LevelGrid.Instance.UnitMovedGridPosition(_currentGridPosition, this, newGridPosition);
            _currentGridPosition = newGridPosition;
        }
    }
    private void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    public bool IsEnemy() { return _isEnemy; }
    public void TakeDamage(out HitPositionType hitType)//Empty needs to connect to health system
    {
        Debug.Log($"{name} took damage.");
        hitType = HitPositionType.Normal;
    }
    public int GetActionPoints() { return _actionPoints; }
    public int GetBonusActionPoints() { return _bonusActionPoints; }
    public List<HitPosition> GetUnitHitPositionList() { return _unitHitPositionList; }
    /// <summary>
    /// Allows Actions to get the units GridPosition without calculations
    /// </summary>
    /// <returns></returns>
    public GridPosition GetGridPosition() { return _currentGridPosition; }
    public Vector3 GetWorldPosition() { return transform.position; }
    public BaseAction[] GetBaseActionArray() { return _baseActionArray; }
    public bool TrySpendPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendPointsToTakeAction(baseAction))
        {
            switch (baseAction.GetActionCost())
            {
                case ActionCost.Free:
                    break;
                case ActionCost.Action:
                    _actionPoints--;
                    break;
                case ActionCost.BonusAction:
                    _bonusActionPoints--;
                    break;
                case ActionCost.Both:
                    _actionPoints--;
                    _bonusActionPoints--;
                    break;
                default:
                    Debug.Log($"{transform} - encounterd action cost bug");
                    break;
            }

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
        else { return false; }
    }
    /// <summary>
    /// Allows UnitActionSystem to reach the Units' Action
    /// </summary>
    /// <returns></returns>
    public MoveAction GetMoveAction() { return _moveAction; }
    public SpinAction GetSpinAction() { return _spinAction; }

    private bool CanSpendPointsToTakeAction(BaseAction baseAction)
    {
        switch (baseAction.GetActionCost())
        {
            case ActionCost.Free:
                return true;
            case ActionCost.Action:
                if (_actionPoints > 0) { return true; }
                return false;
            case ActionCost.BonusAction:
                if (_bonusActionPoints > 0) { return true; }
                return false;
            case ActionCost.Both:
                if (_actionPoints > 0 && _bonusActionPoints > 0) { return true; }
                return false;
            default:
                Debug.Log($"{transform} - encounterd action cost bug");
                return false;
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
            !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        {
            _actionPoints = ACTION_POINT_MAX;
            _bonusActionPoints = BONUS_ACTION_POINT_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}