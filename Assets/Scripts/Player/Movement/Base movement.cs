using System;
using UnityEngine;

public class Basemovement : MonoBehaviour
{
    public enum MoveState
    {
        Idle, Left, Right, Up, Down,
        UpLeft, UpRight, DownLeft, DownRight
    }

    public MoveState currentState { get; private set; }
    public MoveState lastState { get; private set; }   // << NEW

    [SerializeField] private float speed = 5f;
    [HideInInspector] public bool canMove = true;

    private SpriteRenderer sr;

    // idle memory timer
    private float lastMoveTimer = 0f;
    private float maxRememberTime = 30f; // 30 seconds

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!canMove)
        {
            SetIdleState();
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0, y);

        if (input.sqrMagnitude > 0.1f)
        {
            input.Normalize();
            transform.Translate(input * speed * Time.deltaTime, Space.World);

            UpdateState(x, y);
            StoreLastDirection(); // remember direction
            HandleFlipping(x);
        }
        else
        {
            SetIdleState(); // Idle, but direction is remembered
        }

        UpdateIdleMemoryTimer();
    }

    // -------------------- NEW FUNCTIONS --------------------

    private void SetIdleState()
    {
        currentState = MoveState.Idle;
    }

    private void StoreLastDirection()
    {
        if (currentState != MoveState.Idle)
        {
            lastState = currentState;
            lastMoveTimer = 0f;
        }
    }

    private void UpdateIdleMemoryTimer()
    {
        if (currentState == MoveState.Idle)
        {
            lastMoveTimer += Time.deltaTime;
            if (lastMoveTimer > maxRememberTime)
                lastState = MoveState.Idle; // forget direction after 30 sec
        }
    }

    // -------------------------------------------------------

    private void HandleFlipping(float x)
    {
        if (x > 0) sr.flipX = true;
        else if (x < 0) sr.flipX = false;
    }

    private void UpdateState(float x, float y)
    {
        if (x == 0 && y > 0) currentState = MoveState.Up;
        else if (x == 0 && y < 0) currentState = MoveState.Down;
        else if (x > 0 && y == 0) currentState = MoveState.Right;
        else if (x < 0 && y == 0) currentState = MoveState.Left;
        else if (x < 0 && y > 0) currentState = MoveState.UpLeft;
        else if (x > 0 && y > 0) currentState = MoveState.UpRight;
        else if (x < 0 && y < 0) currentState = MoveState.DownLeft;
        else if (x > 0 && y < 0) currentState = MoveState.DownRight;
    }

    public Vector3 GetDirectionVector(bool useLast = false)
    {
        MoveState state = useLast ? lastState : currentState;

        switch (state)
        {
            case MoveState.Left: return Vector3.left;
            case MoveState.Right: return Vector3.right;
            case MoveState.Up: return Vector3.forward;
            case MoveState.Down: return Vector3.back;
            case MoveState.UpLeft: return (Vector3.forward + Vector3.left).normalized;
            case MoveState.UpRight: return (Vector3.forward + Vector3.right).normalized;
            case MoveState.DownLeft: return (Vector3.back + Vector3.left).normalized;
            case MoveState.DownRight: return (Vector3.back + Vector3.right).normalized;
            default: return Vector3.forward;
        }
    }
}
