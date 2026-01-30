using System;
using System.Collections;
using UnityEngine;

[HelpURL("")]
[DisallowMultipleComponent]
public class PlayerCore : Direction8InputBase
{
    /* ========================= MOVEMENT ========================= */

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private bool continueMovingDuringHold = false;
    [SerializeField] private float inputThresholdLocal = 0.0f;

    [Header("Movement Options")]
    [SerializeField] private bool useSafeMove = true;

    [Header("Dodge")]
    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeDuration = 0.3f;
    [SerializeField] private float dodgeCooldown = 1f;

    private bool isDodging;
    private float dodgeTimer;
    private Vector3 dodgeDirection;
    private float dodgeProgress;

    /* ========================= COMBAT ========================= */

    [Header("Combat – Combo Attack")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    [Tooltip("Layers that can be hit by the player")]
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private float attackRadius = 0.7f;
    [SerializeField] private float attackRange = 1.2f;

    [SerializeField] private float enemyKnockback = 6f;
    [SerializeField] private float playerLunge = 0.6f;

    [Tooltip("Time between combo presses")]
    [SerializeField] private float comboInputWindow = 0.35f;

    [Tooltip("Delay after final combo hit")]
    [SerializeField] private float comboEndLag = 0.5f;

    private int comboStep = 0;        // 0 ? 1 ? 2 ? 3
    private float comboTimer = 0f;
    private bool isAttacking = false;

    // --- saved attack direction: set when an attack begins and preserved until combo reset
    private Vector3 lastAttackDirection = Vector3.zero;
    public Vector3 LastAttackDirection => lastAttackDirection;

    /* ========================= ANIMATION (ADDED) ========================= */

    [Header("Animation - Direction Objects")]
    [Tooltip("Assign GameObjects matching Direction8 order: Idle, N, NE, E, SE, S, SW, W, NW.\nIf any slot is left null the script will try to find a child by the enum name.")]
    [SerializeField] private GameObject[] directionObjects;

    [Tooltip("Optional: manually choose an Idle fallback object in the inspector. If left null the script will search for Direction8.Idle, then a child named 'Idle', then fall back to this.gameObject.")]
    [SerializeField] private GameObject idleFallbackOverride = null;

    // resolved fallback used at runtime
    private GameObject idleFallback;

    // track last applied direction to avoid unnecessary SetActive calls
    private Direction8 lastDirection = Direction8.Idle;

    /* ========================= UNITY ========================= */

    void Awake()
    {
        // Ensure the array has the right size (one slot per enum value)
        int enumCount = Enum.GetNames(typeof(Direction8)).Length;
        if (directionObjects == null || directionObjects.Length != enumCount)
        {
            var newArr = new GameObject[enumCount];
            if (directionObjects != null)
            {
                for (int i = 0; i < Mathf.Min(directionObjects.Length, enumCount); i++)
                    newArr[i] = directionObjects[i];
            }
            directionObjects = newArr;
        }

        // Auto-fill missing entries by looking for a child with the same name as the enum
        AutoFillDirectionsWithIdleFallback();
    }

    void Start()
    {
        // Initialize the active direction visuals
        UpdateActiveDirection(CurrentDirection);
        lastDirection = CurrentDirection;
    }

#if UNITY_EDITOR
    // Make it easy to auto-fill while editing in the Inspector
    private void OnValidate()
    {
        // guard against Unity calling this during domain reload when transform may be null
        if (this == null) return;
        try
        {
            int enumCount = Enum.GetNames(typeof(Direction8)).Length;
            if (directionObjects == null || directionObjects.Length != enumCount)
                directionObjects = new GameObject[enumCount];

            AutoFillDirectionsWithIdleFallback();
        }
        catch { }
    }
#endif

    protected override void Update()
    {
        base.Update();

        HandleDodge();
        HandleMovement();
        HandleCombat();

        // Update animation children after all logic so CurrentDirection is final for this frame
        if (CurrentDirection != lastDirection)
        {
            UpdateActiveDirection(CurrentDirection);
            lastDirection = CurrentDirection;
        }
    }

    /* ========================= MOVEMENT ========================= */

    private void HandleMovement()
    {
        if (isDodging || isAttacking) return;

        float threshold = inputThresholdLocal > 0 ? inputThresholdLocal : inputThreshold;
        Vector2 input = GetRawInput();
        bool hasInput = input.magnitude >= threshold;

        Vector3 dir = Vector3.zero;

        if (hasInput)
            dir = new Vector3(input.x, 0f, input.y).normalized;
        else if (continueMovingDuringHold && CurrentDirection != Direction8.Idle)
            dir = DirectionToVector3(CurrentDirection);

        Vector3 targetVel = dir * moveSpeed;
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        currentHorizontal = Vector3.MoveTowards(
            currentHorizontal,
            targetVel,
            acceleration * Time.deltaTime
        );

        currentVelocity.x = currentHorizontal.x;
        currentVelocity.z = currentHorizontal.z;

        Vector3 delta = currentVelocity * Time.deltaTime;

        if (useSafeMove)
            SafeMove(delta);
        else
            transform.position += delta;
    }

    private void HandleDodge()
    {
        if (dodgeTimer > 0f)
            dodgeTimer -= Time.deltaTime;

        if (!isDodging && dodgeTimer <= 0f && Input.GetKeyDown(KeyCode.Space))
            StartDodge();

        if (!isDodging) return;

        dodgeProgress += Time.deltaTime / dodgeDuration;
        Vector3 dodgeVel = dodgeDirection * (dodgeDistance / dodgeDuration);

        currentVelocity.x = dodgeVel.x;
        currentVelocity.z = dodgeVel.z;

        SafeMove(currentVelocity * Time.deltaTime);

        if (dodgeProgress >= 1f)
        {
            isDodging = false;
            dodgeTimer = dodgeCooldown;
        }
    }

    private void StartDodge()
    {
        Vector2 input = GetRawInput();
        dodgeDirection = new Vector3(input.x, 0f, input.y).normalized;

        if (dodgeDirection == Vector3.zero)
            dodgeDirection = DirectionToVector3(CurrentDirection);

        dodgeProgress = 0f;
        isDodging = true;
    }

    /* ========================= COMBAT ========================= */

    private void HandleCombat()
    {
        if (comboTimer > 0f)
            comboTimer -= Time.deltaTime;
        else if (comboStep > 0 && !isAttacking)
            ResetCombo();

        if (Input.GetKeyDown(attackKey) && !isDodging)
        {
            if (!isAttacking)
                StartCoroutine(AttackCombo());
        }
    }

    private IEnumerator AttackCombo()
    {
        isAttacking = true;
        comboStep++;
        comboStep = Mathf.Clamp(comboStep, 1, 3);

        // Capture and SAVE the attack direction at the start of this attack step
        Vector3 dir = GetAttackDirection();
        lastAttackDirection = dir;

        // Player lunge
        ApplyImpulse(dir * playerLunge);

        // ---- HIT ENEMIES ----
        Vector3 hitCenter = transform.position + dir * attackRange;
        Collider[] hits = Physics.OverlapSphere(
            hitCenter,
            attackRadius,
            enemyLayers,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider c in hits)
        {
            // --- KNOCKBACK ---
            BasePhysics bp = c.GetComponentInParent<BasePhysics>();
            if (bp != null)
            {
                bp.ApplyImpulse(dir * enemyKnockback);
                bp.StartKnockback(dir, enemyKnockback, 0.2f);
            }

            // --- DAMAGE ---
            EnemyHealth health = c.GetComponentInParent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(1); // damage per hit
            }
        }

        comboTimer = comboInputWindow;

        if (comboStep >= 3)
        {
            yield return new WaitForSeconds(comboEndLag);
            ResetCombo();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            isAttacking = false;
        }
    }

