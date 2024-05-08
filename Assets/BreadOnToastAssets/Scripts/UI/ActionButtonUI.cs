using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject _selectedButtonVisual;
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _actionNameText;
    [SerializeField] private Image _actionImage;

    private BaseAction _myBaseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        _myBaseAction = baseAction;
        _actionNameText.text = baseAction.GetActionName();

        Image actionImage = baseAction.GetActionImage();
        if (actionImage != null) { _actionImage = actionImage; }

        _actionButton.onClick.AddListener(() => { UnitActionSystem.Instance.SetSelectedAction(baseAction); });
    }
    public void UpdateSelectedVisual()
    {
        if (_selectedButtonVisual != null)
        {
            BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
            _selectedButtonVisual.SetActive(_myBaseAction == selectedBaseAction);
        }
    }

}