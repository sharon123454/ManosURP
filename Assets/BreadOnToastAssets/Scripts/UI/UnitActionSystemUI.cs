using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private GameObject _actionsUIParent;
    [SerializeField] private Transform _actionButtonPrefab;
    [SerializeField] private Transform _buttonContainer;

    private List<ActionButtonUI> _actionButtonUIList;

    private void Awake()
    {
        _actionButtonUIList = new List<ActionButtonUI>();
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;//might delete
        UnitActionSystem.Instance.OnActionStart += UnitActionSystem_OnActionStart;//might delete
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
    }
    private void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;//might delete
        UnitActionSystem.Instance.OnActionStart -= UnitActionSystem_OnActionStart;//might delete
        UnitActionSystem.Instance.OnBusyChanged -= UnitActionSystem_OnBusyChanged;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        _actionsUIParent.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)//might delete
    {
        //might need event on UnitUI instead of here
        UpdateActionPoints();
    }
    private void UnitActionSystem_OnActionStart(object sender, EventArgs e)//might delete
    {
        //UpdateActionPoints();
    }
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        _actionsUIParent.SetActive(!isBusy);
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedButtonVisual();
    }
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedButtonVisual();
    }

    private void UpdateActionPoints()//NEEDS VISUAL REPRESENTATION
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        Debug.Log($"Unit:{selectedUnit.transform} - AP:{selectedUnit.GetActionPoints()}, BAP{selectedUnit.GetBonusActionPoints()}");
    }
    private void UpdateSelectedButtonVisual()
    {
        foreach (ActionButtonUI actionButton in _actionButtonUIList)
        {
            actionButton.UpdateSelectedVisual();
        }
    }
    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in _buttonContainer)
        {
            Destroy(buttonTransform.gameObject);
        }
        _actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate(_actionButtonPrefab, _buttonContainer);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            _actionButtonUIList.Add(actionButtonUI);
        }
    }

}