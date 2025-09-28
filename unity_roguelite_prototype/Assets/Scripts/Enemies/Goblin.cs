using UnityEngine;

/// <summary>
/// Melee enemy that uses FSM for intelligent behavior: patrols, detects player,
/// chases, and attacks with cooldown periods.
/// </summary>
public class Goblin : EnemyStateMachine
{
    [Header("Goblin Settings")]
    [Tooltip("Movement speed of the goblin.")]
    public float moveSpeed = 2f;
    [Tooltip("Patrol distance from starting position.")]
    public float patrolDistance = 3f;
    [Tooltip("Damage inflicted to the player on contact.")]
    public int contactDamage = 1;

    private Vector3 patrolTarget;
    private bool movingToPatrolTarget = true;

    protected override void Awake()
    {
        base.Awake();
        SetNewPatrolTarget();
    }

    protected override void ExecutePatrolBehavior()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f)
        {
            SetNewPatrolTarget();
        }

        Vector2 direction = (patrolTarget - transform.position).normalized;
        transform.Translate(direction * moveSpeed * 0.5f * Time.deltaTime);
    }

    protected override void ExecuteAggroBehavior()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    protected override void ExecuteAttack()
    {
        // Attack by dealing damage if player is still in range
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }

    private void SetNewPatrolTarget()
    {
        float randomX = Random.Range(-patrolDistance, patrolDistance);
        patrolTarget = patrolStartPosition + new Vector3(randomX, 0, 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage the player on contact during aggro/attack states
        if (currentState == EnemyState.Aggro || currentState == EnemyState.Attack)
        {
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(contactDamage);
            }
        }
    }
}