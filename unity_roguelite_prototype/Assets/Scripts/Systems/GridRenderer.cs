using UnityEngine;

/// <summary>
/// Renders a debug grid in the Scene view using Gizmos.
/// Attach to any GameObject in the scene.
/// </summary>
public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Size of each grid cell")]
    public float cellSize = 1f;

    [Tooltip("Number of cells in X direction")]
    public int gridWidth = 20;

    [Tooltip("Number of cells in Y direction")]
    public int gridHeight = 20;

    [Tooltip("Color of the grid lines")]
    public Color gridColor = new Color(1f, 1f, 1f, 0.3f);

    [Tooltip("Offset from origin")]
    public Vector2 offset = Vector2.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // Vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            float xPos = x * cellSize + offset.x;
            Vector3 start = new Vector3(xPos, offset.y, 0);
            Vector3 end = new Vector3(xPos, gridHeight * cellSize + offset.y, 0);
            Gizmos.DrawLine(start, end);
        }

        // Horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            float yPos = y * cellSize + offset.y;
            Vector3 start = new Vector3(offset.x, yPos, 0);
            Vector3 end = new Vector3(gridWidth * cellSize + offset.x, yPos, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
