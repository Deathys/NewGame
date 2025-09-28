using UnityEngine;

/// <summary>
/// Simple currency display that works without any UI package dependencies.
/// Uses basic GameObject and string manipulation for display.
/// </summary>
public class SimpleCurrencyDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    [Tooltip("Prefix text before currency amount.")]
    public string currencyPrefix = "Gold: ";

    [Tooltip("Display currency in console if no UI available.")]
    public bool logToConsole = true;

    private int lastDisplayedAmount = -1;

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
        if (lastDisplayedAmount == amount) return;

        lastDisplayedAmount = amount;
        string displayText = currencyPrefix + amount.ToString();

        // Try to find any Text component (legacy or TMP) and update it
        var textComponents = GetComponentsInChildren<Component>();
        bool foundTextComponent = false;

        foreach (var component in textComponents)
        {
            // Check if it's a Text component (using reflection to avoid dependencies)
            var textProperty = component.GetType().GetProperty("text");
            if (textProperty != null && textProperty.PropertyType == typeof(string))
            {
                textProperty.SetValue(component, displayText);
                foundTextComponent = true;
                break;
            }
        }

        // Fallback to console logging
        if (!foundTextComponent && logToConsole)
        {
            Debug.Log($"Currency Update: {displayText}");
        }

        // Also update the GameObject name for easy debugging
        gameObject.name = $"CurrencyDisplay ({amount})";
    }
}