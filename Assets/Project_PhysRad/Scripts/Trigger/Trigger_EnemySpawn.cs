using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Trigger_EnemySpawn : MonoBehaviour
    {
        //public EnemySpawnPoint[] m_SpawnPoints;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.tag == "Player")
            {
                //foreach (EnemySpawnPoint sp in m_SpawnPoints)
                //{
                //    sp.SpawnEnemy();
                //}
                GetComponent<Collider>().enabled = false;
            }

        }
    }
}