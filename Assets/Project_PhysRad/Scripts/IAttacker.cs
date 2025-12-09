using Shooter.Gameplay;
using UnityEngine;

public interface IAttacker
{
    void Attack(IDamageable target);
    int AttackDamage { get; }
    float AttackRange { get; }
    float AttackRate { get; }
    bool CanAttack { get; }
}
