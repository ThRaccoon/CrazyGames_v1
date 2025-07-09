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

    [SerializeField] private Vector3 player;

    [SerializeField] private float _health;
    [SerializeField] private float _healthMultiplier;
    [SerializeField] private float _upgradeOnEvery;
    [SerializeField] private int _upgradeCounter = 0;

    private float timer;

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        if(_upgradeCounter > _upgradeOnEvery )
        {
            _health *= _healthMultiplier;
            _upgradeCounter = 0;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
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

        GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        Vector3 position = new Vector3(UnityEngine.Random.Range(1.5f, 15f),0, 65);        
       var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();
        if(enemyScript != null) 
        {
            enemyScript.Init(_health);
        }

        _upgradeCounter ++;
    }
}
