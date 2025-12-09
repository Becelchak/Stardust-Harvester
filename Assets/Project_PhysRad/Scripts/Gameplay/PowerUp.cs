using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class PowerUp : MonoBehaviour
    {
        public int m_PowerNum = 0;
        [HideInInspector]
        public bool m_Picked = false;

        public GameObject m_BulletPrefab1;
        public GameObject m_ParticlePrefab1;

        public GameObject m_DisablingPart;
        // Start is called before the first frame update
        void Start()
        {
            m_Picked = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider coll)
        {
            if (!m_Picked)
            {
                if (coll.gameObject.tag == "Player")
                {
                    GameObject obj = Instantiate(m_ParticlePrefab1);
                    obj.transform.position = transform.position;
                    Destroy(obj, 3);

                    m_Picked = true;
                    GetComponent<Collider>().enabled = false;
                    m_DisablingPart.SetActive(false);
                    ApplyPower();
                }
            }

        }

        public void ApplyPower()
        {
            switch(m_PowerNum)
            {
                case 0:
                    StartCoroutine(Co_FireRingBullets());
                    break;
            }
        }

        IEnumerator Co_FireRingBullets()
        {
            for (int i = 0; i < 40; i++)
            {
                GameObject obj1 = Instantiate(m_BulletPrefab1);
                obj1.transform.position = transform.position;
                Vector3 v = Quaternion.Euler(0, i * 12, 0) * Vector3.forward;
                obj1.transform.forward = v;
                obj1.GetComponent<ProjectileCollision>().m_Creator = PlayerChar.m_Current.gameObject;
                yield return new WaitForSeconds(.01f);
            }

            Destroy(gameObject);
        }
    }
}
