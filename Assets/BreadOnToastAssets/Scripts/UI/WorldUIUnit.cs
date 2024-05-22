using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WorldUIUnit : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private HealthSystem _unitHealthSystem;
    [SerializeField] private TextMeshProUGUI _unitNameTMP;
    [SerializeField] private TextMeshProUGUI _shieldTMP;
    [SerializeField] private TextMeshProUGUI _healthTMP;
    [SerializeField] private TextMeshProUGUI _postureTMP;
    [SerializeField] private Image _currentHealthBar;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _currentPostureBar;
    [SerializeField] private Image _postureBar;

    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        _unitNameTMP.text = _unit.name;
        UpdateVitalsVisual();
    }

    private void UpdateVitalsVisual()
    {
        int hp = _unitHealthSystem.GetHealth();
        float maxHP = _unitHealthSystem.GetMaxHealth();
        int posture = _unitHealthSystem.GetPosture();
        float maxPosture = _unitHealthSystem.GetMaxPosture();

        _shieldTMP.text = _unitHealthSystem.GetShield().ToString();

        float healthNormalized = hp / maxHP;
        float postureNormalized = posture / maxPosture;

        _currentHealthBar.fillAmount = healthNormalized;
        _healthBar.fillAmount = healthNormalized;
        _currentPostureBar.fillAmount = postureNormalized;
        _postureBar.fillAmount = postureNormalized;

        _healthTMP.text = $"{hp}/{maxHP}";
        _postureTMP.text = $"{posture}/{maxPosture}";
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool actionStarted)
    {
        if (!actionStarted)
        {
            UpdateVitalsVisual();
        }
    }

}