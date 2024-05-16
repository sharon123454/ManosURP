using UnityEngine.Rendering.Universal;
using UnityEngine;
using System;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit _selectedVisualUnit;

    private DecalProjector _decalRenderer;

    private void Awake()
    {
        _decalRenderer = GetComponent<DecalProjector>();
    }
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        _decalRenderer.enabled = false;
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
                _decalRenderer.enabled = true;
            }
            else
            {
                if (_decalRenderer.enabled)
                    _decalRenderer.enabled = false;
            }
        }
        else
        {
            Debug.LogError($"UnitSelectedVisual is missing unit reference! {transform}");
        }
    }

}