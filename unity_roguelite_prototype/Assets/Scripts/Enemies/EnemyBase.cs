using UnityEngine;

/// <summary>
/// Base class for all enemy behaviours.  Handles hit points and death.  Derived
/// classes should implement their own movement and attack logic in Update or
/// coroutines.
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    [Tooltip("Maximum health points for this enemy.")]
    public int maxHealth = 3;

    [Header("Currency Drop")]
    [Tooltip("Prefab for currency pickup to spawn on death.")]
    public GameObject currencyPickupPrefab;
    [Tooltip("Amount of currency to drop on death.")]
    public int currencyDropAmount = 1;
    [Tooltip("Chance to drop currency (0-1).")]
    [Range(0f, 1f)]
    public float currencyDropChance = 0.8f;

    protected int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage to this enemy.  If health reaches zero the enemy dies.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Destroy the enemy object.  Derived classes can override this to play
    /// animations or particle effects before destroying the GameObject.
    /// </summary>
    protected virtual void Die()
    {
        DropCurrency();
        Destroy(gameObject);
    }

    protected virtual void DropCurrency()
    {
        if (currencyPickupPrefab != null && Random.Range(0f, 1f) <= currencyDropChance)
        {
            GameObject pickup = Instantiate(currencyPickupPrefab, transform.position, Quaternion.identity);
            CurrencyPickup currencyComponent = pickup.GetComponent<CurrencyPickup>();
            if (currencyComponent != null)
            {
                currencyComponent.currencyValue = currencyDropAmount;
            }
        }
    }
}