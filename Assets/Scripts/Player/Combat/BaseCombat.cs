using System;
using System.Collections;
using UnityEngine;

public class BaseCombat : MonoBehaviour
{
    public static Action OnHit;

    [Header("Combat Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject hurtboxPrefab;
    [SerializeField] private float delayBetweenAttacks = 0.3f;
    [SerializeField] private float comboCooldown = 1f;
    [SerializeField] private float attackRange = 1f;

    [Header("Attack Movement")]
    [SerializeField] private float forwardPush = 0.5f;
    [SerializeField] private float pushSpeed = 10f;

    [Header("Read Only")]
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private float comboResetTimer = 0f;

    private int attackIndex = 0;
    private Basemovement move;
    private Animator anim;

    void Start()
    {
        move = GetComponent<Basemovement>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (comboResetTimer > 0)
            comboResetTimer -= Time.deltaTime;

        if (comboResetTimer > 0 || isAttacking)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            attackIndex = 1;
            StartCoroutine(Attack1());
        }
    }

    private IEnumerator Attack1()
    {
        isAttacking = true;
        move.canMove = false;

        anim.Play("Attack_1");
        DoHit();
        yield return new WaitForSeconds(delayBetweenAttacks);

        float timer = delayBetweenAttacks;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                attackIndex = 2;
                StartCoroutine(Attack2());
                yield break;
            }
            yield return null;
        }

        EndCombo();
    }

    private IEnumerator Attack2()
    {
        anim.Play("Attack_2");
        DoHit();
        yield return new WaitForSeconds(delayBetweenAttacks);

        float timer = delayBetweenAttacks;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                attackIndex = 3;
                StartCoroutine(Attack3());
                yield break;
            }
            yield return null;
        }

        EndCombo();
    }

    private IEnumerator Attack3()
    {
        anim.Play("Attack_3");
        DoHit();
        yield return new WaitForSeconds(delayBetweenAttacks);

        comboResetTimer = comboCooldown;
        EndCombo();
    }

    private void EndCombo()
    {
        isAttacking = false;
        move.canMove = true;
        attackIndex = 0;
    }

    private void DoHit()
    {
        SpawnHurtbox();
        StartCoroutine(PushForward());
        OnHit?.Invoke();
    }

    private IEnumerator PushForward()
    {
        Vector3 dir = move.GetDirectionVector();
        float moved = 0f;

        while (moved < forwardPush)
        {
            float step = pushSpeed * Time.deltaTime;
            transform.Translate(dir * step, Space.World);
            moved += step;

            yield return null;
        }
    }

    private void SpawnHurtbox()
    {
        if (hurtboxPrefab == null) return;

        Vector3 dir = move.GetDirectionVector();
        Vector3 spawnPos = transform.position + dir * attackRange;

        GameObject hb = Instantiate(hurtboxPrefab, spawnPos, Quaternion.identity);
        Destroy(hb, 0.2f);
    }
}
