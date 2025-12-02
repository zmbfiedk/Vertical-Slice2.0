using UnityEngine;

public class ParentFollow : MonoBehaviour
{
    [SerializeField] private Transform player;   // the player to follow

    void LateUpdate()
    {
        if (player == null)
            return;

        // Follow only the position
        transform.position = player.position;

        // Do NOT rotate with the player
        // So this line stays intentionally empty.
    }
}
