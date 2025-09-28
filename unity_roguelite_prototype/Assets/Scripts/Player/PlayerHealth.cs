using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the player's hit points and death.  When health reaches zero the
/// attached UnityEvent is invoked.  Hook this event up to your GameManager
/// to restart the run.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public UnityEvent OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void Die()
    {
        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
        // Disable the player rather than destroy so references remain intact
        gameObject.SetActive(false);
    }
}