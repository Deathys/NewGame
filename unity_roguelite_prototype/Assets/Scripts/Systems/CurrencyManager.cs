using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Currency Settings")]
    [Tooltip("Current amount of currency collected this run.")]
    public int currentCurrency = 0;

    public delegate void CurrencyChangedHandler(int newAmount);
    public event CurrencyChangedHandler OnCurrencyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }

    public bool SpendCurrency(int amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            OnCurrencyChanged?.Invoke(currentCurrency);
            return true;
        }
        return false;
    }

    public void ResetCurrency()
    {
        currentCurrency = 0;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }

    public int GetCurrency()
    {
        return currentCurrency;
    }
}