using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Stats
    private float _damage;
    private float _moveSpeed;
    #endregion

    #region Target
    private Vector3 _targetPos;
    private Vector3 _targetDir;
    private LayerMask _targetLayerMask;
    private LayerMask _ignoredLayerMask;
    #endregion

    public void Init(float dmg, float moveSpeed, float lifeTime, Vector3 targetPos, LayerMask targetLayerMask, LayerMask ignoredLayerMask)
    {
        _damage = dmg;
        _moveSpeed = moveSpeed;

        _targetPos.Set(targetPos.x, 1.5f, targetPos.z);
        _targetDir = (_targetPos - transform.position).normalized;

        _targetLayerMask = targetLayerMask;
        _ignoredLayerMask = ignoredLayerMask;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += _targetDir * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _ignoredLayerMask) != 0) return;

        if (((1 << other.gameObject.layer) & _targetLayerMask) != 0)
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