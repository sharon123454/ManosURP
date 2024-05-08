using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using System;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnActionStart;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;

    private string[] _activeLayerNames = { "Unit" };
    private LayerMask _activeUnitLayerMask;

    private BaseAction _selectedAction;
    private Unit _selectedUnit;
    private bool _isBusy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one UnitActionSystem! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _activeUnitLayerMask.value = LayerMask.GetMask(_activeLayerNames);
    }
    private void Update()
    {
        //place under TryRaycast to allow switching characters mid action
        if (_isBusy) { return; }

        if (!TurnSystem.Instance.IsPlayerTurn()) { return; }

        //Checks if pointer is above UI
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        //Handles clicked character
        if (TryRaycastUnitSelection()) { return; }

        HandleSelectedAction();
    }

    public Unit GetSelectedUnit() { return _selectedUnit; }
    public BaseAction GetSelectedAction() { return _selectedAction; }
    public void SetSelectedAction(BaseAction action)
    {
        _selectedAction = action;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    private void SetSelectedUnit(Unit unit)
    {
        _selectedUnit = unit;
        SetSelectedAction(_selectedUnit.GetMoveAction());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDown())
        {
            //Stops action logic if no Unit selected
            if (GetSelectedUnit() == null) { Debug.Log("No unit is selected"); return; }

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!GetSelectedAction().IsValidActionGridPosition(mouseGridPosition)) { Debug.Log("Position clicked isn't valid"); return; }
            if (!GetSelectedUnit().TrySpendPointsToTakeAction(GetSelectedAction())) { return; }

            SetBusy();
            GetSelectedAction().TakeAction(mouseGridPosition, ClearBusy);
            OnActionStart?.Invoke(this, EventArgs.Empty);
        }

        #region deprecated conversion of baseAction to inhereting Action, call to TakeAction
        //if (InputManager.Instance.IsMouseButtonDown())
        //{
        //    switch (_selectedAction)
        //    {
        //        case MoveAction moveAction:
        //            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        //            if (moveAction.IsValidActionGridPosition(mouseGridPosition))
        //            {
        //                SetBusy();
        //                moveAction.Move(mouseGridPosition, ClearBusy);
        //            }
        //            else { Debug.Log("Position clicked isn't valid"); }
        //            break;
        //        case SpinAction spinAction:
        //            SetBusy();
        //            spinAction.Spin(ClearBusy);
        //            break;
        //        default:
        //            break;
        //    }
        //}
        #endregion
    }
    private bool TryRaycastUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDown())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetPointerPosition());
            if (Physics.Raycast(ray, out RaycastHit rayCastHit, float.MaxValue, _activeUnitLayerMask))
            {
                if (rayCastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (GetSelectedUnit() == unit) { return false; }
                    if (unit.IsEnemy()) { return false; }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }
    private void SetBusy() { _isBusy = true; OnBusyChanged?.Invoke(this, _isBusy); }
    private void ClearBusy() { _isBusy = false; OnBusyChanged?.Invoke(this, _isBusy); }

}