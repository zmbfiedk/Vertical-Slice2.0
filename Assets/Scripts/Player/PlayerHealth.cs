using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int currentHP;

    [Header("Hit Settings")]
    [SerializeField] private float invincibleTime = 0.5f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.15f;
    [SerializeField] private float flashDuration = 0.1f;

    private bool isInvincible;
    private bool isKnocked;

    private Animator anim;
    private Basemovement move;
    private BaseCombat combat;
    private Dodge dodge;

    private Rigidbody rb;
    private SpriteRenderer sprite;
    private Color originalColor;

    public static Action OnPlayerDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        move = GetComponent<Basemovement>();
        combat = GetComponent<BaseCombat>();
        dodge = GetComponent<Dodge>();

        currentHP = maxHP;
    }

    // ================= DAMAGE ================= //

    public void TakeDamage(int amount, Transform damageSource = null)
    {
        if (isInvincible || isKnocked) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        anim.Play("Wounded");

        StartCoroutine(InvincibilityTimer());
        StartCoroutine(FlashWhite());

        if (damageSource != null)
        {
            Vector2 dir = (transform.position - damageSource.position).normalized;
            StartCoroutine(Knockback(dir));
        }

        if (currentHP <= 0)
            Die();
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    // ================= KNOCKBACK ================= //

    private IEnumerator Knockback(Vector2 direction)
    {
        isKnocked = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    // ================= FLASH ================= //

    private IEnumerator FlashWhite()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sprite.color = originalColor;
    }

    // ================= DEATH ================= //

    private void Die()
    {
        move.canMove = false;
        if (combat != null) combat.enabled = false;
        if (dodge != null) dodge.enabled = false;

        anim.Play("Death");
        OnPlayerDeath?.Invoke();

        Debug.Log("Player Dead");
    }

    // ================= HEAL ================= //

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }

    // ================= ENEMY COLLISION ================= //

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamage(1, collision.transform);
        }
    }
}
