using UnityEngine;
using System;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit _selectedVisualUnit;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        _meshRenderer.enabled = false;
    }
    private void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs emtpy)
    {
        if (_selectedVisualUnit != null)
        {
            if (UnitActionSystem.Instance.GetSelectedUnit() == _selectedVisualUnit)
            {
                _meshRenderer.enabled = true;
            }
            else
            {
                if (_meshRenderer.enabled)
                    _meshRenderer.enabled = false;
            }
        }
        else
        {
            Debug.LogError($"UnitSelectedVisual is missing unit reference! {transform}");
        }
    }

}