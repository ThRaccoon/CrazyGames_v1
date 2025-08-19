using UnityEngine;

[CreateAssetMenu(fileName = "BarrelData", menuName = "Scriptable Objects/BarrelData")]
public class BarrelData : ScriptableObject
{
    public enum EBarrelType { None, EBuff, EExplosive }

    // ====================================================================================================
    // === General ===
    [field: Header("General")]
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: SerializeField] public LayerMask TargetLayersMask { get; private set; }
    [field: SerializeField] public LayerMask IgnoredLayersMask { get; private set; }
    // ====================================================================================================

    // ====================================================================================================
    // === Animations ===
    [field: Space(15)]
    [field: Header("Animations")]
    [field: SerializeField] public Vector3 RotationAxis { get; private set; }
    // ====================================================================================================

    // === VFX ===
    [field: Space(15)]
    [field: Header("VFX")]
    [field: SerializeField] public GameObject ExplosionVFXPrefab { get; private set; }
    [field: SerializeField] public float ExplosionVFXLifeTime { get; private set; }
    // ====================================================================================================

    // ====================================================================================================
    // === SFX ===
    [field: Space(15)]
    [field: Header("SFX")]
    [field: SerializeField] public AudioClip BreakingBarrelSFXClip { get; private set; }
    [field: SerializeField] public float BreakingBarrelSFXVolume { get; private set; }
    // ====================================================================================================

    // ====================================================================================================
    // === Stats ===
    [field: Space(15)]
    [field: Header("Stats")]
    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField] public float LifeTime { get; private set; }

    [field: Space(5)]
    [field: Header("Spawn Chance")]
    [field: SerializeField] public Vector2 RollChance { get; private set; }

    [field: Space(5)]
    [field: Header("Type")]
    [field: SerializeField] public EBarrelType Type { get; private set; }
    // ====================================================================================================

    // ====================================================================================================
    // === Explosion ===
    [field: Space(15)]
    [field: Header("Explosion (Only If Explosive)")]
    [field: SerializeField] public float ExplosionDamage { get; private set; }
    [field: SerializeField] public float ExplosionRange { get; private set; }
    // ====================================================================================================
}