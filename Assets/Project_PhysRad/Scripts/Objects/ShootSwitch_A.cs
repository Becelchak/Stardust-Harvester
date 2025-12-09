using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class ShootSwitch_A : MonoBehaviour
    {
        [HideInInspector]
        public bool m_Activated = false;
        public Door_A m_Door;

        public GameObject[] m_ActiveBases;
        // Start is called before the first frame update
        void Start()
        {
            m_ActiveBases[0].SetActive(true);
            m_ActiveBases[1].SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            DamageControl damage = GetComponent<DamageControl>();

            if (!m_Activated)
            {
                if (damage.IsDead)
                {
                    m_Activated = true;
                    m_ActiveBases[0].SetActive(false);
                    m_ActiveBases[1].SetActive(true);
                    m_ActiveBases[1].GetComponent<AudioSource>().Play();
                    Invoke("OpenDoor", .5f);
                }
            }
        }

        public void OpenDoor()
        {
            if (m_Door != null)
            {
                m_Door.Open();
            }
        }
    }
}
