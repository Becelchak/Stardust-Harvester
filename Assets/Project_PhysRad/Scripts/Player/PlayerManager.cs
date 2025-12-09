using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static  PlayerManager Instance;

    [SerializeField] private int currentScrap;

    public int CurrentScrap => currentScrap;
    public UnityEvent<int> OnScrapChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void AddScrap(int amount)
    {
        currentScrap += amount;
        OnScrapChanged?.Invoke(currentScrap);
    }

    public bool SpendScrap(int amount)
    {
        if (currentScrap >= amount)
        {
            currentScrap -= amount;
            OnScrapChanged?.Invoke(currentScrap);
            return true;
        }
        return false;
    }

    public void SetScrap(int amount)
    {
        currentScrap = amount;
        OnScrapChanged?.Invoke(currentScrap);
    }
}