    private void ResetCombo()
    {
        comboStep = 0;
        isAttacking = false;
        comboTimer = 0f;

        // Clear saved attack direction when combo ends
        lastAttackDirection = Vector3.zero;
    }

    private Vector3 GetAttackDirection()
    {
        Vector2 input = GetRawInput();
        Vector3 dir = new Vector3(input.x, 0f, input.y).normalized;

        if (dir == Vector3.zero)
            dir = DirectionToVector3(CurrentDirection);

        if (dir == Vector3.zero)
            dir = transform.forward;

        return dir;
    }

    /* ========================= UTIL ========================= */

    protected Vector3 DirectionToVector3(Direction8 dir)
    {
        switch (dir)
        {
            case Direction8.N: return Vector3.forward;
            case Direction8.NE: return (Vector3.forward + Vector3.right).normalized;
            case Direction8.E: return Vector3.right;
            case Direction8.SE: return (Vector3.back + Vector3.right).normalized;
            case Direction8.S: return Vector3.back;
            case Direction8.SW: return (Vector3.back + Vector3.left).normalized;
            case Direction8.W: return Vector3.left;
            case Direction8.NW: return (Vector3.forward + Vector3.left).normalized;
            default: return Vector3.zero;
        }
    }

    /// <summary>
    /// Enables the GameObject matching the given Direction8 and disables all others.
    /// Uses a resolved idleFallback when a specific direction object is missing.
    /// </summary>
    private void UpdateActiveDirection(Direction8 dir)
    {
        int targetIndex = (int)dir;

        GameObject activeGO = null;
        if (targetIndex >= 0 && targetIndex < directionObjects.Length)
            activeGO = directionObjects[targetIndex];

        if (activeGO == null)
            activeGO = idleFallback ?? this.gameObject;

        for (int i = 0; i < directionObjects.Length; i++)
        {
            GameObject go = directionObjects[i];

            // Null check — skip if this child isn't present
            if (go == null) continue;

            bool shouldBeActive = (go == activeGO);

            // Only change active state if needed
            if (go.activeSelf != shouldBeActive)
                go.SetActive(shouldBeActive);
        }
    }

