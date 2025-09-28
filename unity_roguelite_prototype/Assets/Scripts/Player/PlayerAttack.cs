using UnityEngine;

/// <summary>
/// Simple melee attack controller.  This component listens for an attack input
/// and temporarily enables a collider to detect hits on enemies.  The actual
/// visual and timing of the attack should be handled via animations or
/// animation events.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Tooltip("Damage dealt per hit.")]
    public int damage = 1;
    [Tooltip("Attacks per second.")]
    public float attackRate = 2f;
    [Tooltip("Collider used as the attack hitbox.")]
    public Collider2D attackHitbox;

    [Header("Hit Effects")]
    [Tooltip("Duration of hitstop effect on successful hit.")]
    public float hitStopDuration = 0.1f;
    [Tooltip("Intensity of screen shake on hit.")]
    public float shakeIntensity = 0.3f;
    [Tooltip("Duration of screen shake.")]
    public float shakeDuration = 0.2f;

    private float nextAttackTime;
    private CameraShake cameraShake;

    void Start()
    {
        cameraShake = FindObjectOfType<CameraShake>();
    }

    void Update()
    {
        // Get attack input from InputManager or fallback to legacy input
        bool attackInput = false;

        if (InputManager.Instance != null)
        {
            attackInput = InputManager.Instance.AttackPressed;
        }
        else
        {
            // Fallback to legacy input system
            attackInput = Input.GetButtonDown("Fire1");
        }

        // Listen for attack input and rate limit attacks
        if (Time.time >= nextAttackTime && attackInput)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {
        // Activate the hitbox for a brief moment
        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
            Debug.Log("Attack hitbox ENABLED for attack!");

            // You could play a sound or spawn a VFX here
            // Disable the hitbox after a short delay (instead of end of frame)
            StartCoroutine(DisableHitbox());
        }
        else
        {
            Debug.LogWarning("Attack hitbox is NULL!");
        }
    }

    System.Collections.IEnumerator DisableHitbox()
    {
        // Wait for a few frames instead of just one
        yield return new WaitForSeconds(0.1f);
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
            Debug.Log("Attack hitbox DISABLED after attack!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Attack hitbox triggered by: {other.name}");

        // Check if the collider belongs to an enemy and apply damage
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            Debug.Log($"HIT! Dealing {damage} damage to {enemy.name}");
            enemy.TakeDamage(damage);

            // Apply hit effects
            if (HitStopManager.Instance != null)
            {
                HitStopManager.Instance.HitStop(hitStopDuration);
            }

            if (cameraShake != null)
            {
                cameraShake.Shake(shakeIntensity, shakeDuration);
            }
        }
        else
        {
            Debug.Log($"Hit non-enemy object: {other.name}");
        }
    }
}