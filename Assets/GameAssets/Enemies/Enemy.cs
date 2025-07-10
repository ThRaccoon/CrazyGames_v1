using UnityEngine;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    // ----------------------------------------------------------------------------------------------------------------------------------
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private VisualEffect _visualEffect;
    // ----------------------------------------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------------------------------------
    [Space(15)]
    [Header("Stats")]
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [Space(5)]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectionDistance;
    // ----------------------------------------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------------------------------------
    [Space(15)]
    [Header("Animations")]
    [SerializeField] private float _syncedAttackAnimLength;
    // ----------------------------------------------------------------------------------------------------------------------------------

    // --- Timers ---
    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;

    private bool _canAttack = true;
    private bool _shouldSyncAttackAnim;
    private bool _isChasingTarget;
    private float _distanceToTarget;
    private Vector3 _targetPos;


    private void Awake()
    {
        if (hasPlayer())
        {
            Player.SPlayerScript.enemies.Add(gameObject);

            _targetPos.Set(Player.SPlayerScript.transform.position.x, 0f, Player.SPlayerScript.transform.position.z);
        }

        _attackTimer = new GlobalTimer(_attackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncedAttackAnimLength);
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
                Vector3 direction = (_targetPos - transform.position).normalized;
                transform.position += direction * _moveSpeed * Time.deltaTime;

                transform.LookAt(_targetPos);
            }
            else
            {
                if (_shouldSyncAttackAnim)
                {
                    SyncAttackAnimation();
                }

                if (!_canAttack && !_shouldSyncAttackAnim)
                {
                    AttackCooldown();
                }

                if (_canAttack && !_shouldSyncAttackAnim)
                {
                    AttackTarget();
                }
            }
        }
    }

    private bool hasPlayer()
    {
        return (Player.SPlayerScript != null && Player.SPlayerScript.enemies != null);
    }


    public void Init(float healthMultiplier, float damageMultiplier)
    {
        _health *= healthMultiplier;
        _damage *= damageMultiplier;
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

    private void AttackTarget()
    {
        if (_animator != null)
        {
            _animator.Play("Attack");
        }

        if (_visualEffect != null) 
        {
            _visualEffect.Play();
        }

        _canAttack = false;
        _shouldSyncAttackAnim = true;
    }

    private void SyncAttackAnimation()
    {
        _syncAnimTimer.Tick();

        if (_syncAnimTimer.Flag)
        {
            _shouldSyncAttackAnim = false;

            if (hasPlayer())
            {
                Player.SPlayerScript.Health -= _damage;
            }

            if (_visualEffect != null)
            {
                _visualEffect.Stop();
            }

            _syncAnimTimer.Reset();
        }
    }

    private void AttackCooldown()
    {
        _attackTimer.Tick();

        if (_attackTimer.Flag)
        {
            _canAttack = true;

            _attackTimer.Reset();
        }
    }
}