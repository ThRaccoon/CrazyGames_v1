using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _targetLayerMask;

    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;


    private GameObject _target;

    private bool _canAttack = true;

    [SerializeField] private float _spawnProjectileInterval;
    private float _countDown;

    private void Awake()
    {
        _countDown = _spawnProjectileInterval;
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
            _countDown -= Time.deltaTime;
            
            if (_countDown <= 0) 
            {
                _countDown = _spawnProjectileInterval;
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
        if (_target != null)
        {
            Vector3 dir = (_target.transform.position - transform.position).normalized;

            GameObject projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Init(dir, _projectileSpeed);
        }
    }
}