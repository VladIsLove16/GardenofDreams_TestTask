using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public event Action<float, float> HealthChanged;
    public event Action Died;
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        HealthChanged?.Invoke(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Персонаж умер!");
        Died?.Invoke();
    }
}
