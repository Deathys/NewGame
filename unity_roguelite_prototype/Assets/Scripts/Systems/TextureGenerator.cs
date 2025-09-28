using UnityEngine;

/// <summary>
/// Utility class for generating procedural textures and sprites
/// </summary>
public static class TextureGenerator
{
    /// <summary>
    /// Creates a stone/brick platform texture
    /// </summary>
    public static Sprite CreatePlatformTexture(int width = 200, int height = 100)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        // Base stone color
        Color baseColor = new Color(0.6f, 0.6f, 0.65f, 1f);
        Color darkColor = new Color(0.4f, 0.4f, 0.45f, 1f);
        Color lightColor = new Color(0.8f, 0.8f, 0.85f, 1f);

        // Fill with base color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = baseColor;
        }

        // Add stone blocks pattern
        int blockWidth = 40;
        int blockHeight = 20;

        for (int y = 0; y < height; y += blockHeight)
        {
            for (int x = 0; x < width; x += blockWidth)
            {
                // Offset every other row for brick pattern
                int offsetX = (y / blockHeight) % 2 == 0 ? 0 : blockWidth / 2;
                int startX = x + offsetX;

                if (startX >= width) continue;

                // Draw block outline
                DrawBlock(pixels, width, height, startX, y,
                         Mathf.Min(blockWidth, width - startX),
                         Mathf.Min(blockHeight, height - y),
                         darkColor, lightColor);
            }
        }

        // Add some noise/variation
        for (int i = 0; i < pixels.Length; i++)
        {
            float noise = Mathf.PerlinNoise((i % width) * 0.1f, (i / width) * 0.1f);
            Color variation = Color.Lerp(darkColor, lightColor, noise) * 0.2f;
            pixels[i] = Color.Lerp(pixels[i], pixels[i] + variation, 0.3f);
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), TileSystem.PIXELS_PER_UNIT);
    }

    /// <summary>
    /// Creates a grass platform texture
    /// </summary>
    public static Sprite CreateGrassPlatformTexture(int width = 200, int height = 100)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        Color dirtColor = new Color(0.4f, 0.25f, 0.1f, 1f);
        Color grassColor = new Color(0.2f, 0.6f, 0.1f, 1f);
        Color darkGrass = new Color(0.1f, 0.4f, 0.05f, 1f);

        // Fill with dirt
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = dirtColor;
        }

        // Add grass on top
        int grassHeight = height / 4;
        for (int y = height - grassHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (index < pixels.Length)
                {
                    // Grass with some variation
                    float grassNoise = Mathf.PerlinNoise(x * 0.2f, y * 0.2f);
                    Color grass = Color.Lerp(darkGrass, grassColor, grassNoise);
                    pixels[index] = grass;
                }
            }
        }

        // Add some grass blades sticking up
        for (int x = 0; x < width; x += 5)
        {
            if (Random.Range(0f, 1f) > 0.3f)
            {
                int bladeHeight = Random.Range(3, 8);
                for (int i = 0; i < bladeHeight && (height - 1 - i) >= 0; i++)
                {
                    int y = height - 1 - i;
                    int index = y * width + x;
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = darkGrass;
                    }
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), TileSystem.PIXELS_PER_UNIT);
    }

    /// <summary>
    /// Creates a metal platform texture
    /// </summary>
    public static Sprite CreateMetalPlatformTexture(int width = 200, int height = 100)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        Color baseColor = new Color(0.7f, 0.7f, 0.8f, 1f);
        Color darkColor = new Color(0.4f, 0.4f, 0.5f, 1f);
        Color lightColor = new Color(0.9f, 0.9f, 1f, 1f);

        // Fill with base color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = baseColor;
        }

        // Add metal plate pattern
        int plateSize = 25;
        for (int y = 0; y < height; y += plateSize)
        {
            for (int x = 0; x < width; x += plateSize)
            {
                DrawPlate(pixels, width, height, x, y, plateSize, plateSize, darkColor, lightColor);
            }
        }

        // Add rivets
        for (int y = plateSize / 2; y < height; y += plateSize)
        {
            for (int x = plateSize / 2; x < width; x += plateSize)
            {
                DrawRivet(pixels, width, height, x, y, 3, darkColor, lightColor);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), TileSystem.PIXELS_PER_UNIT);
    }

    private static void DrawBlock(Color[] pixels, int texWidth, int texHeight, int x, int y, int width, int height, Color darkColor, Color lightColor)
    {
        // Draw top and left edges (light)
        for (int i = 0; i < width && x + i < texWidth; i++)
        {
            if (y < texHeight)
            {
                int index = y * texWidth + (x + i);
                if (index >= 0 && index < pixels.Length)
                    pixels[index] = lightColor;
            }
        }

        for (int i = 0; i < height && y + i < texHeight; i++)
        {
            if (x < texWidth)
            {
                int index = (y + i) * texWidth + x;
                if (index >= 0 && index < pixels.Length)
                    pixels[index] = lightColor;
            }
        }

        // Draw bottom and right edges (dark)
        for (int i = 0; i < width && x + i < texWidth; i++)
        {
            if (y + height - 1 < texHeight)
            {
                int index = (y + height - 1) * texWidth + (x + i);
                if (index >= 0 && index < pixels.Length)
                    pixels[index] = darkColor;
            }
        }

        for (int i = 0; i < height && y + i < texHeight; i++)
        {
            if (x + width - 1 < texWidth)
            {
                int index = (y + i) * texWidth + (x + width - 1);
                if (index >= 0 && index < pixels.Length)
                    pixels[index] = darkColor;
            }
        }
    }

    private static void DrawPlate(Color[] pixels, int texWidth, int texHeight, int x, int y, int width, int height, Color darkColor, Color lightColor)
    {
        // Similar to DrawBlock but with different styling
        DrawBlock(pixels, texWidth, texHeight, x, y, width, height, darkColor, lightColor);
    }

    private static void DrawRivet(Color[] pixels, int texWidth, int texHeight, int centerX, int centerY, int radius, Color darkColor, Color lightColor)
    {
        for (int y = centerY - radius; y <= centerY + radius; y++)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    if (distance <= radius)
                    {
                        int index = y * texWidth + x;
                        if (index >= 0 && index < pixels.Length)
                        {
                            // Create 3D rivet effect
                            if (distance < radius * 0.7f)
                                pixels[index] = lightColor;
                            else
                                pixels[index] = darkColor;
                        }
                    }
                }
            }
        }
    }
}