    /// <summary>
    /// Auto-fill missing direction slots by looking for children that match the enum name.
    /// Then resolve an idleFallback GameObject to be used at runtime for any missing entries.
    /// </summary>
    private void AutoFillDirectionsWithIdleFallback()
    {
        // Fill by enum-name children
        for (int i = 0; i < directionObjects.Length; i++)
        {
            if (directionObjects[i] == null)
            {
                string childName = ((Direction8)i).ToString();
                Transform child = transform.Find(childName);
                if (child != null)
                    directionObjects[i] = child.gameObject;
            }
        }

        // Resolve idle fallback priority:
        // 1) inspector override
        // 2) directionObjects[Direction8.Idle]
        // 3) child named "Idle"
        // 4) this.gameObject

        if (idleFallbackOverride != null)
        {
            idleFallback = idleFallbackOverride;
            return;
        }

        int idleIndex = -1;
        try { idleIndex = (int)Direction8.Idle; } catch { idleIndex = -1; }

        if (idleIndex >= 0 && idleIndex < directionObjects.Length && directionObjects[idleIndex] != null)
        {
            idleFallback = directionObjects[idleIndex];
            return;
        }

        Transform idleChild = transform.Find("Idle");
        if (idleChild != null)
        {
            idleFallback = idleChild.gameObject;
            return;
        }

        idleFallback = this.gameObject;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 dir = GetAttackDirection();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + dir * attackRange, attackRadius);

        // visualize saved attack direction while attacking / during combo
        if (lastAttackDirection != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + lastAttackDirection.normalized * (attackRange + 0.5f));
        }
    }
#endif
}
