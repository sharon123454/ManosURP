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
        _targetUnit.TakeDamage(out hitType);

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
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();

        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; } // if position is outside the gridSystem

                //Collective distance from position. Makes valid grid circular instead of square
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > _maxShootDistance) { continue; }

                //if (unitGridPosition == testGridPosition) { continue; } // if position is the same as units'
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; } // if position empty, NO UNIT

                Unit unitAtGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (_unit.IsEnemy() == unitAtGridPosition.IsEnemy()) { continue; } // if units are of the same side

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

}