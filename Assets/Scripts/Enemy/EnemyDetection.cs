using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] public float _targetingRange = 4f;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] public Transform _player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
    }
    private void FindTarget()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _targetingRange, (Vector2)transform.position, 0f, _playerMask);
        if (hits.Length > 0 )
        {
            _player = hits[0].transform;    
        }
    }
}
