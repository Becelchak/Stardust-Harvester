using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shooter.UI;
namespace Shooter.Gameplay
{
    public class Level_1 : MonoBehaviour
    {

        public GameObject m_BossObject;
        [HideInInspector]
        public bool m_KilledBoss = false;
        public GameObject m_BossBlock1;
        public GameObject m_BossBlockBack1;

        //public EnemySpawnPoint[] m_BossSpawnPoints;
        void Start()
        {
            m_BossBlockBack1.SetActive(false);
        }

        void Update()
        {
            if (!m_KilledBoss)
            {
                //check boss
                DamageControl bossDamage = m_BossObject.GetComponent<DamageControl>();
                if (bossDamage.IsDead)
                {
                    m_KilledBoss = true;
                    CameraControl.m_Current.m_BossTarget = null;
                    m_BossBlock1.SetActive(false);
                    CameraControl.m_Current.m_BackBlock.gameObject.SetActive(true);
                }
            }

        }

        public void StartBossFight()
        {
            //("Start Boss");
            CameraControl.m_Current.m_BossTarget = m_BossObject.transform;
            m_BossObject.GetComponent<Enemy>().EnableEnemy();
            m_BossBlockBack1.SetActive(true);
            UI_HUD.m_Main.ShowBossHealth();
            CameraControl.m_Current.m_BackBlock.gameObject.SetActive(false);
            StartCoroutine(Co_SpawnSmallEnemies());
        }

        IEnumerator Co_SpawnSmallEnemies()
        {
            int num = 0;
            yield return new WaitForSeconds(6);

            for (int i = 0; i < 10; i++)
            {
                //m_BossSpawnPoints[num].SpawnEnemy();
                if (num == 0)
                    num = 1;
                else
                    num = 0;
                yield return new WaitForSeconds(5);
            }
        }

        public void EndLevel()
        {
            StartCoroutine(Co_EndLevel());
        }

        IEnumerator Co_EndLevel()
        {
            GameControl.m_Current.m_MainSaveData.m_CheckpointNumber = 0;
            GameControl.m_Current.m_MainSaveData.Save();
            yield return new WaitForSeconds(1);
            FadeControl.m_Current.StartFadeOut();
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("EndScene");
        }
    }
}
