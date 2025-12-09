using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shooter.ScriptableObjects;

namespace Shooter.Gameplay
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl m_Current;

        public bool m_EnteredBossFight = false;
        public GameObject m_LevelBoss;

        public SaveData m_MainSaveData;

        public GameObject m_TextUI_1;


        public GameObject[] m_Tutorials;

        public GameObject m_PauseUI;

        // public GameObject m_DeathEff;

        public bool m_Pausesd = false;
        void Awake()
        {
            m_Current = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Co_Start());
            //m_DeathEff.SetActive(false);
        }

        IEnumerator Co_Start()
        {
            if (m_MainSaveData.m_CheckpointNumber == 0)
            {
                m_TextUI_1.SetActive(true);
                FadeControl.m_Current.StartFadeIn();
                yield return new WaitForSeconds(4f);
                FadeControl.m_Current.StartFadeOut();
                yield return new WaitForSeconds(3f);
                m_TextUI_1.SetActive(false);
                FadeControl.m_Current.StartFadeIn();


                yield return new WaitForSeconds(2f);
                m_Tutorials[0].SetActive(true);
                yield return new WaitForSeconds(4f);
                m_Tutorials[0].SetActive(false);
                m_Tutorials[1].SetActive(true);
                yield return new WaitForSeconds(4f);
                m_Tutorials[1].SetActive(false);
            }
            else
            {
                FadeControl.m_Current.StartFadeIn();
                yield return new WaitForSeconds(1f);

            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!m_Pausesd)
                {
                    m_Pausesd = true;
                    Time.timeScale = 0;
                    m_PauseUI.SetActive(true);
                }
                else
                {
                    m_Pausesd = false;
                    Time.timeScale = 1;
                    m_PauseUI.SetActive(false);
                }
            }
        }

        void FixedUpdate()
        {



        }

        public void HandleCheckpoint(int num)
        {
            if (num > m_MainSaveData.m_CheckpointNumber)
            {
                m_MainSaveData.m_CheckpointNumber = num;
                m_MainSaveData.Save();
            }
        }

        public void HandlePlayerDeath()
        {
            StartCoroutine(Co_HandleGameOver());
        }

        IEnumerator Co_HandleGameOver()
        {
            // m_DeathEff.SetActive(true);
            CameraControl.m_Current.StartShake(.4f, .3f);
            yield return new WaitForSeconds(1);
            FadeControl.m_Current.StartFadeOut();
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        public void Resume()
        {
            m_Pausesd = false;
            Time.timeScale = 1;
            m_PauseUI.SetActive(false);
        }
        public void Exit()
        {
            m_Pausesd = false;
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
