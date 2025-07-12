using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform _projectileParent;

    [SerializeField] private GameObject[] _enemyPrefabs;

    [Space(15)]
    [SerializeField] private Vector3[] _enemySpawnPositions;

    [Space(15)]
    [Header("Multipliers")]
    [SerializeField] private float _baseHealthMultiplier;
    [SerializeField] private float _healthMultiplier;
    [SerializeField] private float _baseDamageMultiplier;
    [SerializeField] private float _damageMultiplier;

    [Space(5)]
    [SerializeField] private float _incrementOnEvery;


    [Space(15)]
    [Header("Timers")]
    [SerializeField] private float spawnInterval;

    private GlobalTimer _spawnTimer;


    [Space(15)]
    [Header("Debug")]
    [SerializeField] private int _incrementCounter;

    [HideInInspector] public static EnemySpawner SSpawnerScript;

    public static bool  shouldSpawn;


    void Start()
    {
        shouldSpawn = true;
        _spawnTimer = new GlobalTimer(0);
    }

    void Update()
    {
        if(shouldSpawn == false) 
        {
            return;
        }


        if (_incrementCounter > _incrementOnEvery)
        {
            _healthMultiplier++;
            _damageMultiplier++;
            _incrementCounter = 0;
        }

        
        _spawnTimer.Tick();
        
        if (_spawnTimer.Flag)
        {
            SpawnEnemy();
            
            _spawnTimer.Reset(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (_enemyPrefabs.Length == 0 || _enemySpawnPositions.Length == 0)
        {
            Debug.LogWarning("Spawner: enemyPrefabs or spawnPositions list is empty!");
            return;
        }

        GameObject prefab = _enemyPrefabs[UnityEngine.Random.Range(0, _enemyPrefabs.Length)];
        //Vector3 position = _enemySpawnPositions[UnityEngine.Random.Range(0, _enemySpawnPositions.Length)];
        Vector3 position = new Vector3(UnityEngine.Random.Range(1.5f, 15.5f), 0, 65);
        var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _projectileParent);
        }

        _incrementCounter++;
    }

    private void ResetSpawner()
    {
        //Reset
        _spawnTimer = new GlobalTimer(0);
        _incrementCounter = 0;
        _healthMultiplier = _baseHealthMultiplier;
        _damageMultiplier = _baseDamageMultiplier;
        //ClearEnemies
        foreach (var enemy in Player.SPlayerScript.enemies)
        {
            Destroy(enemy);
        }
        shouldSpawn = true;
    }
}