using System;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public static event Action OnAllEnemiesDefeated;
    private Transform[] enemies;


    void Start()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        enemies = new Transform[enemyObjects.Length];

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemies[i] = enemyObjects[i].transform;
            Debug.Log("Enemy " + i + ": " + enemies[i].name);
        }
    }

    void Update()
    {
        if(enemies.Length == 0)
        {
            OnAllEnemiesDefeated?.Invoke();
            Debug.Log("No enemies found.");
            return;
        }
        
    }

    public void EnemyDefeated(Transform enemyTransform)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == enemyTransform)
            {
                enemies[i] = enemies[enemies.Length - 1];
                Array.Resize(ref enemies, enemies.Length - 1);
                Debug.Log("Enemy defeated: " + enemyTransform.name);
                break;
            }
        }

        if (enemies.Length == 0)
        {
            OnAllEnemiesDefeated?.Invoke();
            Debug.Log("All enemies defeated!");
        }
    }
}
