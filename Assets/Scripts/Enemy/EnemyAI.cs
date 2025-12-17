using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] EnemyDetection _enemyDetection;
    private float _moveSpeed = 1f;
    public float _attackRange = 1.5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
    }
 
    private void MoveEnemy()
    {
        float dist = Vector3.Distance(_enemyDetection._player.position, transform.position);
        if (dist <= _enemyDetection._targetingRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, _enemyDetection._player.position, _moveSpeed * Time.deltaTime);
        }
        if (dist <= _attackRange)
        {
            _moveSpeed = 0f;    
        }
        else if (dist > _attackRange && dist <= _enemyDetection._targetingRange)
        {
            _moveSpeed = 1f;
        }   
    }
}
