using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Components
    [Header("Components")]
    [SerializeField] private Transform _projectileParent;

    [Space(5)]
    [SerializeField] private GameObject[] _enemyPrefabs;

    [Space(5)]
    [SerializeField] private GameObject _barrelPrefab;

    [Space(5)]
    [SerializeField] private Vector3[] _barrelSpawnPositions;
    #endregion

    #region Multipliers
    [Space(15)]
    [Header("Multipliers")]
    [SerializeField] private float _baseHealthMultiplier;
    [SerializeField] private float _healthMultiplier;

    [SerializeField] private float _baseDamageMultiplier;
    [SerializeField] private float _damageMultiplier;

    [SerializeField] private float _baseExpRewardMultiplier;
    [SerializeField] private float _expRewardMultiplier;
    #endregion

    #region Timers
    [Space(15)]
    [Header("Timers")]
    [SerializeField] private float _mobSpawnInterval;
    private GlobalTimer _mobSpawnTimer;

    [SerializeField] private float _barrelSpawnInterval;
    private GlobalTimer _barrelSpawnTimer;

    [SerializeField] private float _bossSpawnInterval;
    private GlobalTimer _bosslSpawnTimer;

    public static bool _shouldSpawn;

    #endregion

    #region Counters
    [Space(15)]
    [Header("Counters")]
    [SerializeField] private float _multipliersIncrementThreshold;
    [SerializeField] private int _spawnedMobCountForMultipliers;

    [Space(5)]
    [SerializeField] private float _mobTypeUnlockThreshold;
    [SerializeField] private float _spawnedMobCountForMobType;
    #endregion

    #region Runtime
    [HideInInspector] public static EnemySpawner _SSpawnerScript;
    #endregion

    #region Debug
    [Space(15)]
    [Header("Debug")]
    [SerializeField] private int _mobTypes;
    #endregion

    void Update()
    {
        if (_shouldSpawn == false)
        {
            return;
        }

        if (_spawnedMobCountForMultipliers > _multipliersIncrementThreshold)
        {
            _healthMultiplier++;
            _damageMultiplier++;
            _expRewardMultiplier++;

            _spawnedMobCountForMultipliers = 0;
        }

        if (_spawnedMobCountForMobType > _mobTypeUnlockThreshold)
        {
            if (_mobTypes < _enemyPrefabs.Length - 1)
            {
                _mobTypes++;
            }

            _spawnedMobCountForMobType = 0;
        }

        _mobSpawnTimer.Tick();

        if (_mobSpawnTimer.Flag)
        {
            SpawnEnemy();

            _mobSpawnTimer.Reset(_mobSpawnInterval);
        }

        _barrelSpawnTimer.Tick();

        if (_barrelSpawnTimer.Flag)
        {
            SpawnBarrel();

            _barrelSpawnTimer.Reset();
        }
    }

    private void SpawnEnemy()
    {
        if (_enemyPrefabs.Length == 0 || _barrelSpawnPositions.Length == 0)
        {
            Debug.LogWarning("Spawner: enemyPrefabs or spawnPositions list is empty!");
            return;
        }

        GameObject prefab = _enemyPrefabs[UnityEngine.Random.Range(0, _mobTypes)];
        Vector3 position = new Vector3(UnityEngine.Random.Range(1.5f, 15.5f), 0, 65);

        var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _expRewardMultiplier, _projectileParent);
        }

        _spawnedMobCountForMultipliers++;
        _spawnedMobCountForMobType++;
    }

    private void SpawnBarrel()
    {
        Vector3 position = _barrelSpawnPositions[UnityEngine.Random.Range(0, _barrelSpawnPositions.Length)];

        var barrel = Instantiate(_barrelPrefab, position, Quaternion.Euler(0, 0, 90));
    }

    private void ResetSpawner()
    {
        //Reset
        _mobSpawnTimer = new GlobalTimer(0);
        _barrelSpawnTimer = new GlobalTimer(_barrelSpawnInterval);
        _bosslSpawnTimer = new GlobalTimer(_bossSpawnInterval);

        _spawnedMobCountForMultipliers = 0;
        _spawnedMobCountForMobType = 0;

        _healthMultiplier = _baseHealthMultiplier;
        _damageMultiplier = _baseDamageMultiplier;
        _expRewardMultiplier = _baseExpRewardMultiplier;

        //ClearEnemies
        foreach (var enemy in Player._SPlayerScript._enemies)
        {
            Destroy(enemy);
        }

        _shouldSpawn = true;
    }
}