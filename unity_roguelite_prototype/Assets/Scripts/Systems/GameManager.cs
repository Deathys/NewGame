using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager responsible for handling player death and restarting the
/// current run.  Attach this to an object in your first scene and reference
/// the player's death event from PlayerHealth.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Called when the player dies.  Resets currency and reloads the current scene to restart the run.
    /// </summary>
    public void OnPlayerDeath()
    {
        // Reset currency on death (roguelite mechanic)
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ResetCurrency();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}