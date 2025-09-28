using UnityEngine;

/// <summary>
/// Debug script to monitor enemy health and damage
/// </summary>
public class EnemyHealthDebugger : MonoBehaviour
{
    private EnemyBase enemyBase;
    private int lastHealth;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            lastHealth = enemyBase.maxHealth;
            Debug.Log($"{gameObject.name} started with {enemyBase.maxHealth} HP");
        }
    }

    void Update()
    {
        if (enemyBase != null)
        {
            // Check if health changed
            int currentHealth = GetCurrentHealth();
            if (currentHealth != lastHealth)
            {
                Debug.Log($"{gameObject.name} health changed: {lastHealth} -> {currentHealth}");
                lastHealth = currentHealth;
            }
        }
    }

    int GetCurrentHealth()
    {
        // Use reflection to access protected currentHealth field
        var field = typeof(EnemyBase).GetField("currentHealth",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return (int)field.GetValue(enemyBase);
        }

        return enemyBase.maxHealth; // fallback
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} triggered by: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered enemy trigger!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"{gameObject.name} collided with: {collision.gameObject.name}");
    }
}