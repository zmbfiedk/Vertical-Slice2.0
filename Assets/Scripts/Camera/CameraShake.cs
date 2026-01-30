using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeStrength = 0.6f;

    private Vector3 originalLocalPos;

    private void Awake()
    {
        // Store the local position so shake resets correctly
        originalLocalPos = transform.localPosition;
    }
    


    private void Shake()
    {
        transform.DOKill();
        transform.localPosition = originalLocalPos;

        transform.DOShakePosition(
            _shakeDuration,
            _shakeStrength,
            20,
            90,
            false,
            true
        ).OnComplete(() =>
        {
            transform.localPosition = originalLocalPos;
        });
    }
}