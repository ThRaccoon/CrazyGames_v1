using UnityEngine;
using static BarrelData;

public class EnemySpawner : MonoBehaviour
{
    #region Components
    [Header("Components")]
    [Space(5)]
    [SerializeField] private GameObject[] _mobPrefabs;

    [Space(5)]
    [SerializeField] private GameObject[] _bossPrefabs;

    [Space(5)]
    [Header("Barrels")]
    [SerializeField] private BarrelDatabase _barrelDatabase;
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

    [SerializeField] private float _bossSpawnInterval;
    private GlobalTimer _bossSpawnTimer;

    [SerializeField] private float _barrelSpawnInterval;
    private GlobalTimer _barrelSpawnTimer;
    #endregion

    #region Counters
    [Space(15)]
    [Header("Counters")]
    [SerializeField] private float _multipliersIncrementThreshold;
    [SerializeField] private int _spawnedMobCountForMultipliers;

    [Space(5)]
    [SerializeField] private float _mobTypeUnlockThreshold;
    [SerializeField] private float _spawnedMobCountForMobType;

    [Space(5)]
    [SerializeField] private float _bossTypeUnlockThreshold;
    [SerializeField] private float _spawnedBossCountForBossType;
    #endregion

    #region Runtime
    public static bool _shouldSpawn;
    
    public static EnemySpawner _SSpawnerScript;
    #endregion

    #region Debug
    [Space(15)]
    [Header("Debug")]
    [SerializeField] private int _mobTypes;
    [SerializeField] private int _bossTypes;
    #endregion

  
    [SerializeField] private AudioClip _deathSound;

    private void Awake()
    {
        if (_SSpawnerScript == null) 
        {
            _SSpawnerScript = this;
        }
        
        _mobSpawnTimer = new GlobalTimer(0);
        _barrelSpawnTimer = new GlobalTimer(_barrelSpawnInterval);
        _bossSpawnTimer = new GlobalTimer(_bossSpawnInterval);

        _shouldSpawn = true;
    }

    private void Update()
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
            if (_mobTypes < _mobPrefabs.Length - 1)
            {
                _mobTypes++;
            }

            _spawnedMobCountForMobType = 0;
        }

        if (_spawnedBossCountForBossType > _bossTypeUnlockThreshold)
        {
            if (_bossTypes < _bossPrefabs.Length - 1)
            {
                _bossTypes++;
            }

            _spawnedBossCountForBossType = 0;
        }

        _mobSpawnTimer.Tick();

        if (_mobSpawnTimer.Flag)
        {
            SpawnMob();

            _mobSpawnTimer.Reset(_mobSpawnInterval);
        }

        _bossSpawnTimer.Tick();

        if (_bossSpawnTimer.Flag)
        {
            SpawnBoss();

            _bossSpawnTimer.Reset(_bossSpawnInterval);
        }

        _barrelSpawnTimer.Tick();

        if (_barrelSpawnTimer.Flag)
        {
            SpawnBarrel();

            _barrelSpawnTimer.Reset(_barrelSpawnInterval);
        }
    }

    private void SpawnMob()
    {
        GameObject prefab = _mobPrefabs[UnityEngine.Random.Range(0, _mobTypes + 1)];
        Vector3 position = new Vector3(UnityEngine.Random.Range(1.5f, 15.5f), 0, 65);

        var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _expRewardMultiplier);
        }

        _spawnedMobCountForMultipliers++;
        _spawnedMobCountForMobType++;
        _spawnedBossCountForBossType++;
    }

    private void SpawnBoss()
    {
        GameObject prefab;

        if (_bossTypes != _bossPrefabs.Length - 1)
        {
            prefab = _bossPrefabs[_bossTypes];
        }
        else
        {
            prefab = _bossPrefabs[UnityEngine.Random.Range(0, _bossTypes)];
        }

        Vector3 position = new Vector3(UnityEngine.Random.Range(1.5f, 15.5f), 0, 65);
        
        var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _expRewardMultiplier);
        }
    }

    private void SpawnBarrel()
    {
        Vector3 position = _barrelSpawnPositions[UnityEngine.Random.Range(0, _barrelSpawnPositions.Length)];
        BarrelData barrelData = GetBarrelDataByroll(UnityEngine.Random.Range(0, 101));
        if(barrelData != null) 
        {
           var barrel = Instantiate(barrelData.prefab, position, Quaternion.Euler(0, 0, 90));
           var barrelScript = barrel.GetComponent<Barrel>();

           if (barrelScript != null) 
           {
               barrelScript.Init(barrelData, _damageMultiplier);
           }
        }

    }

    private BarrelData GetBarrelDataByroll(int roll) 
    {
       foreach (var barrel in _barrelDatabase.barrels) 
        {
            if(roll >= barrel.rollRangeToSpawnFrom && roll <= barrel.rollRangeToSpawnTo)
            {
                return barrel;
            }
        }

       return null;
    }

    private void ResetSpawner()
    {
        //Reset
        _mobSpawnTimer = new GlobalTimer(0);
        _barrelSpawnTimer = new GlobalTimer(_barrelSpawnInterval);
        _bossSpawnTimer = new GlobalTimer(_bossSpawnInterval);

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