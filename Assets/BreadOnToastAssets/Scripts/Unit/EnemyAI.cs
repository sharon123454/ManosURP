using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class EnemyAI : MonoBehaviour
{
    private enum State { WaitingForEnemyTurn, TakingTurn, Busy }

    private State _state;
    private float _timer;

    private void Awake()
    {
        _state = State.WaitingForEnemyTurn;
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) { return; }

        switch (_state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        _state = State.Busy;
                    }
                    else//No more enemies who can take actions, ends turn
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }
    private void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    /// <summary>
    /// Resetting state for the next turn
    /// </summary>
    private void SetStateTakingTurn()
    {
        _timer = 0.5f;
        _state = State.TakingTurn;
    }
    /// <summary>
    /// Goes over all Enemies and checks if they can take Action
    /// </summary>
    /// <param name="onEnemyActionComplete"></param>
    /// <returns></returns>
    private bool TryTakeEnemyAIAction(Action onEnemyActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyActionComplete))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Checks if Enemy Can Take Action
    /// </summary>
    /// <param name="enemyUnit"></param>
    /// <param name="onEnemyAIActionComplete"></param>
    /// <returns></returns>
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        //Going over all enemy base actions
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendPointsToTakeAction(baseAction)) { continue; }//Enemy Cann't afford action

            //If it's first loop then set values
            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else//looking for better action value
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.ActionValue > bestEnemyAIAction.ActionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        //If you can take the action then take it
        if (bestEnemyAIAction != null && enemyUnit.TrySpendPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.GridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            _state = State.TakingTurn;
            _timer = 2f;
        }
    }

}