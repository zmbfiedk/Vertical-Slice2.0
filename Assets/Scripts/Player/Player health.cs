using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime = 0.5f;

    public int CurrentHealth { get; private set; }
    public bool IsInvincible { get; private set; }

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsInvincible) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        OnDeath?.Invoke();
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        IsInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        IsInvincible = false;
    }
}
