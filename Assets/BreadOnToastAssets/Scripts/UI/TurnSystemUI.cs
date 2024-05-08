using UnityEngine.UI;
using UnityEngine;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;

    private void Start()
    {
        _endTurnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());
    }

}