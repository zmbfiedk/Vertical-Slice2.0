using UnityEngine;

public class AnimatieOnPlayerInteract : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Animator>().SetTrigger("PlayerInteract");
        }
    }
}