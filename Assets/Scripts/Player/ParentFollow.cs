using UnityEngine;

public class ParentFollow : MonoBehaviour
{
    [SerializeField] private Transform player;   

    void LateUpdate()
    {
        if (player == null)
            return;

        transform.position = player.position;

    }
}
