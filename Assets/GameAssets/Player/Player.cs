using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _targetLayerMask;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;

    private Vector3 _spawnPoint;
    private GameObject _target;

    private bool _canAttack = true;

    [SerializeField] private float _spawnProjectileInterval;
    [SerializeField] float _attackSpeed = 1f;
    [SerializeField] float _attackRange = 20f;

    private void Awake()
    {
        _spawnPoint = new Vector3(transform.position.x,transform.position.y+1.5f,transform.position.z);
        _attackSpeed = _spawnProjectileInterval;
    }

    private void Update()
    {
        if (_canAttack && _target != null) 
        {
            AttackTarget();
        
            _canAttack = false;
        }
        

        if (!_canAttack) 
        {
            _attackSpeed -= Time.deltaTime;
            
            if (_attackSpeed <= 0) 
            {
                _attackSpeed = _spawnProjectileInterval;
                _canAttack = true;
            }
        }
    }


    public void SelectTarget()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _targetLayerMask))
        {
            _target = hit.collider.gameObject;

            Debug.Log(_target.name);
        }
    }

    private void AttackTarget()
    {
        float targetDistance = Vector3.Distance(transform.position, _target.transform.position);
        if (_target != null && _attackRange >= targetDistance)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _spawnPoint, Quaternion.identity);
            projectile.GetComponent<Projectile>().Init(_projectileSpeed,_target);
        }
    }
}