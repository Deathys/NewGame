using UnityEngine;
using System.Collections;

/// <summary>
/// Simple camera shake effect.  For best results attach this script to your
/// camera rig.  Call StartShake() with the desired parameters when you want
/// to trigger a hitstop or impact.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    /// <summary>
    /// Helper method to start the shake coroutine.
    /// </summary>
    /// <param name="duration">How long the shake should last in seconds.</param>
    /// <param name="magnitude">Amplitude of the shake.</param>
    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    /// <summary>
    /// Overloaded method for more intuitive calling from other scripts.
    /// </summary>
    /// <param name="magnitude">Amplitude of the shake.</param>
    /// <param name="duration">How long the shake should last in seconds.</param>
    public void Shake(float magnitude, float duration)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
}