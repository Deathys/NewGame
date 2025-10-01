using UnityEngine;

/// <summary>
/// Manages tile-based level creation with consistent sizing
/// </summary>
public static class TileSystem
{
    // Standard tile size in Unity units (1 Unity unit = 1 meter in physics)
    public const float TILE_SIZE = 1f;

    // Pixels per Unity unit (affects sprite resolution)
    public const float PIXELS_PER_UNIT = 32f;

    // Standard tile size in pixels
    public static int TILE_PIXELS => Mathf.RoundToInt(TILE_SIZE * PIXELS_PER_UNIT);

    /// <summary>
    /// Creates a platform made of tiles
    /// </summary>
    public static GameObject CreateTiledPlatform(string name, Vector3 position, int tilesWidth, int tilesHeight, TileType tileType)
    {
        GameObject platform = new GameObject(name);
        position.z = 0f;
        platform.transform.position = position;

        // Calculate actual size in Unity units
        Vector2 platformSize = new Vector2(tilesWidth * TILE_SIZE, tilesHeight * TILE_SIZE);

        // Get appropriate sprite
        Sprite tileSprite = GetTileSprite(tileType, tilesWidth, tilesHeight);

        // Add sprite renderer
        SpriteRenderer spriteRenderer = platform.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = tileSprite;
        spriteRenderer.sortingLayerName = "Background";
        spriteRenderer.sortingOrder = 0; // Background layer

        // Add collider that matches the visual size
        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = platformSize;

        // Set layer
        platform.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;

        Debug.Log($"Created {name}: {tilesWidth}x{tilesHeight} tiles ({platformSize.x}x{platformSize.y} units)");
        return platform;
    }

    /// <summary>
    /// Creates a single tile
    /// </summary>
    public static GameObject CreateSingleTile(Vector3 position, TileType tileType)
    {
        return CreateTiledPlatform($"{tileType} Tile", position, 1, 1, tileType);
    }

    /// <summary>
    /// Gets the appropriate sprite for a tile type and size
    /// </summary>
    private static Sprite GetTileSprite(TileType tileType, int tilesWidth, int tilesHeight)
    {
        int pixelWidth = tilesWidth * TILE_PIXELS;
        int pixelHeight = tilesHeight * TILE_PIXELS;

        switch (tileType)
        {
            case TileType.Grass:
                return TextureGenerator.CreateGrassPlatformTexture(pixelWidth, pixelHeight);
            case TileType.Stone:
                return TextureGenerator.CreatePlatformTexture(pixelWidth, pixelHeight);
            case TileType.Metal:
                return TextureGenerator.CreateMetalPlatformTexture(pixelWidth, pixelHeight);
            default:
                return TextureGenerator.CreatePlatformTexture(pixelWidth, pixelHeight);
        }
    }

    /// <summary>
    /// Converts world position to tile coordinates
    /// </summary>
    public static Vector2Int WorldToTileCoords(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / TILE_SIZE),
            Mathf.FloorToInt(worldPos.y / TILE_SIZE)
        );
    }

    /// <summary>
    /// Converts tile coordinates to world position
    /// </summary>
    public static Vector3 TileToWorldPos(Vector2Int tileCoords)
    {
        return new Vector3(
            tileCoords.x * TILE_SIZE + TILE_SIZE * 0.5f,
            tileCoords.y * TILE_SIZE + TILE_SIZE * 0.5f,
            0f
        );
    }
}

/// <summary>
/// Types of tiles available
/// </summary>
public enum TileType
{
    Grass,
    Stone,
    Metal
}