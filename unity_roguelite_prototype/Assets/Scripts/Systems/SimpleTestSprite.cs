using UnityEngine;

/// <summary>
/// Simple test for sprite rendering issues
/// </summary>
public class SimpleTestSprite : MonoBehaviour
{

    [ContextMenu("Create Simple SpriteRenderer Test")]
    public void CreateSimpleSpriteTest()
    {
        GameObject testObj = new GameObject("SimpleSpriteTest");
        testObj.transform.position = Vector3.zero;

        SpriteRenderer sr = testObj.AddComponent<SpriteRenderer>();

        // Create the simplest possible sprite
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.green);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        sr.sprite = sprite;
        sr.sortingOrder = 9999;

        Debug.Log("Simple sprite test created - basic green pixel");
    }
}