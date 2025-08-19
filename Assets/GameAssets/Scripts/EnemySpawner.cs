using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ====================================================================================================
    // === General ===
    [Header("General")]
    [SerializeField] private GameObject _target;
    // ====================================================================================================

    // ====================================================================================================
    // === Mobs ===
    [Space(15)]
    [Header("Mobs")]
    [SerializeField] private GameObject[] _mobPrefabs;
    [SerializeField] private Vector2 _mobSpawnXRange;
    // ====================================================================================================

    // ====================================================================================================
    // === Bosses ===
    [Space(15)]
    [Header("Bosses")]
    [SerializeField] private GameObject[] _bossPrefabs;
    [SerializeField] private Vector2 _bossSpawnXRange;
    // ====================================================================================================

    // ====================================================================================================
    // === Barrels ===
    [Space(15)]
    [Header("Barrels")]
    [SerializeField] private BarrelDatabase _barrelDatabase;
    [SerializeField] private Vector3[] _barrelSpawnPositions;
    // ====================================================================================================

    // ====================================================================================================
    // === SFX ===
    [Space(15)]
    [Header("SFX")]
    [SerializeField] private AudioClip _deathSFXClip;
    [SerializeField, Range(0f, 1f)] private float _deathSoundVolume;
    // ====================================================================================================

    // ====================================================================================================
    // === Multipliers ===
    [Space(15)]
    [Header("Multipliers")]
    [SerializeField] private float _baseDamageMultiplier;
    [SerializeField] private float _damageMultiplier;

    [SerializeField] private float _baseHealthMultiplier;
    [SerializeField] private float _healthMultiplier;

    [SerializeField] private float _baseExpRewardMultiplier;
    [SerializeField] private float _expRewardMultiplier;
    // ====================================================================================================

    // ====================================================================================================
    // === Timers ===
    [Space(15)]
    [Header("Timers")]
    [SerializeField] private float _mobSpawnInterval;
    private GlobalTimer _mobSpawnTimer;

    [Space(5)]
    [SerializeField] private float _bossSpawnInterval;
    private GlobalTimer _bossSpawnTimer;

    [Space(5)]
    [SerializeField] private float _barrelSpawnInterval;
    private GlobalTimer _barrelSpawnTimer;
    // ====================================================================================================

    // ====================================================================================================
    // === Counters ===
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
    // ====================================================================================================

    // ====================================================================================================
    // === Runtime ===
    private int _mobTypes;
    private int _bossTypes;

    public bool _shouldSpawn { get; set; }
    // ====================================================================================================

    private void Start()
    {
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
        Vector3 position = new Vector3(UnityEngine.Random.Range(_mobSpawnXRange.x, _mobSpawnXRange.y), 0, 65);

        var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _expRewardMultiplier, _target);
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

        Vector3 position = new Vector3(UnityEngine.Random.Range(_bossSpawnXRange.x, _bossSpawnXRange.y), 0, 65);

        var enemy = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _expRewardMultiplier, _target);
        }
    }

    private void SpawnBarrel()
    {
        Vector3 position = _barrelSpawnPositions[UnityEngine.Random.Range(0, _barrelSpawnPositions.Length)];
        BarrelData barrelData = GetBarrelDataByroll(UnityEngine.Random.Range(0, 101));
        if (barrelData != null)
        {
            var barrel = Instantiate(barrelData.Prefab, position, Quaternion.Euler(0, 0, 90));
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
            if (roll >= barrel.RollChance.x && roll <= barrel.RollChance.y)
            {
                return barrel;
            }
        }

        return null;
    }
}