using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class SpawnControl : MonoBehaviour
    {
        private bool m_DelayingSpawn = true;

        public GameObject[] m_EnemyPrefabs;
        [HideInInspector]
        public GameObject[] m_SpawnPoints;


        [HideInInspector]
        public List<GameObject> m_Enemies;

        [HideInInspector]
        public int SpawnCounter = 0;
        [HideInInspector]
        public int TotalSpawnCount = 0;
        [HideInInspector]
        public int CurrentEnemyCount = 0;
        [HideInInspector]
        public bool KeepSpawnning = true;
        [HideInInspector]
        public int TotalKillCount = 0;

        public static SpawnControl Current;

        int m_SpawnPointNumber = 0;


        void Awake()
        {
            Current = this;
            m_Enemies = new List<GameObject>();

            GameObject[] objs = GameObject.FindGameObjectsWithTag("SpawnPoint");
            m_SpawnPoints = objs;
        }
        // Start is called before the first frame update
        void Start()
        {
            m_DelayingSpawn = false;
            SpawnCounter = 0;
            CurrentEnemyCount = 0;
            KeepSpawnning = true;
            TotalSpawnCount = 20;


        }

        // Update is called once per frame
        void Update()
        {
            if (KeepSpawnning)
            {
                if (CurrentEnemyCount < 100)
                {
                    if (!m_DelayingSpawn)
                    {
                        Transform CurrentSpawnPoint = m_SpawnPoints[m_SpawnPointNumber].transform;
                        if (Vector3.Distance(CurrentSpawnPoint.position, PlayerChar.m_Current.transform.position) <= 30)
                        {
                            m_SpawnPointNumber++;
                        }
                        else
                        {
                            int enemyNumber = 0;

                            GameObject obj = Instantiate(m_EnemyPrefabs[enemyNumber]);
                            Vector3 pos = m_SpawnPoints[m_SpawnPointNumber].transform.position;
                            pos.y = 1;
                            obj.transform.position = pos;
                            AddEnemy();
                            //SpawnCounter++;

                            m_DelayingSpawn = true;

                            if (SpawnCounter >= TotalSpawnCount)
                            {
                                KeepSpawnning = false;
                                //trigger event
                            }
                            else
                            {
                                Invoke("EnableCanSpawnEnemy", .05f);
                            }

                            m_SpawnPointNumber++;
                        }
                        
                        if (m_SpawnPointNumber>m_SpawnPoints.Length-1)
                        {
                            m_SpawnPointNumber = 0;
                        }
                        
                    }
                }
            }

        }

        private void EnableCanSpawnEnemy()
        {
            m_DelayingSpawn = false;
        }

        public void AddEnemy()
        {
            CurrentEnemyCount++;
        }

        public void RemoveEnemy()
        {
            CurrentEnemyCount--;
            TotalKillCount++;
        }
    }
}
