using UnityEngine;

public class Projectile : MonoBehaviour
{
    // ====================================================================================================
    // === General ===
    private LayerMask _targetLayersMask;
    private LayerMask _ignoredLayersMask;
    // ====================================================================================================

    // ====================================================================================================
    // === Stats ===
    private float _damage;
    private float _moveSpeed;
    // ====================================================================================================

    // ====================================================================================================
    // === Target ===
    private Vector3 _targetPos;
    private Vector3 _targetDir;
    // ====================================================================================================

    void Update()
    {
        transform.position += _targetDir * _moveSpeed * Time.deltaTime;
    }

    public void Init(float dmg, float moveSpeed, float lifeTime, Vector3 targetPos, LayerMask targetLayerMask, LayerMask ignoredLayerMask)
    {
        _damage = dmg;
        _moveSpeed = moveSpeed;

        _targetPos.Set(targetPos.x, 1.5f, targetPos.z);
        _targetDir = (_targetPos - transform.position).normalized;

        _targetLayersMask = targetLayerMask;
        _ignoredLayersMask = ignoredLayerMask;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _ignoredLayersMask) != 0) return;

        if (((1 << other.gameObject.layer) & _targetLayersMask) != 0)
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(_damage);
            }
        }

        Destroy(gameObject);
    }
}