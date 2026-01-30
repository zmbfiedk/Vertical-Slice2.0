using UnityEngine;
using System;

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Hit Settings")]
    [SerializeField] private float hitInvincibilityTime = 0.15f;


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
        Debug.Log($"{name} died");

        OnDeath?.Invoke();

        // Optional cleanup
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        IsInvincible = true;
        yield return new WaitForSeconds(hitInvincibilityTime);
        IsInvincible = false;
    }
}
