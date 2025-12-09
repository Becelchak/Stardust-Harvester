using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public List<EnemyData> enemyTypePool;
    public int count;
    public float spawnInterval = 1f;
    public float delayBeforeWave = 2f;
}