using UnityEngine;
#if UI_AVAILABLE
using UnityEngine.UI;
#endif
#if TMP_PRESENT
using TMPro;
#endif

public class CurrencyUI : MonoBehaviour
{
    [Header("UI References")]
#if TMP_PRESENT
    [Tooltip("Text component to display currency amount.")]
    public TextMeshProUGUI currencyText;
#endif
#if UI_AVAILABLE
    [Tooltip("Alternative UI Text component (fallback or if not using TextMeshPro).")]
    public Text legacyText;
#endif

    [Header("Display Settings")]
    [Tooltip("Prefix text before currency amount.")]
    public string currencyPrefix = "Gold: ";

    void Start()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
            UpdateCurrencyDisplay(CurrencyManager.Instance.GetCurrency());
        }
    }

    void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
        }
    }

    void UpdateCurrencyDisplay(int amount)
    {
        string displayText = currencyPrefix + amount.ToString();

#if TMP_PRESENT
        if (currencyText != null)
        {
            currencyText.text = displayText;
            return;
        }
#endif
#if UI_AVAILABLE
        if (legacyText != null)
        {
            legacyText.text = displayText;
            return;
        }
#endif
        // If no text component is available, log a warning
        Debug.LogWarning("CurrencyUI: No text component assigned or UI packages not available. Please install UI package and assign a text component.");
    }
}