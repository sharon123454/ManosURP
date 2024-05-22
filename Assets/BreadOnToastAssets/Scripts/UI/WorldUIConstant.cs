using UnityEngine.UI;
using UnityEngine;

public class WorldUIConstant : MonoBehaviour
{
    [SerializeField] private HealthSystem _unitHealthSystem;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _postureBar;

    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool e)
    {
        _healthBar.fillAmount = _unitHealthSystem.GetNormalizedHealth();
        _postureBar.fillAmount = _unitHealthSystem.GetNormalizedPosture();
    }

}