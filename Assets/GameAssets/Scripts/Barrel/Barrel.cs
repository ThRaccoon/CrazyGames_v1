using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{
    private BarrelData _barrelData;
    private Vector3 _rotationAxis;
    private float _rotationSpeed;
    private float _moveSpeed;
    private float _multiplier;
    private bool _isExploded;
    
    private LayerMask _targetLayersMask;
    private LayerMask _ignoredLayersMask;

    private void Update()
    {
        transform.position += Vector3.back * _moveSpeed * Time.deltaTime;
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    public void Init(BarrelData barrelData, float multiplier)
    {
        _multiplier = multiplier < 1 ? 1 : multiplier;
        _barrelData = barrelData;
        _rotationAxis = _barrelData.RotationAxis;
        _rotationSpeed = _barrelData.RotationSpeed;
        _moveSpeed = _barrelData.MovementSpeed;

        _targetLayersMask = barrelData.TargetLayersMask;
        _ignoredLayersMask = barrelData.IgnoredLayersMask;

        Destroy(gameObject, _barrelData.LifeTime);
    }

    void BarrelDestroy()
    {
        InstantiateVFXOnDestroy();

        switch (_barrelData.Type)
        {
            case BarrelData.EBarrelType.EBuff:
                {
                    GiveBuff();
                }
                break;
            case BarrelData.EBarrelType.EExplosive:
                {
                    if (!_isExploded)
                    {
                        Explode();
                    }
                }
                break;
        }

        Destroy(gameObject);
    }

    public void InstantiateVFXOnDestroy()
    {
        if (_barrelData.ExplosionVFXPrefab != null)
        {
            var vfx = Instantiate(_barrelData.ExplosionVFXPrefab, transform.position, transform.rotation);
            if (vfx != null)
            {
                Destroy(vfx, _barrelData.ExplosionVFXLifeTime);
            }
        }

        AudioManager.Instance.PlaySFXClip(_barrelData.BreakingBarrelSFXClip, _barrelData.BreakingBarrelSFXVolume, transform.position);
    }

    public void TakeDamage(float damage)
    {
        BarrelDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _targetLayersMask) != 0)
        {
            BarrelDestroy();
        }
    }

    private Collider[] _hitColliders;

    private void Explode()
    {
        _isExploded = true;

        _hitColliders = Physics.OverlapSphere(transform.position, _barrelData.ExplosionRange, ~_ignoredLayersMask);

        foreach (Collider hit in _hitColliders)
        {
            if (hit.gameObject == gameObject) continue;

            IDamageable splashTarget = hit.GetComponent<IDamageable>();
            if (splashTarget != null)
            {
                splashTarget.TakeDamage(_barrelData.ExplosionDamage);
            }
        }
    }

    private void GiveBuff()
    {
        BuffCardsManager.Instance.RollBuffs();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _barrelData.ExplosionRange);
    }
}