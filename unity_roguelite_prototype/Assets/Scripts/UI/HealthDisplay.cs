using UnityEngine;

/// <summary>
/// Simple health display that shows HP in the top-left corner
/// </summary>
public class HealthDisplay : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private GUIStyle healthStyle;

    void Start()
    {
        // Find player health component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // Setup GUI style
        healthStyle = new GUIStyle();
        healthStyle.fontSize = 24;
        healthStyle.normal.textColor = Color.white;
        healthStyle.fontStyle = FontStyle.Bold;
    }

    void OnGUI()
    {
        if (playerHealth != null)
        {
            string healthText = $"HP: {playerHealth.currentHealth}/{playerHealth.maxHealth}";
            GUI.Label(new Rect(10, 10, 200, 30), healthText, healthStyle);

            // Health bar
            GUI.backgroundColor = Color.red;
            GUI.Box(new Rect(10, 40, 200, 20), "");

            GUI.backgroundColor = Color.green;
            float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
            GUI.Box(new Rect(10, 40, 200 * healthPercent, 20), "");

            GUI.backgroundColor = Color.white;
        }
    }
}