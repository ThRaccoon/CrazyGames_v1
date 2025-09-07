using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // General
    [Header("General")]
    [SerializeField] private GameObject _target;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Mobs - General
    [Space(15)]
    [Header("Mobs - General")]
    [SerializeField] private Vector2 _mobsSpawnXRange;
    [SerializeField] private Vector2 _mobsSpawnYRange;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Bosses
    [Space(15)]
    [Header("Bosses")]
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Elite Mobs
    [Space(15)]
    [Header("Elite Mobs")]
    [SerializeField] private GameObject[] _eliteMobPrefabs;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Normal Mobs
    [Space(15)]
    [Header("Normal Mobs")]
    [SerializeField] private GameObject[] _normalMobPrefabs;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Barrels
    [Space(15)]
    [Header("Barrels")]
    [SerializeField] private BarrelDatabase _barrelDatabase;
    [SerializeField] private Vector3[] _barrelSpawnPositions;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Multipliers
    [Space(15)]
    [Header("Multipliers")]
    [SerializeField] private float _baseDamageMultiplier;
    [SerializeField] private float _damageMultiplier;

    [SerializeField] private float _baseHealthMultiplier;
    [SerializeField] private float _healthMultiplier;

    [SerializeField] private float _baseExpRewardMultiplier;
    [SerializeField] private float _expRewardMultiplier;
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Timers
    [Space(15)]
    [Header("Timers")]
    [SerializeField] private float _waveFreezeTimeInterval;
    private GlobalTimer _waveFreezeTimeTimer;

    [Space(5)]
    [SerializeField] private float _mobsSpawnInterval;
    private GlobalTimer _mobsSpawnTimer;

    [Space(5)]
    [SerializeField] private float _barrelSpawnInterval;
    private GlobalTimer _barrelSpawnTimer;
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Game Modes
    [Space(15)]
    [Header("Game Modes")]
    [Header("Endless")]
    [SerializeField] private int _initialNormalMobsCountPerWave;
    [SerializeField] private int _incrementNormalMobsCountPerWaveStep;

    [Space(5)]
    [Header("Progression")]
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Runtime 
    private List<Enemy> _aliveEnemies;

    [SerializeField] private int _waveCount;
    [SerializeField] private int _eliteMobsCountThisWave;
    [SerializeField] private int _spawnedEliteMobsThisWave;
    [SerializeField] private int _normalMobsCountThisWave;
    [SerializeField] private int _spawnedNormalMobsThisWave;
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private enum EWavePhase { None, SpawningNormalMobs, SpawningEliteMobs, WaveFreezeTime }
    private EWavePhase _currentWavePhase;

    private void Start()
    {
        _aliveEnemies = new List<Enemy>();

        _waveFreezeTimeTimer = new GlobalTimer(_waveFreezeTimeInterval);
        _mobsSpawnTimer = new GlobalTimer(_mobsSpawnInterval);
        _barrelSpawnTimer = new GlobalTimer(_barrelSpawnInterval);
    
        // Choose Game Mode:
        _currentWavePhase = EWavePhase.SpawningNormalMobs;

        // Endless
        _normalMobsCountThisWave = _initialNormalMobsCountPerWave;
    }

    private void Update()
    {
        Endless();

        if (_barrelSpawnTimer.Tick())
        {
            SpawnBarrel();
        
            _barrelSpawnTimer.Reset();
        }
    }


    private void SpawnMob(GameObject mob)
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(_mobsSpawnXRange.x, _mobsSpawnXRange.y), 0, UnityEngine.Random.Range(_mobsSpawnYRange.x, _mobsSpawnYRange.y));

        var enemy = Instantiate(mob, position, Quaternion.Euler(0, 180, 0));
        var enemyScript = enemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            enemyScript.Init(_healthMultiplier, _damageMultiplier, _expRewardMultiplier, _target, this);
        }

        _aliveEnemies.Add(enemyScript);
    }

    public void OnMobDeath(Enemy enemy)
    {
        _aliveEnemies.Remove(enemy);
    }

    private void SpawnBarrel()
    {
        Vector3 position = _barrelSpawnPositions[UnityEngine.Random.Range(0, _barrelSpawnPositions.Length)];

        // BarrelData barrelData = GetBarrelDataByroll(UnityEngine.Random.Range(0, 101));
        BarrelData barrelData = _barrelDatabase.barrels[0];

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


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Endless
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void Endless()
    {
        switch (_currentWavePhase)
        {
            case EWavePhase.SpawningNormalMobs:
                {
                    Endless_SpawnMobsPhase();
                }
                break;

            case EWavePhase.SpawningEliteMobs:
                {
                    Endless_SpawnEliteMobsPhase();
                }
                break;
            case EWavePhase.WaveFreezeTime:
                {
                    Endless_WaveFreezeTimePhase();
                }
                break;
        }
    }

    private void Endless_SpawnMobsPhase()
    {
        if (_spawnedNormalMobsThisWave >= _normalMobsCountThisWave)
        {
            _currentWavePhase = EWavePhase.SpawningEliteMobs;

            _mobsSpawnTimer.Reset();
        }

        if (_mobsSpawnTimer.Tick())
        {
            SpawnMob(_normalMobPrefabs[Random.Range(0, _normalMobPrefabs.Length)]);
            _spawnedNormalMobsThisWave++;
            _mobsSpawnTimer.Reset();
        }
    }

    private void Endless_SpawnEliteMobsPhase()
    {
        if (_spawnedEliteMobsThisWave >= _eliteMobsCountThisWave)
        {
            _currentWavePhase = EWavePhase.WaveFreezeTime;

            _mobsSpawnTimer.Reset();
        }

        if (_mobsSpawnTimer.Tick())
        {
            SpawnMob(_eliteMobPrefabs[Random.Range(0, _eliteMobPrefabs.Length)]);
            _spawnedEliteMobsThisWave++;
            _mobsSpawnTimer.Reset();
        }
    }

    private void Endless_WaveFreezeTimePhase()
    {
        if (_waveFreezeTimeTimer.Tick())
        {
            _waveCount++;

            _spawnedNormalMobsThisWave = 0;
            _spawnedEliteMobsThisWave = 0;

            _normalMobsCountThisWave = IncrementMobsCountPerWave();
            _eliteMobsCountThisWave = GetEliteMobsCount();

            _waveFreezeTimeTimer.Reset();

            _currentWavePhase = EWavePhase.SpawningNormalMobs;
        }
    }

    private int IncrementMobsCountPerWave()
    {
        if (_incrementNormalMobsCountPerWaveStep < 1) return _initialNormalMobsCountPerWave;
        
        return _initialNormalMobsCountPerWave + (_waveCount / _incrementNormalMobsCountPerWaveStep);
    }

    private int GetEliteMobsCount()
    {
        if (Random.value >= 0.50f)
        {
            if (Random.value >= 0.35f)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        return 0;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Progression
    ////////////////////////////////////////////////////////////////////////////////////////////////////
}