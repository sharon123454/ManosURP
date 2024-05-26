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
    private List<Vector3> _positionList;
    private int _currentPositionIndex;

    private void Update()
    {
        if (!_isActive) return;

        Vector3 targetPosition = _positionList[_currentPositionIndex];
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        //Rotation (smooth rotation because starting point isn't cached)
        transform.forward = Vector3.Lerp(transform.forward, moveDir, _unitRotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) > _stoppingDistance)//Stops jittering from never reaching clean position
        {
            //Movement
            transform.position += moveDir * _unitMoveSpeed * Time.deltaTime;
        }
        else
        {
            //If taget position reached increase incrament
            _currentPositionIndex++;

            //Ends action if reached end of positionList
            if (_currentPositionIndex >= _positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    /// <summary>
    /// Sets the units target position.
    /// Unit will move if Update allowes
    /// </summary>
    /// <param name="targetPosition"></param>
    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        //Getting the moving path from Pathfinding
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(_unit.GetGridPosition(), targetPosition, out int pathLength);

        //Resetting positionIndex and positionList
        _currentPositionIndex = 0;
        _positionList = new List<Vector3>();

        //Going over recieved moving path and adding to followed position list
        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            _positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        int pathfindingDistanceMultiplier = 10;//muliplier equals Pathfindings' MOVE_STRAIGHT_COST

        for (int x = -_maxMoveDistance; x <= _maxMoveDistance; x++)
        {
            for (int z = -_maxMoveDistance; z <= _maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; } //if position is outside the gridSystem
                if (unitGridPosition == testGridPosition) { continue; } //if position is the same as units'
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; } //if position is occupied by other unit
                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) { continue; } //if position isn't walkable (blocked by obstacle)
                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) { continue; } //if unit can't reach the position
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > _maxMoveDistance * pathfindingDistanceMultiplier) { continue; } //if path length is too long (behinde walls)

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = _unit.GetAction<RangeAction>().GetTargetAtPosition(gridPosition);
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = targetCountAtGridPosition * 10,
        };
    }

}