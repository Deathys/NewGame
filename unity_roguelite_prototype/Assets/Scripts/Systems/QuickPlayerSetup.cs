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
    public Vector2 playerSize = new Vector2(1f, 2f);

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
        player.transform.position = Vector3.zero;

        // Add SpriteRenderer with a simple colored sprite
        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSimpleSprite();
        spriteRenderer.color = playerColor;

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

    [ContextMenu("Create Basic Ground")]
    public void CreateBasicGround()
    {
        GameObject ground = new GameObject("Ground");
        ground.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;

        // Create a simple ground sprite
        SpriteRenderer groundSprite = ground.AddComponent<SpriteRenderer>();
        groundSprite.sprite = CreateGroundSprite();
        groundSprite.color = Color.green;

        // Add collider
        BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(20f, 1f);

        // Position ground below player
        ground.transform.position = new Vector3(0, -3f, 0);

        Debug.Log("Basic Ground created successfully");
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

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite CreateGroundSprite()
    {
        Texture2D texture = new Texture2D(2000, 100);
        Color[] pixels = new Color[texture.width * texture.height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    [ContextMenu("Create Basic Enemies")]
    public void CreateBasicEnemies()
    {
        CreateGoblin(new Vector3(5f, 0f, 0f));
        CreateGoblin(new Vector3(-5f, 0f, 0f));
        CreateBat(new Vector3(3f, 3f, 0f));
        CreateBat(new Vector3(-3f, 3f, 0f));

        Debug.Log("Basic enemies created!");
    }

    private void CreateGoblin(Vector3 position)
    {
        GameObject goblin = new GameObject("Goblin");
        goblin.transform.position = position;

        // Add SpriteRenderer with red color
        SpriteRenderer spriteRenderer = goblin.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateEnemySprite(1f, 1.5f);
        spriteRenderer.color = Color.red;

        // Add Rigidbody2D
        Rigidbody2D rb = goblin.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        // Add BoxCollider2D
        BoxCollider2D collider = goblin.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 1.5f);

        // Add Goblin AI script with reduced damage
        Goblin goblinScript = goblin.AddComponent<Goblin>();
        goblinScript.contactDamage = 1; // Меньше урона

        // Add debug component
        goblin.AddComponent<EnemyHealthDebugger>();

        Debug.Log($"Goblin created at {position}");
    }

    private void CreateBat(Vector3 position)
    {
        GameObject bat = new GameObject("Bat");
        bat.transform.position = position;

        // Add SpriteRenderer with purple color
        SpriteRenderer spriteRenderer = bat.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateEnemySprite(0.8f, 0.6f);
        spriteRenderer.color = Color.magenta;

        // Add Rigidbody2D (no gravity for flying)
        Rigidbody2D rb = bat.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Add BoxCollider2D
        BoxCollider2D collider = bat.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 0.6f);

        // Add Bat AI script with reduced damage
        Bat batScript = bat.AddComponent<Bat>();
        batScript.contactDamage = 1; // Меньше урона

        // Add debug component
        bat.AddComponent<EnemyHealthDebugger>();

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

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    [ContextMenu("Setup Complete Scene")]
    public void SetupCompleteScene()
    {
        CreateBasicCamera();
        CreateBasicGround();
        CreateBasicPlayer();
        CreateBasicEnemies();

        Debug.Log("Complete basic scene setup finished!");
    }
}