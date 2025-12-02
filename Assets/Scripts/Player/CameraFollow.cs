using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float followSpeed = 7f;

    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {
        Vector3 targetPos = _player.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
    }
}
