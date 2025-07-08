using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _moveSpeed;
    private Vector3 _direction;
    private GameObject _target;


    public void Init(float moveSpeed, float lifeTime, Vector3 targetInitPos, GameObject target)
    {
        _moveSpeed = moveSpeed;
        _target = target;
        _direction = (targetInitPos - transform.position).normalized;

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
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}
