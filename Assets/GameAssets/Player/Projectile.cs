using UnityEngine;

public class Projectile : MonoBehaviour
{
    // --- Stats ---
    private float _dmg;
    private float _moveSpeed;

    // --- Target ---
    Vector3 _targetPos;
    private Vector3 _direction;
    private LayerMask _targetLayerMask;
    private LayerMask _ignoredLayerMask;


    public void Init(float dmg, float moveSpeed, float lifeTime, Vector3 targetPos, LayerMask targetLayerMask, LayerMask ignoredLayerMask, Transform projectileParent)
    {
        _dmg = dmg;
        _moveSpeed = moveSpeed;

        _targetPos.Set(targetPos.x, 1.5f, targetPos.z);
        _direction = (_targetPos - transform.position).normalized;

        _targetLayerMask = targetLayerMask;
        _ignoredLayerMask = ignoredLayerMask;

        gameObject.transform.SetParent(projectileParent);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += _direction * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _ignoredLayerMask) != 0) return;

        if (((1 << other.gameObject.layer) & _targetLayerMask) != 0)
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(_dmg);
            }
        }

        Destroy(gameObject);
    }
}