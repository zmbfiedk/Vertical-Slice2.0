using UnityEngine;
using System;

public class Damage : MonoBehaviour
{
    public static event Action<int> damageDealt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            damageDealt?.Invoke(10);
        }
        if (collision.gameObject.CompareTag("AttackArea"))
        {
            damageDealt?.Invoke(34);
        }
    }   
}
