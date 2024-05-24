using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnUnitDeath;
    public event EventHandler OnUnitDamaged;

    [SerializeField] private int _health = 25;
    [SerializeField] private int _posture = 100;
    [SerializeField] private int _shield = 10;

    private int _maxHP;
    private int _maxShield;
    private int _maxPosture;

    private void Awake()
    {
        _maxHP = _health;
        _maxShield = _shield;
        _maxPosture = _posture;
    }

    public void TakeDamage(int damageAmount)
    {
        _health -= damageAmount;

        if (_health < 0) { _health = 0; }

        OnUnitDamaged?.Invoke(this, EventArgs.Empty);

        if (_health == 0) { OnDeath(); }
    }
    public int GetShield() { return _shield; }
    public float GetNormalizedHealth() { return (float)_health / _maxHP; }
    public float GetMaxHealth() { return _maxHP; }
    public int GetHealth() { return _health; }
    public float GetNormalizedPosture() { return (float)_posture / _maxPosture; }
    public float GetMaxPosture() { return _maxPosture; }
    public int GetPosture() { return _posture; }

    private void OnDeath()
    {
        OnUnitDeath?.Invoke(this, EventArgs.Empty);
    }

}