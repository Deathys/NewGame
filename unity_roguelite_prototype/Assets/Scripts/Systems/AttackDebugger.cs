using UnityEngine;

/// <summary>
/// Debug script to visualize attack hitboxes and diagnose combat issues
/// </summary>
public class AttackDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool showAttackRange = true;
    public bool showHitboxes = true;
    public bool logAttackEvents = true;

    private PlayerAttack playerAttack;
    private PlayerController playerController;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerController = GetComponent<PlayerController>();

        if (logAttackEvents)
        {
            Debug.Log("AttackDebugger started. Attack hitbox: " +
                     (playerAttack.attackHitbox != null ? "Found" : "NOT FOUND"));
        }
    }

    void Update()
    {
        // Check for attack input and log it
        bool attacking = false;

        if (InputManager.Instance != null)
        {
            attacking = InputManager.Instance.AttackPressed;
        }
        else
        {
            attacking = Input.GetButtonDown("Fire1");
        }

        if (attacking && logAttackEvents)
        {
            Debug.Log("Attack input detected!");

            if (playerAttack.attackHitbox != null)
            {
                Debug.Log($"Attack hitbox position: {playerAttack.attackHitbox.transform.position}");
                Debug.Log($"Attack hitbox enabled: {playerAttack.attackHitbox.enabled}");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showHitboxes) return;

        // Draw attack range
        if (showAttackRange)
        {
            Gizmos.color = Color.yellow;
            Vector3 attackPos = transform.position + Vector3.right * 1.5f;
            Gizmos.DrawWireCube(attackPos, new Vector3(1f, 2f, 0f));
        }

        // Draw actual hitbox if available
        if (playerAttack != null && playerAttack.attackHitbox != null)
        {
            Gizmos.color = playerAttack.attackHitbox.enabled ? Color.red : Color.gray;

            BoxCollider2D boxCol = playerAttack.attackHitbox as BoxCollider2D;
            if (boxCol != null)
            {
                Vector3 center = boxCol.transform.TransformPoint(boxCol.offset);
                Vector3 size = boxCol.size;
                size.Scale(boxCol.transform.lossyScale);

                Gizmos.DrawWireCube(center, size);
            }
        }
    }

    // Method to manually test attack
    [ContextMenu("Test Attack")]
    public void TestAttack()
    {
        if (playerAttack != null)
        {
            Debug.Log("Testing attack manually...");

            // Force enable hitbox for testing
            if (playerAttack.attackHitbox != null)
            {
                playerAttack.attackHitbox.enabled = true;
                Debug.Log("Hitbox force enabled");

                // Check for enemies in range
                Collider2D[] enemies = Physics2D.OverlapBoxAll(
                    playerAttack.attackHitbox.transform.position,
                    playerAttack.attackHitbox.bounds.size,
                    0f
                );

                Debug.Log($"Found {enemies.Length} colliders in attack range");

                foreach (var enemy in enemies)
                {
                    Debug.Log($"Collider found: {enemy.name} - {enemy.tag}");
                    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
                    if (enemyScript != null)
                    {
                        Debug.Log($"Enemy script found! Dealing damage...");
                        enemyScript.TakeDamage(playerAttack.damage);
                    }
                }

                // Disable hitbox after test
                Invoke("DisableHitboxAfterTest", 0.1f);
            }
        }
    }

    void DisableHitboxAfterTest()
    {
        if (playerAttack.attackHitbox != null)
        {
            playerAttack.attackHitbox.enabled = false;
            Debug.Log("Hitbox disabled after test");
        }
    }
}