using UnityEngine;

public class PropCircleFilter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]private  float hideRadius = 3f;        // circle around camera or player
    [SerializeField] private LayerMask Treelayer;         // only affects props on this layer

    private Collider[] foundProps;

    void Update()
    {
        foundProps = Physics.OverlapSphere(transform.position, hideRadius, Treelayer);

        foreach (Collider prop in foundProps)
        {
            SpriteRenderer r = prop.GetComponent<SpriteRenderer>();
            if (r != null) r.enabled = false; // hides while inside circle
        }
    }

    private void LateUpdate()
    {
        // Restore any props that moved outside the circle
        Collider[] all = Physics.OverlapSphere(transform.position, hideRadius + 0.1f, Treelayer);

        foreach (Collider prop in all)
        {
            SpriteRenderer r = prop.GetComponent<SpriteRenderer>();
            if (r != null && Vector3.Distance(transform.position, prop.transform.position) > hideRadius)
                r.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hideRadius);
    }
}
