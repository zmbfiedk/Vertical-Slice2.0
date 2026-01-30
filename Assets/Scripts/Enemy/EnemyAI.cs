using System.Collections;
using UnityEngine;

public class EnemyAI : BasePhysics
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.2f;
    [SerializeField] private float randomOffsetRadius = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashKnockbackStrength = 10f;
    [SerializeField] private float dashDelay = 0.5f;

    /* ================= KNOCKBACK STATE ================= */

    public bool IsBeingKnockedBack { get; private set; }

    /* ================= AI STATE ================= */

    private Vector3 targetPosition;

    private enum State { Idle, MoveToRandom, DashToPlayer, Stunned }
    private State currentState = State.Idle;

    protected override void Awake()
    {
        base.Awake();

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        // HARD STOP: if knockback is active, AI cannot move
        if (IsBeingKnockedBack)
        {
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            return;
        }

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= detectionRange)
                {
                    PickRandomTargetNearPlayer();
                    currentState = State.MoveToRandom;
                }
                break;

            case State.MoveToRandom:
                MoveTowardsTarget();
                break;

            case State.DashToPlayer:
                // handled by coroutine
                break;

            case State.Stunned:
                // waiting for knockback to end
                break;
        }
    }

    /* ================= MOVEMENT ================= */

    private void PickRandomTargetNearPlayer()
    {
        Vector2 randCircle = Random.insideUnitCircle * randomOffsetRadius;
        targetPosition = player.position + new Vector3(randCircle.x, 0f, randCircle.y);
    }

    private void MoveTowardsTarget()
    {
        Vector3 dir = targetPosition - transform.position;
        dir.y = 0f;
        float distance = dir.magnitude;

        if (distance <= stoppingDistance)
        {
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            StartCoroutine(DashToPlayerCoroutine());
            return;
        }

        dir.Normalize();
        currentVelocity.x = dir.x * moveSpeed;
        currentVelocity.z = dir.z * moveSpeed;

        SafeMove(currentVelocity * Time.deltaTime);
    }

    /* ================= DASH ================= */

    private IEnumerator DashToPlayerCoroutine()
    {
        currentState = State.DashToPlayer;

        yield return new WaitForSeconds(dashDelay);

        Vector3 dashDir = player.position - transform.position;
        dashDir.y = 0f;
        dashDir.Normalize();

        float t = 0f;
        float checkRadius = 1f;
        bool hitPlayer = false;

        while (t < dashDuration && !IsBeingKnockedBack)
        {
            currentVelocity = dashDir * dashSpeed;
            SafeMove(currentVelocity * Time.deltaTime);

            if (!hitPlayer)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);
                foreach (Collider c in hits)
                {
                    if (c.transform == player)
                    {
                        Vector3 knockDir = player.position - transform.position;
                        knockDir.y = 0f;
                        knockDir.Normalize();

                        // Knockback
                        BasePhysics playerPhysics = player.GetComponent<BasePhysics>();
                        if (playerPhysics != null)
                            playerPhysics.ApplyImpulse(knockDir * dashKnockbackStrength);

                        // Damage
                        PlayerHealth health = player.GetComponent<PlayerHealth>();
                        if (health != null)
                        {
                            health.TakeDamage(1); // <- damage per dash hit
                        }


                        hitPlayer = true;
                        break;
                    }
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        currentVelocity = Vector3.zero;
        currentState = State.Idle;
    }

    /* ================= KNOCKBACK HOOKS ================= */

    public override void StartKnockback(Vector3 direction, float strength, float duration, AnimationCurve falloff = null)
    {
        IsBeingKnockedBack = true;
        currentState = State.Stunned;
        StopAllCoroutines(); // cancel dash / movement immediately
        base.StartKnockback(direction, strength, duration, falloff);
    }

    protected override void OnKnockbackEnded()
    {
        IsBeingKnockedBack = false;
        currentState = State.Idle;
    }

    /* ================= GIZMOS ================= */

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, randomOffsetRadius);
        }
    }
}