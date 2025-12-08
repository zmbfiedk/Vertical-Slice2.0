using System.Collections;
using UnityEngine;

public class Dodge : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float cooldownTime = 1f;

    private float cooldownTimer = 0f;
    private bool isDodging = false;

    private Collider col;
    private Basemovement move;
    private Animator anim;

    void Start()
    {
        col = GetComponent<Collider>();
        move = GetComponent<Basemovement>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        bool isMoving = move.currentState != Basemovement.MoveState.Idle;

        if (Input.GetKeyDown(KeyCode.Space) && isMoving && !isDodging && cooldownTimer <= 0)
        {
            StartCoroutine(DodgeRoutine());
        }
    }

    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        cooldownTimer = cooldownTime;
        col.enabled = false;

        Vector3 dir = move.GetDirectionVector(true);
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + dir * dodgeDistance;

        // ANIM CORRESPONDS TO DIRECTION
        if (dir.x > 0) anim.Play("Dash_Forward");
        else if (dir.x < 0) anim.Play("Dash_Behind");
        else anim.Play("Dash_sideways");

        float t = 0f;
        while (t < dodgeDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / dodgeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        col.enabled = true;
        isDodging = false;
    }
}
