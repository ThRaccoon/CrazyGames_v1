using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Vector3 _targetPos;

    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [Space(5)]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectionDistance;


    [Space(15)]
    [Header("Type")]
    [SerializeField] private bool _isRange;


    private bool _isChasingTarget;
    private float _distanceToTarget;
    private Vector3 _targetPosition;


    private void Awake()
    {
        if (hasPlayer())
        {
            Player.SPlayerScript.enemies.Add(gameObject);
        }

        _targetPosition.Set(_targetPos.x, 0f, _targetPos.z);
    }

    void Update()
    {
        if (_health <= 0)
        {
            Destroy(gameObject);
        }

        _distanceToTarget = Vector3.Distance(transform.position, _targetPos);

        if (!_isChasingTarget)
        {
            if (_distanceToTarget <= _detectionDistance)
            {
                _isChasingTarget = true;
            }
            else
            {
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
            }
        }

        if (_isChasingTarget)
        {
            if (_distanceToTarget > _attackRange)
            {
                Vector3 direction = (_targetPosition - transform.position).normalized;
                transform.position += direction * _moveSpeed * Time.deltaTime;

                transform.LookAt(_targetPosition);
            }
            else
            {
                if (_isRange)
                {

                }
                else
                {

                }
            }
        }
    }

    public void Init(float healthMultiplier, float damageMultiplier)
    {
        _health *= healthMultiplier;
        _damage *= damageMultiplier;
    }

    private bool hasPlayer()
    {
        return (Player.SPlayerScript != null && Player.SPlayerScript.enemies != null);
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
    }

    private void OnDestroy()
    {
        if (hasPlayer())
        {
            Player.SPlayerScript.enemies.Remove(gameObject);
        }
    }


    //===//


}