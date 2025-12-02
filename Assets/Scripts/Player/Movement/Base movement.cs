using UnityEngine;

public class Basemovement : MonoBehaviour
{
    public enum MoveState
    {
        Idle,
        Left,
        Right,
        Up,
        Down,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public MoveState currentState { get; private set; }

    [SerializeField] private float speed = 5f;
    [HideInInspector] public bool canMove = true;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        // Make sure your sprite is a child with a SpriteRenderer!
    }

    void Update()
    {
        if (!canMove)
        {
            currentState = MoveState.Idle;
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
            HandleFlipping(x);
        }
        else
        {
            currentState = MoveState.Idle;
        }
    }

    private void HandleFlipping(float x)
    {
        // Flip sprite ONLY based on horizontal direction
        if (x > 0)
            sr.flipX = true;
        else if (x < 0)
            sr.flipX = false;
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

    public Vector3 GetDirectionVector()
    {
        switch (currentState)
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
