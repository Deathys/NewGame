using UnityEngine;

/// <summary>
/// Displays enemy health as a world space health bar above the enemy
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [Tooltip("Offset above the enemy")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    [Tooltip("Width of the health bar")]
    public float barWidth = 1f;
    [Tooltip("Height of the health bar")]
    public float barHeight = 0.2f;
    [Tooltip("Color when health is full")]
    public Color fullHealthColor = Color.green;
    [Tooltip("Color when health is low")]
    public Color lowHealthColor = Color.red;

    private EnemyBase enemyBase;
    private Camera playerCamera;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        if (enemyBase == null)
        {
            Debug.LogError("EnemyHealthBar requires an EnemyBase component!");
            enabled = false;
            return;
        }

        playerCamera = Camera.main;
    }

    void OnGUI()
    {
        if (enemyBase == null || playerCamera == null) return;

        // Convert world position to screen position
        Vector3 worldPos = transform.position + offset;
        Vector3 screenPos = playerCamera.WorldToScreenPoint(worldPos);

        // Don't draw if behind camera or too far
        if (screenPos.z <= 0 || Vector3.Distance(transform.position, playerCamera.transform.position) > 15f)
            return;

        // Convert to GUI coordinates (Y is flipped)
        screenPos.y = Screen.height - screenPos.y;

        // Calculate health percentage
        float healthPercent = (float)enemyBase.currentHealth / enemyBase.maxHealth;

        // Background (black border)
        GUI.backgroundColor = Color.black;
        GUI.Box(new Rect(screenPos.x - barWidth * 50 - 2, screenPos.y - barHeight * 50 - 2,
                        barWidth * 100 + 4, barHeight * 100 + 4), "");

        // Background bar (dark red)
        GUI.backgroundColor = Color.red * 0.3f;
        GUI.Box(new Rect(screenPos.x - barWidth * 50, screenPos.y - barHeight * 50,
                        barWidth * 100, barHeight * 100), "");

        // Health bar
        Color healthColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        GUI.backgroundColor = healthColor;
        GUI.Box(new Rect(screenPos.x - barWidth * 50, screenPos.y - barHeight * 50,
                        barWidth * 100 * healthPercent, barHeight * 100), "");

        // Health text
        GUI.backgroundColor = Color.white;
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 12;
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;

        string healthText = $"{enemyBase.currentHealth}/{enemyBase.maxHealth}";
        GUI.Label(new Rect(screenPos.x - 30, screenPos.y - barHeight * 50 - 20, 60, 15),
                 healthText, textStyle);

        GUI.backgroundColor = Color.white;
    }
}