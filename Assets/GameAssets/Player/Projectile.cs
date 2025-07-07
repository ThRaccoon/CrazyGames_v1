using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _moveDir;
    private float _moveSpeed;

    public void Init(Vector3 direction, float speed)
    {
        _moveDir = direction;
        _moveSpeed = speed;
    }


    void Update()
    {
        transform.position += _moveDir * _moveSpeed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
