using UnityEngine;
#if CINEMACHINE_AVAILABLE
using Cinemachine;
#endif

public class CinemachineSetup : MonoBehaviour
{
#if CINEMACHINE_AVAILABLE
    [Header("Camera References")]
    [Tooltip("Main virtual camera that follows the player.")]
    public CinemachineVirtualCamera playerCamera;
    [Tooltip("Camera shake noise profile for hit effects.")]
    public NoiseSettings shakeProfile;

    [Header("Camera Settings")]
    [Tooltip("Dampening for camera movement.")]
    public float dampening = 1f;
    [Tooltip("Camera dead zone width.")]
    public float deadZoneWidth = 0.5f;
    [Tooltip("Camera dead zone height.")]
    public float deadZoneHeight = 0.5f;

    private Transform player;
    private CinemachineBasicMultiChannelPerlin noiseComponent;
#else
    [Header("Fallback Settings")]
    [Tooltip("Cinemachine package not detected. This script requires Cinemachine to function.")]
    public string cinemachineNotAvailable = "Install Cinemachine package to use this script.";

    private Transform player;
    private Camera mainCamera;
#endif

    void Start()
    {
        SetupCamera();
        FindPlayer();
    }

    void SetupCamera()
    {
#if CINEMACHINE_AVAILABLE
        if (playerCamera == null)
        {
            // Try to find existing camera or create one
            playerCamera = FindObjectOfType<CinemachineVirtualCamera>();

            if (playerCamera == null)
            {
                Debug.LogWarning("No Cinemachine Virtual Camera found. Please add one to the scene.");
                return;
            }
        }

        // Setup camera following
        var transposer = playerCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_XDamping = dampening;
            transposer.m_YDamping = dampening;
        }

        // Setup camera framing
        var framing = playerCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framing != null)
        {
            framing.m_XDamping = dampening;
            framing.m_YDamping = dampening;
            framing.m_DeadZoneWidth = deadZoneWidth;
            framing.m_DeadZoneHeight = deadZoneHeight;
        }

        // Setup camera shake noise
        noiseComponent = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noiseComponent != null && shakeProfile != null)
        {
            noiseComponent.m_NoiseProfile = shakeProfile;
            noiseComponent.m_AmplitudeGain = 0f; // Start with no shake
        }
#else
        // Fallback to main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("No Camera found in scene. Please add a Camera component.");
        }
#endif
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;

#if CINEMACHINE_AVAILABLE
            if (playerCamera != null)
            {
                playerCamera.Follow = player;
                playerCamera.LookAt = player;
            }
#else
            // Simple camera follow for fallback
            if (mainCamera != null && player != null)
            {
                // Basic follow script could be added here
                var followScript = mainCamera.GetComponent<SimpleCameraFollow>();
                if (followScript == null)
                {
                    followScript = mainCamera.gameObject.AddComponent<SimpleCameraFollow>();
                }
                followScript.target = player;
            }
#endif
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
#if CINEMACHINE_AVAILABLE
        if (noiseComponent != null)
        {
            StartCoroutine(ShakeCoroutine(intensity, duration));
        }
#else
        // Fallback shake using transform manipulation
        if (mainCamera != null)
        {
            StartCoroutine(FallbackShakeCoroutine(intensity, duration));
        }
#endif
    }

#if CINEMACHINE_AVAILABLE
    private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        noiseComponent.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        noiseComponent.m_AmplitudeGain = 0f;
    }
#else
    private System.Collections.IEnumerator FallbackShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            mainCamera.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }
#endif

    void OnValidate()
    {
#if CINEMACHINE_AVAILABLE
        // Clamp values to reasonable ranges
        dampening = Mathf.Max(0f, dampening);
        deadZoneWidth = Mathf.Clamp01(deadZoneWidth);
        deadZoneHeight = Mathf.Clamp01(deadZoneHeight);
#endif
    }
}

#if !CINEMACHINE_AVAILABLE
// Simple camera follow script for fallback
public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
#endif