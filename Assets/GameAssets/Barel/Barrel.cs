using UnityEngine;

public class Barrel : MonoBehaviour, IDamageable
{
    [SerializeField] Vector3 _rotationAxis;
    [SerializeField] float _rotationSpeed;

    [Space(15)]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _lifeTime;


    private void Update()
    {
        transform.position += Vector3.back * _moveSpeed * Time.deltaTime;
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    public void Init(float health, float moveSpeed, float lifeTime)
    {
        _moveSpeed = moveSpeed;
        _lifeTime = lifeTime;

        Destroy(gameObject, _lifeTime);
    }

    public void TakeDamage(float damage)
    {
        GiveBuff();

        Destroy(gameObject);
    }

    public void GiveBuff()
    {
        // Play Audio

        if (BuffCardManager._SBuffCardManagerScript)
        {
            BuffCardManager._SBuffCardManagerScript.RollBuff();
        }

        GameManager._SGameManager.PauseGame();
    }
}