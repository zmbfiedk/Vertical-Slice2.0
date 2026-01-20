using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] EnemyDetection _enemyDetection;
    [SerializeField] EnemyAI _enemyAI;
    private float _attackCooldown = 0f;
    private float _dashDistance = 6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _attackCooldown += Time.deltaTime;
        Attack();
    }
    
    private void Attack()
    {
        float dist = Vector3.Distance(_enemyDetection._player.position, transform.position);
        if (_attackCooldown >= 1f && dist <= _enemyAI._attackRange) 
        {
            Vector3 direction = _enemyDetection._player.position - transform.position;
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + direction.normalized * _dashDistance;
            
            Vector3.MoveTowards(transform.position, targetPos, 10f);
            _attackCooldown = 0f;
        }   
    }
}
