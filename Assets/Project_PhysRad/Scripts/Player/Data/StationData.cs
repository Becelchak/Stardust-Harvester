using UnityEngine;

[CreateAssetMenu(fileName = "NewStationData", menuName = "Stardust Harvester/Station Data")]
public class StationData : ScriptableObject
{
    [Header("Core Stats")]
    public int maxHealth = 1000;

    [Header("Defense")]
    public int baseAttackDamage = 20;
    public float baseAttackRange = 10f;
    public float baseAttackRate = 0.8f;

    [Header("Economy")]
    public int startingScrap = 200;
}