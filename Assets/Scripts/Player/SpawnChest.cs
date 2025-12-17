using UnityEngine;

public class SpawnChest : MonoBehaviour
{
    GameObject ChestPrefab;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnEnable()
    {
        EnemyCounter.OnAllEnemiesDefeated += Spawn;
    }

    private void OnDisable()
    {
        EnemyCounter.OnAllEnemiesDefeated -= Spawn;
    }

    void Spawn()
    {
        GameObject.Instantiate(ChestPrefab, transform.position, Quaternion.identity);
    }
}
