using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int currentHP;

    [Header("Hit Settings")]
    [SerializeField] private float invincibleTime = 0.5f; // prevents instant double damage
    private bool isInvincible = false;

    private Animator anim;
    private Basemovement move;
    private BaseCombat combat;
    private Dodge dodge;

    public static Action OnPlayerDeath;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<Basemovement>();
        combat = GetComponent<BaseCombat>();
        dodge = GetComponent<Dodge>();

        currentHP = maxHP;
    }

    // ================= DAMAGE ================= //

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        anim.Play("Wounded");   // <<< PLAY DAMAGE ANIMATION

        StartCoroutine(InvincibilityTimer());

        if (currentHP <= 0)
            Die();
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    // ================= DEATH ================= //

    private void Die()
    {
        move.canMove = false;
        if (combat != null) combat.enabled = false;
        if (dodge != null) dodge.enabled = false;

        anim.Play("Death"); // <<< PLAY DEATH ANIMATION
        OnPlayerDeath?.Invoke();

        Debug.Log("Player Dead");
    }

    // ================= HEAL (optional) ================= //

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }
}
