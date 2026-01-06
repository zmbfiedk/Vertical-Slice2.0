using UnityEngine;

public class AnimatieOnPlayerInteract : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Animator>().SetTrigger("PlayerInteract");
        }
    }
}
