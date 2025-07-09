using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Player transform to target.")]
    [SerializeField] private Vector3 player;

    [Tooltip("Speed of the enemy movement.")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Distance at which enemy starts chasing the player.")]
    [SerializeField] private float detectionDistance = 5f;

    [Tooltip("Distance at which enemy stops moving (considered reached).")]
    [SerializeField] private float stopDistance = 1.75f;

    private bool chasingPlayer = false;
    private Vector3 targetPosition;
    private float _health;


    public void Init(float health)
    {
        _health = health;
    }

    public void TakeDamage (float damage)
    {
        _health -= damage;
    }


    private void Awake()
    {
        if(hasPlayer())
        {
            Player.SPlayerScript.enemies.AddLast(gameObject);
        }

        targetPosition = player;
        targetPosition.y = 0;
    }

    void Update()
    {
        if(_health<=0)
        {
            Destroy(gameObject);
        }
       
        float distanceToPlayer = Vector3.Distance(transform.position, player);

        if (!chasingPlayer)
        {
            // If player is within detection range, start chasing
            if (distanceToPlayer <= detectionDistance)
            {
                chasingPlayer = true;
            }
            else
            {
                // Move forward in a straight line
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }

        if (chasingPlayer)
        {
            if (distanceToPlayer > stopDistance)
            {
                // Move toward the player's current position
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Optionally rotate enemy to face the player
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
            }
            else
            {
                // Stop moving
                // Optional: add behavior like attacking
            }
        }


    }

    private void OnDestroy()
    {
        if(hasPlayer())
        {
            Player.SPlayerScript.enemies.Remove(gameObject);
        }
    }

    private bool hasPlayer()
    {
        return (Player.SPlayerScript != null && Player.SPlayerScript.enemies != null);
    }
}
