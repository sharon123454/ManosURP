using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Holds the: ActionName, ShootingUnit, TargetUnit and the TargetHitPositionType
/// </summary>
public class OnShootEventArgs : EventArgs
{
    public string ActionName;
    public Unit ShootingUnit;
    public Unit TargetUnit;
    public HitPositionType TargetHitPositionType;
}
public class RangeAction : BaseAction
{
    public event EventHandler<OnShootEventArgs> OnShoot;

    [SerializeField] private int _maxShootDistance = 6;
    [SerializeField] private LayerMask _obstacleLayerMask;

    private Unit _targetUnit;
    private State _currentState;
    private bool _canShootBullet;

    private float _stateTimer;
    private float _aimingStateTime = 1f;
    private float _shootingStateTime = 0.1f;
    private float _cooloffStateTime = 0.5f;

    private enum State { Aiming, Shooting, Cooloff }

    private void Update()
    {
        if (!_isActive) { return; }

        _stateTimer -= Time.deltaTime;

        switch (_currentState)
        {
            case State.Aiming:
                Vector3 aimDirection = (_targetUnit.GetWorldPosition() - _unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, 9.5f * Time.deltaTime);
                break;
            case State.Shooting:
                if (_canShootBullet)
                {
                    Shoot();
                }
                break;
            case State.Cooloff:
                break;
        }

        if (_stateTimer <= 0) { NextState(); }

    }

    public int GetTargetAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
    public int GetActionRange() { return _maxShootDistance; }

    private void NextState()
    {
        switch (_currentState)
        {
            case State.Aiming:
                _currentState = State.Shooting;
                _stateTimer = _shootingStateTime;
                break;
            case State.Shooting:
                _currentState = State.Cooloff;
                _stateTimer = _cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }
    private void Shoot()
    {
        HitPositionType hitType;
        _targetUnit.TakeDamage(out hitType, _actionDamage);

        OnShoot?.Invoke(this, new OnShootEventArgs()
        {
            ActionName = GetActionName(),
            TargetUnit = _targetUnit,
            ShootingUnit = _unit,
            TargetHitPositionType = hitType
        });

        _canShootBullet = false;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        _currentState = State.Aiming;
        _stateTimer = _aimingStateTime;
        _canShootBullet = true;
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = _unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }
    /// <summary>
    /// Returns valid shootable targets around given GridPosition
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        float unitShoulderHeight = 1.7f;

        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; } // if position is outside the gridSystem

                //Collective distance from position. Makes valid grid circular instead of square
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > _maxShootDistance) { continue; }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; } // if position empty, NO UNIT

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (_unit.IsEnemy() == targetUnit.IsEnemy()) { continue; } // if units are of the same side

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDirection,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    _obstacleLayerMask)) { continue; } // Blocked by an Obstacle

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            //Normalized HP is 0 to 1 and the lower it is the better the action so 1-HpNormalized. multy by 100 so once rounded it's accepted as int
            ActionValue = 200 + Mathf.RoundToInt(1 - targetUnit.GetHealthNormalized() * 100),//Example- full HP = 100, half HP = 150 (in total action value)
        };
    }

}