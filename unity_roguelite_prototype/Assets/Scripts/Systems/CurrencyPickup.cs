using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Amount of currency this pickup gives.")]
    public int currencyValue = 1;
    [Tooltip("Speed at which pickup moves toward player.")]
    public float magnetSpeed = 5f;
    [Tooltip("Distance at which pickup starts moving toward player.")]
    public float magnetRange = 3f;

    private Transform player;
    private bool isBeingCollected = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || isBeingCollected) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= magnetRange)
        {
            // Move toward player
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * magnetSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBeingCollected)
        {
            isBeingCollected = true;

            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCurrency(currencyValue);
            }

            // Simple collect effect - destroy immediately
            // In a full game, you'd want to play an animation/sound first
            Destroy(gameObject);
        }
    }
}