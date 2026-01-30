using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyCounter : MonoBehaviour
{
    public static event Action OnAllEnemiesDefeated;

    private List<EnemyHealth> enemies = new List<EnemyHealth>();
    private bool eventFired = false;

    private void Awake()
    {
        EnemyHealth[] foundEnemies = FindObjectsOfType<EnemyHealth>();

        foreach (EnemyHealth enemy in foundEnemies)
        {
            enemies.Add(enemy);
            enemy.OnDeath += () => OnEnemyDefeated(enemy);
        }

        Debug.Log($"EnemyCounter: {enemies.Count} enemies registered");
    }

    private void OnEnemyDefeated(EnemyHealth enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);

        Debug.Log($"Enemy defeated. Remaining: {enemies.Count}");

        if (enemies.Count == 0 && !eventFired)
        {
            eventFired = true;
            Debug.Log("All enemies defeated!");
            OnAllEnemiesDefeated?.Invoke();
        }
    }
}
