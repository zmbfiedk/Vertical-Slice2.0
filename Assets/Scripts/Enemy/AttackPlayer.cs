using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] EnemyDetection _enemyDetection;
    [SerializeField] EnemyAI _enemyAI;
    private float _attackCooldown = 0f;
    [SerializeField]private float _dashDistance = 3f;
    [SerializeField]private float _dashSpeed = 5f;
    [SerializeField] private float _dashCooldown = 1.5f;
    private bool _isDashing = false;
    private Vector3 _dashTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (_isDashing) 
        {
            transform.position = Vector3.MoveTowards(transform.position, _dashTarget, _dashSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _dashTarget) < 0.1f)
            {
                _isDashing = false;
            }   
            return;
        }
        Attack();
    }
    
    private void Attack()
    {
        float dist = Vector3.Distance(_enemyDetection._player.position, transform.position);
        if (dist <= _enemyAI._attackRange) 
        {
            _attackCooldown += Time.deltaTime;
            if (_attackCooldown >= 2f) 
            {
                Vector3 direction = _enemyDetection._player.position - transform.position;
                Vector3 startPos = transform.position;
                _dashTarget = startPos + direction.normalized * _dashDistance;

                _isDashing = true;
                _attackCooldown = 0f;
            }
        }   
    }
}
