using Pathfinding;
using Shooter.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    [SerializeField] private int scrapRewardPerWave = 200;

    private PlayerStationControl stationControl;
    private int currentWave = 0;

    [SerializeField] private List<Wave> waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenWaves = 10f;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    public void Initialize(PlayerStationControl station)
    {
        stationControl = station;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaveSpawner());
    }

    IEnumerator WaveSpawner()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            Debug.Log($"Начинается волна {currentWaveIndex + 1}");

            for (int i = 0; i < currentWave.count; i++)
            {
                var rndIndex = Random.Range(0, currentWave.enemyTypePool.Count);
                SpawnEnemy(currentWave.enemyTypePool[rndIndex]);
                yield return new WaitForSeconds(currentWave.spawnInterval);
            }

            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            currentWaveIndex++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("Волны кончились!");
        GameController.Instance?.GameOver(true);
    }
    public void CompleteWave()
    {
        currentWave++;

        if (stationControl != null)
        {
            int reward = scrapRewardPerWave * currentWave;
            stationControl.AddScrap(reward);
            Debug.Log($"Wave {currentWave} complete! +{reward} scrap");
        }

        StartCoroutine(WaveSpawner());
    }

    void SpawnEnemy(EnemyData enemyData)
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var entityObj = GameObject.Find("Entity");
        GameObject enemyObj = Instantiate(enemyData.prefab, spawnPoint.position, spawnPoint.rotation, entityObj.transform);
        var aiDest = enemyObj.GetComponent<AIDestinationSetter>();

        aiDest.target = GameObject.Find("Station").transform;
        Enemy enemy = enemyObj.GetComponent<Enemy>();
    }
}