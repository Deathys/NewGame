using UnityEngine;

/// <summary>
/// Adds visual feedback for player attacks
/// </summary>
public class AttackVisualizer : MonoBehaviour
{
    [Header("Attack Visual Settings")]
    public Color attackColor = Color.yellow;
    public float flashDuration = 0.1f;

    private PlayerAttack playerAttack;
    private SpriteRenderer weaponRenderer;
    private GameObject attackEffect;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();

        // Create a simple weapon visual
        CreateWeaponSprite();

        // Create attack effect
        CreateAttackEffect();
    }

    void CreateWeaponSprite()
    {
        GameObject weapon = new GameObject("Weapon");
        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = new Vector3(0.5f, 0, 0);

        weaponRenderer = weapon.AddComponent<SpriteRenderer>();
        weaponRenderer.sprite = GenerateWeaponSprite();
        weaponRenderer.color = Color.gray;
        weaponRenderer.sortingOrder = 1;
    }

    void CreateAttackEffect()
    {
        attackEffect = new GameObject("AttackEffect");
        attackEffect.transform.SetParent(transform);
        attackEffect.transform.localPosition = new Vector3(1f, 0, 0);

        SpriteRenderer effectRenderer = attackEffect.AddComponent<SpriteRenderer>();
        effectRenderer.sprite = GenerateAttackEffectSprite();
        effectRenderer.color = attackColor;
        effectRenderer.sortingOrder = 2;

        attackEffect.SetActive(false);
    }

    void Update()
    {
        // Check for attack input and show visual feedback
        bool attacking = false;

        if (InputManager.Instance != null)
        {
            attacking = InputManager.Instance.AttackPressed;
        }
        else
        {
            attacking = Input.GetButtonDown("Fire1");
        }

        if (attacking)
        {
            ShowAttackEffect();
        }
    }

    void ShowAttackEffect()
    {
        if (attackEffect != null)
        {
            attackEffect.SetActive(true);
            Invoke("HideAttackEffect", flashDuration);
        }

        // Flash weapon
        if (weaponRenderer != null)
        {
            weaponRenderer.color = attackColor;
            Invoke("ResetWeaponColor", flashDuration);
        }
    }

    void HideAttackEffect()
    {
        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
        }
    }

    void ResetWeaponColor()
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.color = Color.gray;
        }
    }

    Sprite GenerateWeaponSprite()
    {
        Texture2D texture = new Texture2D(20, 60);
        Color[] pixels = new Color[texture.width * texture.height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.1f), 100f);
    }

    Sprite GenerateAttackEffectSprite()
    {
        Texture2D texture = new Texture2D(40, 40);
        Color[] pixels = new Color[texture.width * texture.height];

        // Create a simple slash effect
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (Mathf.Abs(x - y) < 5)
                {
                    pixels[y * texture.width + x] = Color.white;
                }
                else
                {
                    pixels[y * texture.width + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }
}