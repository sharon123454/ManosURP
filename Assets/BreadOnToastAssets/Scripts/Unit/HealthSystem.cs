using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnUnitDeath;

    [SerializeField] private int _health = 100;

    public void TakeDamage(int damageAmount)
    {
        _health -= damageAmount;

        if (_health <= 0)
        {
            _health = 0;
            OnDeath();
        }
    }

    private void OnDeath()
    {
        OnUnitDeath?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

}