public interface IDamageable
{
    void TakeDamage(int damage);
    void Heal(int amount);
    void Die();
    bool IsAlive { get; }
    int CurrentHealth { get; }
    int MaxHealth { get; }
}