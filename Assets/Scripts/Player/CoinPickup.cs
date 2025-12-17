using UnityEngine;
using DG.Tweening;

public class CoinPickup : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Rotation")]
    [SerializeField] private float flipAmount = 360f;

    void Start()
    {
        Vector3 startPos = transform.position;

        transform.DOMoveY(startPos.y + jumpHeight, jumpDuration)
                 .SetEase(Ease.OutBack);

        transform.DORotate(
            new Vector3(0f, 0f, flipAmount),
            jumpDuration,
            RotateMode.FastBeyond360
        );
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Add coin to player's inventory or score
            // PlayerInventory.Instance.AddCoin(1); // Example method

            // Destroy the coin object
            Destroy(gameObject);
        }
    }
}
