using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _x;
    private float _moveSpeed;
    private Vector3 _direction;
    private GameObject _target;
    private float _damage;


    public void Init(float moveSpeed, float lifeTime, Vector3 targetInitPos, GameObject target, float damage)
    {
        _moveSpeed = moveSpeed;
        _target = target;
        _direction = (targetInitPos - transform.position).normalized;
        transform.eulerAngles = new Vector3(_x, transform.eulerAngles.y, transform.eulerAngles.z);
        _damage = damage;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (_target != null)
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemyScript = other.gameObject.GetComponent<Enemy>();
            if (enemyScript != null) 
            {
                enemyScript.TakeDamage(_damage);
            }
        }

        Destroy(gameObject);
    }
}
