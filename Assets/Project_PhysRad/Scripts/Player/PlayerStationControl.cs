using Shooter.Gameplay;
using System.Resources;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerStationControl : MonoBehaviour, IDamageable
{
    [SerializeField] private StationData data;
    [SerializeField] private Transform weaponPivot;

    private int currentHealth;
    private int currentScrap;
    private float lastAttackTime;

    public Transform Transform => transform;
    public bool IsAlive => currentHealth > 0;
    public int CurrentHealth { get => currentHealth;} 
    public int MaxHealth => data.maxHealth;
    public int AttackDamage => data.baseAttackDamage;
    public float AttackRange => data.baseAttackRange;
    public float AttackRate => data.baseAttackRate;
    public bool CanAttack => Time.time >= lastAttackTime + 1f / AttackRate;

    void Start()
    {
        currentHealth = data.maxHealth;
        currentScrap = data.startingScrap;
        PlayerManager.Instance?.SetScrap(currentScrap);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (!IsAlive)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
    }

    public void Die()
    {
        Debug.Log("Station destroyed! Game Over.");
    }
}
