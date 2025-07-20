using UnityEngine;

[CreateAssetMenu(fileName = "BarrelData", menuName = "Scriptable Objects/BarrelData")]
public class BarrelData : ScriptableObject
{
    
    public enum EBarrelType { EBuff, EExplosive }

    [Header("General Barrel Settings")]
    public EBarrelType type;
    public GameObject prefab;
    public float lifeTime;

    [Header("SpawnChance")]
    public int rollRangeToSpawnFrom;
    public int rollRangeToSpawnTo;

    [Header("Movment and Rotation Barrel Settings")]
    public float moveSpeed;
    public Vector3 rotationAxis;
    public float rotationSpeed;

    [Header("General Barrel VFX Settings")]
    public GameObject VFXPrefabOnDestroy;
    public float VFXOnDestroyLifeTime;

    [Header("General Barrel SFX Settings")]
    public AudioClip breakingBarrelClip;
    public float breakingBarrelVolume;

    [Header ("==============Barrel Types Settings==============")]

    [Space(5)]
    [Header("Explosive Barrel Settings")]
    public float ExplosionDamage;
    public float ExplosionRange;

    
}
