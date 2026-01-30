using UnityEngine;

public enum Direction8
{
    Idle,
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW
}
public class Direction8InputBase : BasePhysics
{
    [Header("Input")]
    [SerializeField] protected float inputThreshold = 0.1f;
    [SerializeField] protected float holdDuration = 3f;

    public Direction8 CurrentDirection { get; protected set; } = Direction8.Idle;
    protected Direction8 lastNonIdleDirection = Direction8.N;
    protected float timeSinceInputStopped;

    protected override void Update()
    {
        // run physics (gravity + drag + grounded check)
        base.Update();

        Vector2 input = GetRawInput();

        if (input.magnitude >= inputThreshold)
        {
            Direction8 dir = VectorToDirection8(input);
            SetDirection(dir);
            lastNonIdleDirection = dir;
            timeSinceInputStopped = 0f;
        }
        else
        {
            timeSinceInputStopped += Time.deltaTime;
            if (timeSinceInputStopped < holdDuration)
                SetDirection(lastNonIdleDirection);
            else
                SetDirection(Direction8.Idle);
        }
    }

    protected Vector2 GetRawInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (Mathf.Approximately(x, 0f))
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x = 1f;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        }

        if (Mathf.Approximately(y, 0f))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y = 1f;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y = -1f;
        }

        return new Vector2(x, y);
    }

    protected Direction8 VectorToDirection8(Vector2 v)
    {
        if (v == Vector2.zero) return Direction8.Idle;

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        int sector = Mathf.RoundToInt(angle / 45f) % 8;

        switch (sector)
        {
            case 0: return Direction8.E;
            case 1: return Direction8.NE;
            case 2: return Direction8.N;
            case 3: return Direction8.NW;
            case 4: return Direction8.W;
            case 5: return Direction8.SW;
            case 6: return Direction8.S;
            case 7: return Direction8.SE;
            default: return Direction8.Idle;
        }
    }

    protected void SetDirection(Direction8 dir)
    {
        CurrentDirection = dir;
    }

}
