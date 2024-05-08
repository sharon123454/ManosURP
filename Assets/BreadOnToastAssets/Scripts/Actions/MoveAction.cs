using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [Header("Base Unit Parameters")]
    [SerializeField] private float _unitRotationSpeed = 7.5f;
    [SerializeField] private float _unitMoveSpeed = 4f;
    [SerializeField] private int _maxMoveDistance = 6;

    private float _stoppingDistance = 0.1f;
    private Vector3 _targetPosition;

    protected override void Awake()
    {
        base.Awake();
        _targetPosition = transform.position;
    }
    private void Update()
    {
        if (!_isActive) return;

        Vector3 moveDir = (_targetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, _targetPosition) > _stoppingDistance)//Stops jittering from never reaching clean position
        {
            //Movement
            transform.position += moveDir * _unitMoveSpeed * Time.deltaTime;
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        //Rotation (smooth rotation because starting point isn't cached)
        transform.forward = Vector3.Lerp(transform.forward, moveDir, _unitRotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Sets the units target position.
    /// Unit will move if Update allowes
    /// </summary>
    /// <param name="targetPosition"></param>
    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetPosition);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();

        for (int x = -_maxMoveDistance; x <= _maxMoveDistance; x++)
        {
            for (int z = -_maxMoveDistance; z <= _maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; } //if position is outside the gridSystem
                if (unitGridPosition == testGridPosition) { continue; } //if position is the same as units'
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; } //if position is occupied by other unit

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

}