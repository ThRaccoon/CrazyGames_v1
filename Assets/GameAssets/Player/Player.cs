using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _targetLayerMask;

    [Header("Stats")]
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;

    [Header("Animations")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _syncAnimLength;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileMoveSpeed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private float _projectileYOffset;

    private GameObject _projectileTarget;
    private Vector3 _projectileTargetInitPos;
    private Vector3 _projectileSpawnPoint;

    private bool _canAttack = true;
    private bool _shouldSyncAnim = false;

    private GlobalTimer _attackTimer;
    private GlobalTimer _syncAnimTimer;

    public LinkedList<GameObject> enemies;

    public static Player SPlayerScript;

    private void Awake()
    {
        SPlayerScript = this;
        enemies = new LinkedList<GameObject>();

        _projectileSpawnPoint = new Vector3(transform.position.x, transform.position.y + _projectileYOffset, transform.position.z);

        _attackTimer = new GlobalTimer(_attackSpeed);
        _syncAnimTimer = new GlobalTimer(_syncAnimLength);
    }

    private void Update()
    {
        if (_shouldSyncAnim)
        {
            SyncAnimation();
        }

        if (!_canAttack && !_shouldSyncAnim)
        {
            AttackCooldown();
        }

        if (_projectileTarget != null && _canAttack && !_shouldSyncAnim)
        {
            if (AttackTarget())
            {
                _canAttack = false;
            }
        }
        else if (enemies.Count > 0 && enemies.First() != null && _canAttack && !_shouldSyncAnim)
        {
            _projectileTarget = enemies.First();

            if (AttackTarget())
            {
                _canAttack = false;
            }
        }
    }


    public void SelectTarget()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _targetLayerMask))
        {
            _projectileTarget = hit.collider.gameObject;

            UpdateLookAt();

            Debug.Log(_projectileTarget.name);
        }
    }

    private bool AttackTarget()
    {

        float targetDistance = Vector3.Distance(transform.position, _projectileTarget.transform.position);

        if (_projectileTarget != null && _attackRange >= targetDistance)
        {
            UpdateLookAt();

            _projectileTargetInitPos = _projectileTarget.transform.position;

            if (_animator != null)
            {
                _animator.Play("Attack");
            }

            _shouldSyncAnim = true;

            return true;
        }
        return false;
    }

    private void UpdateLookAt()
    {
        if (_projectileTarget != null)
        {
            transform.LookAt(_projectileTarget.transform);
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

    private void SyncAnimation()
    {
        _syncAnimTimer.Tick();

        if (_syncAnimTimer.Flag)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint, Quaternion.identity);
            projectile.GetComponent<Projectile>().Init(_projectileMoveSpeed, _projectileLifeTime, _projectileTargetInitPos, _projectileTarget, _damage);

            _shouldSyncAnim = false;

            _syncAnimTimer.Reset();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + _projectileYOffset, transform.position.z), Vector3.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}