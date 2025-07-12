using UnityEngine;

public class Projectile : MonoBehaviour
{
    // --- Stats ---
    private float _dmg;
    private float _moveSpeed;

    // --- Target ---
    private GameObject _target;
    private Vector3 _direction;
    private LayerMask _targetLayerMask;
    private LayerMask _ignoredLayerMask;

    // --- Sender ---
    private bool _isPlayer;

    // For Player
    public void Init(float dmg, float moveSpeed, float lifeTime, Vector3 targetInitPos, GameObject target, LayerMask targetLayerMask, LayerMask ignoredLayerMask, bool isPlayer, Transform projectileParent)
    {
        _dmg = dmg;
        _moveSpeed = moveSpeed;

        _direction = (targetInitPos - transform.position).normalized;
        _target = target;
        _targetLayerMask = targetLayerMask;

        _isPlayer = isPlayer;

        gameObject.transform.SetParent(projectileParent);

        Destroy(gameObject, lifeTime);
    }

    // For Enemy
    public void Init(float dmg, float moveSpeed, float lifeTime, Vector3 targetPos, LayerMask targetLayerMask, LayerMask ignoredLayerMask, bool isPlayer, Transform projectileParent)
    {
        _dmg = dmg;
        _moveSpeed = moveSpeed;

        _direction = (new Vector3(targetPos.x, 1.5f, targetPos.z) - transform.position).normalized;
        _targetLayerMask = targetLayerMask;

        _isPlayer = isPlayer;

        gameObject.transform.SetParent(projectileParent);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (_target != null && _target.GetComponent<Enemy>()._isDeadth == false)
        {
            Vector3 targetPos = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
            _direction = (targetPos - transform.position).normalized;
            transform.position += _direction * _moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += _direction * _moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int otherLayer = other.gameObject.layer;

        if (((1 << otherLayer) & _ignoredLayerMask.value) != 0)
            return;

        if (((1 << other.gameObject.layer) & _targetLayerMask.value) != 0)
        {
            if (_isPlayer)
            {
                var enemyScript = other.gameObject.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(_dmg);
                }

                Destroy(gameObject);
            }
            else
            {
                var playerScript = other.gameObject.GetComponent<Player>();

                if (playerScript != null)
                {
                    playerScript.TakeDamage(_dmg);
                }
                
                Destroy(gameObject);
            }
        }
    }
}