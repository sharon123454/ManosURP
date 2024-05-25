using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public enum ActionCost { Free, Action, BonusAction, Both }
public class EnemyAIAction
{
    public GridPosition GridPosition;
    public int ActionValue;
}
public abstract class BaseAction : MonoBehaviour
{
    [SerializeField] protected string _actionName;
    [SerializeField] protected Image _actionImage;
    [SerializeField] protected int _actionDamage;
    [SerializeField] protected ActionCost _actionCost = ActionCost.Action;

    protected Unit _unit;
    protected bool _isActive;

    private Action _onActionComplete;

    protected virtual void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        //Creates a list of actions and thier values
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();

        //Goes through valid positions and 
        foreach (GridPosition gridPosition in validGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            //Actions sorted by action value
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.ActionValue - a.ActionValue);
            return enemyAIActionList[0];
        }
        else
        {
            //No possible enemy AI Actions
            return null;
        }
    }

    public virtual ActionCost GetActionCost() { return _actionCost; }
    public virtual string GetActionName() { return _actionName; }
    public virtual Image GetActionImage() { return _actionImage; }

    /// <summary>
    /// Checks if grid selected by input is valid for specific action
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    /// <summary>
    /// Validation of the actions' grid
    /// </summary>
    /// <returns></returns>
    public abstract List<GridPosition> GetValidActionGridPositionList();
    /// <summary>
    /// Return the gridPosition and the value(how much interests AI) of each action in poistion.
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
    /// <summary>
    /// Takes Action based on implementation, usually allows Update to run
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <param name="onActionComplete"></param>
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    protected void ActionStart(Action onActionComplete)
    {
        _isActive = true;
        _onActionComplete = onActionComplete;
    }
    protected void ActionComplete()
    {
        _isActive = false;
        _onActionComplete();
    }

}