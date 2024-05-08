using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;

    private bool _isPlayerTurn = true;
    private int _turnNumber = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one TurnSystem! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void NextTurn()
    {
        _turnNumber++;
        _isPlayerTurn = !_isPlayerTurn;
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }
    public int GetTurnNumber() { return _turnNumber; }
    public bool IsPlayerTurn() { return _isPlayerTurn; }

}