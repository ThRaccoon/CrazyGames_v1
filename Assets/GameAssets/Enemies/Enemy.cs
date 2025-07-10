using Unity.VisualScripting;
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
    [Space(15)]
    [Header("FloatingText")]
    [SerializeField] private GameObject _floatingText;
    [SerializeField] private float _flotingTextYOffset;
    // ----------------------------------------------------------------------------------------------------------------------------------
    [Space(15)]
    [Header("OnDeath")]
    [SerializeField] float _waitBeforeDestroy = 1.5f;
    public bool _isDeadth = false;
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
        if (_isDeadth)
        {
            return;
        }

        if (_health <= 0)
        {
            _isDeadth = true;

            if (hasPlayer())
            {
                Player.SPlayerScript._projectileTarget = null;
                Player.SPlayerScript.enemies.Remove(gameObject);
            }


            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) 
            {
                Destroy(rb);
            }

            if (_animator != null)
            {
                _animator.Play("Death");
            }

            Destroy(gameObject, _waitBeforeDestroy);
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
        DisplayDamage(damage);

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

    private void DisplayDamage(float damage)
    {
        if (_floatingText)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(transform.position.x - 1.25f, transform.position.x + 0.5f), UnityEngine.Random.Range(_flotingTextYOffset - 0.5f, _flotingTextYOffset + 0.5f) ,transform.position.z);           
            var floatingTextObject = Instantiate(_floatingText, spawnPosition, Quaternion.identity, transform);
            if (floatingTextObject)
            {
                var floatingTextMesh = floatingTextObject.GetComponent<TextMesh>();

                if (floatingTextMesh)
                {
                    floatingTextMesh.color = new Color(1f, UnityEngine.Random.Range(0,100)/ 255f, UnityEngine.Random.Range(0, 255) / 255f, 1f);
                    floatingTextMesh.text = damage.ToString();
                }

            }
        }
    }

}