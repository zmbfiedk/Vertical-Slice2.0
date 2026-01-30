using UnityEngine;

public class SpawnChest : MonoBehaviour
{
    [SerializeField] GameObject ChestPrefab;

    void Start()
    {
        if (ChestPrefab == null)
        {
            Debug.Log("No Chest Prefab assigned in SpawnChest script.");
        }
        else

        {
            Debug.Log("Chest Prefab assigned in SpawnChest script.");
        }
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