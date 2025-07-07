using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _moveSpeed;
    private GameObject _target = null;
    private float _lifeTime = 4f; 
    private float _timer = 0f;
    private Vector3 _direction;

    public void Init(float speed, GameObject target)
    {
        _moveSpeed = speed;
        _target = target;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        //If don't have a target, projectile continue in straight line until his lifetime
        if (_target)
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
            Destroy(gameObject);
        }
    }
}
