using Shooter.Gameplay;
using System;
using System.Resources;
using TMPro.EditorUtilities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerStationControl : MonoBehaviour, IDamageable
{
    [SerializeField] private StationData data;
    [SerializeField] private UIManager UIManager;

    private int currentHealth;
    private int currentScrap;

    public event Action<int> OnHealthChanged;
    public event Action<int, int> OnScrapChanged;
    public event Action OnStationDestroyed;

    public Transform Transform => transform;
    public bool IsAlive => currentHealth > 0;
    public int CurrentHealth { get => currentHealth;} 
    public int MaxHealth => data.maxHealth;
    public int CurrentScrap => currentScrap;

    public event Action <int> OnSpendScrap;

    void Start()
    {
        Initialize();
    }

    public void Initialize(UIManager uiManagerOverride = null)
    {
        currentHealth = data.maxHealth;
        currentScrap = data.startingScrap;

        if (uiManagerOverride != null)
            UIManager = uiManagerOverride;

        OnHealthChanged?.Invoke(currentHealth);
        OnScrapChanged?.Invoke(0, currentScrap);

        Debug.Log($"Station initialized. Health: {currentHealth}, Scrap: {currentScrap}");
    }

    // ========== —»—“≈Ã¿ SCRAP ==========

    public bool TrySpendScrap(int amount)
    {
        if (amount <= 0 || currentScrap < amount)
            return false;

        int oldScrap = currentScrap;
        currentScrap -= amount;
        NotifyScrapChanged(oldScrap, currentScrap);
        return true;
    }

    public void AddScrap(int amount)
    {
        if (amount <= 0) return;

        int oldScrap = currentScrap;
        currentScrap += amount;
        NotifyScrapChanged(oldScrap, currentScrap);
    }

    public bool HasEnoughScrap(int amount) => currentScrap >= amount;

    private void NotifyScrapChanged(int oldValue, int newValue)
    {
        OnScrapChanged?.Invoke(oldValue, newValue);
        UIManager?.RefreshUI();
    }

    // ========== —»—“≈Ã¿ «ƒŒ–Œ¬‹ﬂ ==========

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        UIManager?.RefreshUI();

        if (!IsAlive)
            Die();
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        UIManager?.RefreshUI();
    }

    public void Die()
    {
        Debug.Log("Station destroyed!");
        OnStationDestroyed?.Invoke();

        // ›ÙÙÂÍÚ˚
        if (data.stationDestroyEffect != null)
            Instantiate(data.stationDestroyEffect, transform.position, Quaternion.identity);

        // Game over ÎÓ„ËÍ‡
        GameController.Instance?.GameOver();
    }
}
