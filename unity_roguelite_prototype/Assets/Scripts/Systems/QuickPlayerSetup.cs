using UnityEngine;

/// <summary>
/// Utility script to quickly setup a basic player GameObject for testing.
/// Attach this to any GameObject and use the context menu to create a player.
/// </summary>
public class QuickPlayerSetup : MonoBehaviour
{
    [Header("Player Setup")]
    [Tooltip("Color for the default player sprite.")]
    public Color playerColor = Color.blue;

    [Tooltip("Size of the player (width and height).")]
    public Vector2 playerSize = new Vector2(0.8f, 1.8f);

    [ContextMenu("Create Basic Player")]
    public void CreateBasicPlayer()
    {
        // Check if player already exists
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            Debug.LogWarning("Player already exists in scene: " + existingPlayer.name);
            return;
        }

        // Create player GameObject
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0, -1.0f, 0f); // On top of the main ground

        // Add SpriteRenderer with a simple colored sprite
        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSimpleSprite();
        spriteRenderer.color = playerColor;
        spriteRenderer.sortingLayerName = "Characters";
        spriteRenderer.sortingOrder = 10; // Player on top in Characters layer

        // Add Rigidbody2D
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        // Add BoxCollider2D
        BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
        collider.size = playerSize;

        // Add PlayerController
        PlayerController playerController = player.AddComponent<PlayerController>();

        // Create GroundCheck child
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0, -playerSize.y / 2, 0);
        playerController.groundCheck = groundCheck.transform;

        // Add PlayerAttack
        PlayerAttack playerAttack = player.AddComponent<PlayerAttack>();

        // Create AttackHitbox child
        GameObject attackHitbox = new GameObject("AttackHitbox");
        attackHitbox.transform.SetParent(player.transform);
        attackHitbox.transform.localPosition = new Vector3(playerSize.x / 2 + 0.5f, 0, 0);

        BoxCollider2D attackCollider = attackHitbox.AddComponent<BoxCollider2D>();
        attackCollider.isTrigger = true;
        attackCollider.size = new Vector2(1f, playerSize.y);
        attackCollider.enabled = false;

        playerAttack.attackHitbox = attackCollider;

        // Add PlayerHealth with more HP
        PlayerHealth playerHealth = player.AddComponent<PlayerHealth>();
        playerHealth.maxHealth = 10; // Увеличиваем здоровье

        // Add attack visualizer and debugger
        player.AddComponent<AttackVisualizer>();
        player.AddComponent<AttackDebugger>();

        // Add health display to camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.gameObject.AddComponent<HealthDisplay>();
        }

        Debug.Log("Basic Player created successfully at position " + player.transform.position);

        // Select the created player
        #if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = player;
        #endif
    }

    [ContextMenu("Create Basic Camera")]
    public void CreateBasicCamera()
    {
        // Check if main camera exists
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.LogWarning("Main Camera already exists: " + mainCam.name);
            return;
        }

        #if UNITY_EDITOR
        // Create sorting layers in editor
        CreateSortingLayers();
        #endif

        // Create camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";

        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cameraObj.transform.position = new Vector3(0, 0, -10);

        // Add CameraShake component
        cameraObj.AddComponent<CameraShake>();

        Debug.Log("Basic Camera created successfully");

        #if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = cameraObj;
        #endif
    }

    #if UNITY_EDITOR
    private void CreateSortingLayers()
    {
        // Get sorting layer settings
        var tagManagerAssets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (tagManagerAssets.Length > 0)
        {
            var tagManager = new UnityEditor.SerializedObject(tagManagerAssets[0]);
            var sortingLayers = tagManager.FindProperty("m_SortingLayers");

            // Check if our layers already exist
            bool hasBackground = false;
            bool hasCharacters = false;

            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                var layer = sortingLayers.GetArrayElementAtIndex(i);
                var name = layer.FindPropertyRelative("name").stringValue;
                if (name == "Background") hasBackground = true;
                if (name == "Characters") hasCharacters = true;
            }

            // Add missing layers
            if (!hasBackground)
            {
                sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
                var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
                newLayer.FindPropertyRelative("name").stringValue = "Background";
                newLayer.FindPropertyRelative("uniqueID").intValue = 1;
            }

            if (!hasCharacters)
            {
                sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
                var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
                newLayer.FindPropertyRelative("name").stringValue = "Characters";
                newLayer.FindPropertyRelative("uniqueID").intValue = 2;
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("Sorting layers created: Background, Characters");
        }
    }
    #endif

    [ContextMenu("Create Basic Ground")]
    public void CreateBasicGround()
    {
        // Create main ground platform using tile system
        // 20 tiles wide, 2 tiles high
        GameObject ground = TileSystem.CreateTiledPlatform("Main Ground", new Vector3(0, -3f, 0), 20, 2, TileType.Grass);

        Debug.Log("Basic Ground created successfully using tile system");
    }

    private Sprite CreateSimpleSprite()
    {
        // Create a simple colored rectangle sprite
        Texture2D texture = new Texture2D((int)(playerSize.x * 100), (int)(playerSize.y * 100));
        Color[] pixels = new Color[texture.width * texture.height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white; // White base that will be tinted by SpriteRenderer color
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), TileSystem.PIXELS_PER_UNIT);
    }


    [ContextMenu("Create Basic Enemies")]
    public void CreateBasicEnemies()
    {
        // Place goblins on the main ground
        CreateGoblin(new Vector3(5f, -2.3f, 0f));   // On main ground
        CreateGoblin(new Vector3(-5f, -2.3f, 0f)); // On main ground

        // Place bats in the air
        CreateBat(new Vector3(3f, 2f, 0f));
        CreateBat(new Vector3(-3f, 2f, 0f));

        Debug.Log("Basic enemies created!");
    }

    private void CreateGoblin(Vector3 position)
    {
        GameObject goblin = new GameObject("Goblin");
        position.z = 0f;
        goblin.transform.position = position;

        // Add SpriteRenderer with red color
        SpriteRenderer spriteRenderer = goblin.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateEnemySprite(0.8f, 1.4f);
        spriteRenderer.color = Color.red;
        spriteRenderer.sortingLayerName = "Characters";
        spriteRenderer.sortingOrder = 5; // Behind player in Characters layer

        // Add Rigidbody2D
        Rigidbody2D rb = goblin.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        // Add BoxCollider2D
        BoxCollider2D collider = goblin.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 1.4f);

        // Add Goblin AI script with reduced damage
        Goblin goblinScript = goblin.AddComponent<Goblin>();
        goblinScript.contactDamage = 1; // Меньше урона

        // Add debug component and health bar
        goblin.AddComponent<EnemyHealthDebugger>();
        goblin.AddComponent<EnemyHealthBar>();

        Debug.Log($"Goblin created at {position}");
    }

    private void CreateBat(Vector3 position)
    {
        GameObject bat = new GameObject("Bat");
        position.z = 0f;
        bat.transform.position = position;

        // Add SpriteRenderer with purple color
        SpriteRenderer spriteRenderer = bat.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateEnemySprite(0.6f, 0.5f);
        spriteRenderer.color = Color.magenta;
        spriteRenderer.sortingLayerName = "Characters";
        spriteRenderer.sortingOrder = 5; // Behind player in Characters layer

        // Add Rigidbody2D (reduced gravity for flying, but still affected by collisions)
        Rigidbody2D rb = bat.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f; // Light gravity so they can fly but still collide
        rb.freezeRotation = true;
        rb.linearDamping = 2f; // Air resistance to control movement

        // Add BoxCollider2D
        BoxCollider2D collider = bat.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.6f, 0.5f);

        // Add Bat AI script with reduced damage
        Bat batScript = bat.AddComponent<Bat>();
        batScript.contactDamage = 1; // Меньше урона

        // Add debug component and health bar
        bat.AddComponent<EnemyHealthDebugger>();
        bat.AddComponent<EnemyHealthBar>();

        Debug.Log($"Bat created at {position}");
    }

    private Sprite CreateEnemySprite(float width, float height)
    {
        Texture2D texture = new Texture2D((int)(width * 100), (int)(height * 100));
        Color[] pixels = new Color[texture.width * texture.height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), TileSystem.PIXELS_PER_UNIT);
    }

    [ContextMenu("Create Additional Platforms")]
    public void CreateAdditionalPlatforms()
    {
        // Stone platform - 4 tiles wide, 3 tiles high (to match visual size)
        TileSystem.CreateTiledPlatform("Stone Platform", new Vector3(-6f, 0f, 0f), 4, 3, TileType.Stone);

        // Metal platform - 3 tiles wide, 3 tiles high
        TileSystem.CreateTiledPlatform("Metal Platform", new Vector3(6f, 2f, 0f), 3, 3, TileType.Metal);

        // Grass platform - 5 tiles wide, 1 tile high (this one is actually thin)
        TileSystem.CreateTiledPlatform("Grass Platform", new Vector3(0f, 4f, 0f), 5, 1, TileType.Grass);

        // Some single tiles for jumping - make them bigger too
        TileSystem.CreateTiledPlatform("Stone Tile", new Vector3(-3f, 6f, 0f), 1, 3, TileType.Stone);
        TileSystem.CreateTiledPlatform("Metal Tile", new Vector3(3f, 6f, 0f), 1, 3, TileType.Metal);

        Debug.Log("Additional platforms created using tile system!");
    }


    [ContextMenu("Setup Complete Scene")]
    public void SetupCompleteScene()
    {
        CreateBasicCamera();
        CreateBasicGround();
        CreateBasicPlayer();
        CreateBasicEnemies();
        CreateAdditionalPlatforms();

        Debug.Log("Complete basic scene setup finished!");
    }
}