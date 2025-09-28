using UnityEngine;

/// <summary>
/// Flying enemy that uses FSM for patrol and dive attack behavior.
/// Patrols horizontally and dives at the player when detected.
/// </summary>
public class Bat : EnemyStateMachine
{
    [Header("Bat Settings")]
    [Tooltip("Movement speed of the bat.")]
    public float moveSpeed = 3f;
    [Tooltip("Half the width of the bat's patrol area.")]
    public float patrolRange = 3f;
    [Tooltip("Speed when diving at player.")]
    public float diveSpeed = 6f;
    [Tooltip("Damage inflicted to the player on contact.")]
    public int contactDamage = 1;

    private float patrolDirection = 1f;
    private Vector3 diveStartPosition;
    private bool isDiving = false;

    protected override void ExecutePatrolBehavior()
    {
        // Horizontal patrol
        transform.Translate(Vector2.right * patrolDirection * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - patrolStartPosition.x) > patrolRange)
        {
            patrolDirection *= -1f;
        }
    }

    protected override void ExecuteAggroBehavior()
    {
        if (!isDiving)
        {
            // Prepare for dive attack
            diveStartPosition = transform.position;
            isDiving = true;
        }

        // Dive toward player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * diveSpeed * Time.deltaTime);
    }

    protected override void ExecuteAttack()
    {
        // Continue diving for a brief moment, then return to patrol height
        if (isDiving)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * diveSpeed * Time.deltaTime);
        }
    }

    protected override void OnStateEnter(EnemyState state)
    {
        base.OnStateEnter(state);

        if (state == EnemyState.Patrol || state == EnemyState.Cooldown)
        {
            isDiving = false;
            // Return to patrol height gradually
            if (transform.position.y != patrolStartPosition.y)
            {
                Vector3 returnPosition = new Vector3(transform.position.x, patrolStartPosition.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, returnPosition, Time.deltaTime * 2f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(contactDamage);
        }
    }
}