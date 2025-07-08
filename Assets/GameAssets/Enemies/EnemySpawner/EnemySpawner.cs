using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("List of enemy prefabs to pick from randomly.")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Tooltip("List of spawn positions to pick from randomly.")]
    [SerializeField] private Vector3[] spawnPositions;

    [Tooltip("Time between spawns (seconds).")]
    [SerializeField] private float spawnInterval = 3f;

    [Tooltip("Maximum number of enemies to spawn (-1 for infinite).")]
    [SerializeField] private int maxSpawnCount = -1;

    [SerializeField] private Vector3 player;

    private float timer;
    private int spawnedCount;

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f && (maxSpawnCount < 0 || spawnedCount < maxSpawnCount))
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPositions.Length == 0)
        {
            Debug.LogWarning("Spawner: enemyPrefabs or spawnPositions list is empty!");
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector3 position = new Vector3(Random.Range(1.5f, 15f),0, 65);        
        Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));

        spawnedCount++;
    }
}
