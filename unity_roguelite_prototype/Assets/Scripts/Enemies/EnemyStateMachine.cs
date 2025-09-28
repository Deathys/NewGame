using UnityEngine;

public enum EnemyState
{
    Patrol,
    Aggro,
    Attack,
    Cooldown,
    Death
}

public abstract class EnemyStateMachine : EnemyBase
{
    [Header("AI Settings")]
    [Tooltip("Detection range for player aggro.")]
    public float detectionRange = 5f;
    [Tooltip("Attack range for melee combat.")]
    public float attackRange = 1.5f;
    [Tooltip("Time to wait after attack before next action.")]
    public float attackCooldown = 1f;

    protected EnemyState currentState = EnemyState.Patrol;
    protected Transform player;
    protected float lastAttackTime;
    protected Vector3 patrolStartPosition;
    protected bool facingRight = true;

    protected override void Awake()
    {
        base.Awake();
        patrolStartPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrolState(distanceToPlayer);
                break;
            case EnemyState.Aggro:
                HandleAggroState(distanceToPlayer);
                break;
            case EnemyState.Attack:
                HandleAttackState(distanceToPlayer);
                break;
            case EnemyState.Cooldown:
                HandleCooldownState(distanceToPlayer);
                break;
        }

        UpdateFacing();
    }

    protected virtual void HandlePatrolState(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Aggro);
            return;
        }

        ExecutePatrolBehavior();
    }

    protected virtual void HandleAggroState(float distanceToPlayer)
    {
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        ExecuteAggroBehavior();
    }

    protected virtual void HandleAttackState(float distanceToPlayer)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            ExecuteAttack();
            lastAttackTime = Time.time;
            ChangeState(EnemyState.Cooldown);
        }
    }

    protected virtual void HandleCooldownState(float distanceToPlayer)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (distanceToPlayer <= detectionRange)
            {
                ChangeState(EnemyState.Aggro);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }
        }
    }

    protected virtual void ChangeState(EnemyState newState)
    {
        OnStateExit(currentState);
        currentState = newState;
        OnStateEnter(newState);
    }

    protected virtual void OnStateEnter(EnemyState state) { }
    protected virtual void OnStateExit(EnemyState state) { }

    protected abstract void ExecutePatrolBehavior();
    protected abstract void ExecuteAggroBehavior();
    protected abstract void ExecuteAttack();

    protected virtual void UpdateFacing()
    {
        if (player == null) return;

        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
            transform.localScale = scale;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}