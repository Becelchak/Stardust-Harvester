using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Stardust Harvester/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public string enemyName;
    public int maxHealth = 100;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    public float movementSpeed = 3f;

    [Header("Reward")]
    public int scrapReward = 25;

    [Header("Other")]
    public GameObject prefab;
}