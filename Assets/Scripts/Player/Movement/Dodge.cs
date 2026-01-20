using System.Collections;
using UnityEngine;

public class Dodge : MonoBehaviour
{
    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeDuration = 0.2f;

    [SerializeField] private float cooldownTime = 1f;
    private float cooldownTimer = 0f;

    private Collider col;
    private bool isDodging = false;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && !isDodging && cooldownTimer <= 0f)
        {
            OnDodge();
        }
    }

    void OnDodge()
    {
        cooldownTimer = cooldownTime;

        StartCoroutine(DodgeRoutine());
    }

    private IEnumerator DodgeRoutine()
    {
        isDodging = true;

        col.enabled = false;

        Vector3 dodgeDir = transform.forward;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + dodgeDir * dodgeDistance;

        float elapsed = 0f;

        while (elapsed < dodgeDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / dodgeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        col.enabled = true;

        isDodging = false;
    }
}
