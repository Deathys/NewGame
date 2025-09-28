using UnityEngine;

/// <summary>
/// Ultra-safe currency UI that works without any package dependencies.
/// This version avoids all potential compilation issues.
/// </summary>
public class SafeCurrencyUI : MonoBehaviour
{
    [Header("Display Settings")]
    [Tooltip("Prefix text before currency amount.")]
    public string currencyPrefix = "Gold: ";

    [Tooltip("Display updates in console.")]
    public bool debugMode = true;

    [Header("Manual UI References")]
    [Tooltip("Manually assign any text component here.")]
    public Component textComponent;

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

        // Try to update manually assigned text component
        if (textComponent != null)
        {
            UpdateTextComponent(textComponent, displayText);
        }
        else
        {
            // Auto-find text components
            FindAndUpdateTextComponents(displayText);
        }

        if (debugMode)
        {
            Debug.Log($"Currency: {displayText}");
        }
    }

    void UpdateTextComponent(Component component, string text)
    {
        if (component == null) return;

        var type = component.GetType();

        // Try common text property names
        var textProperty = type.GetProperty("text");
        if (textProperty != null && textProperty.CanWrite)
        {
            try
            {
                textProperty.SetValue(component, text);
                return;
            }
            catch { }
        }

        // Try common text field names
        var textField = type.GetField("text");
        if (textField != null)
        {
            try
            {
                textField.SetValue(component, text);
                return;
            }
            catch { }
        }
    }

    void FindAndUpdateTextComponents(string text)
    {
        // Search in children for any component with a "text" property
        var allComponents = GetComponentsInChildren<Component>();

        foreach (var component in allComponents)
        {
            if (component == null) continue;

            var typeName = component.GetType().Name;

            // Look for common Unity text component names
            if (typeName.Contains("Text") || typeName.Contains("Label"))
            {
                UpdateTextComponent(component, text);
            }
        }
    }

    // Manual update method for testing
    [ContextMenu("Test Update")]
    void TestUpdate()
    {
        UpdateCurrencyDisplay(999);
    }
}