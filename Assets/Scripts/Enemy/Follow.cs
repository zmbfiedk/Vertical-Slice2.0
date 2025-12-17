using System;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    private float _delay = 0f;
    [SerializeField]private List<Transform> _enemy;
    public float _speed = 1f;
    private float nX;
    private float nZ;
    void Start()
    {
       Debug.Log(_enemy.Count);
    }

    void Update()
    {
        float dist = Vector3.Distance(_enemy[0].position, transform.position);
        if (dist > 1f) {
            for (int i = 0; i < _enemy.Count; i++)
            {
                nX = _enemy[i].position.x;
                nZ = _enemy[i].position.z;
                Transform part;
                part = _enemy[i];
                nX = Mathf.MoveTowards(part.position.x, transform.position.x, _speed * Time.deltaTime);
                nZ = Mathf.MoveTowards(part.position.z, transform.position.z, _speed * Time.deltaTime);
                part.position = new Vector3(nX, part.position.y, nZ);
                Debug.Log(i);
            }
        }            
        if (dist < 1f)
        {
            for (int i = 0; i < _enemy.Count; i++)
            {
                Transform part;
                part = _enemy[i];
                nX = part.position.x;
                nZ = part.position.z;
                part.position = new Vector3(nX, part.position.y, nZ);
            }
        } 
        float distbtwnEnemies = Vector3.Distance(_enemy[0].position, _enemy[1].position);
        if (distbtwnEnemies < 0.5f)
        {
            for (int i = 0; i < _enemy.Count; i++)
            {
                Transform part;
                part = _enemy[i];
                nX = part.position.x;
                nZ = part.position.z;
                part.position = new Vector3(nX, part.position.y, nZ);
            }
        }

    }
}

