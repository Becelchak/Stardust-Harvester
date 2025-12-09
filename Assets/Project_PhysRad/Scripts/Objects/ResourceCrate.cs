using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class ResourceCrate : MonoBehaviour
    {
        public GameObject[] m_ItemPrefabs;

        [HideInInspector]
        public DamageControl m_DamageControl;

        public GameObject m_ExplodeParticle;

        public GameObject m_LidObject;
        public Transform m_LidTransform;

        bool m_Opened = false;
        // Start is called before the first frame update
        void Start()
        {
            m_DamageControl = GetComponent<DamageControl>();
        }

        // Update is called once per frame
        void Update()
        {
            //if (m_DamageControl.Damage / m_DamageControl.MaxDamage <= .4f)
            //{
            //    m_LidTransform.localRotation = Quaternion.Euler(3 * Mathf.Sin(60 * Time.time), 0, 5 * Mathf.Sin(40 * Time.time));
            //}
            if (!m_Opened)
            {
                if (m_DamageControl.IsDead)
                {
                    TargetsControl.m_Main.RemoveTarget(GetComponent<TargetObject>());
                    GetComponent<Collider>().enabled = false;

                    StartCoroutine(Co_OpenChest());

                    
                    //Destroy(gameObject);
                }
            }
        }

        IEnumerator Co_OpenChest()
        {
            m_Opened = true;
            GameObject obj = Instantiate(m_ExplodeParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 6);
            float lerp = 0;
            while (true)
            {
                m_LidObject.transform.localRotation = Quaternion.Euler(lerp*130,0,  0);
                lerp += 2*Time.deltaTime;
                if (lerp >= 1)
                    break;
                yield return null;
            }

            m_LidObject.transform.localRotation = Quaternion.Euler(130,0,   0);
            StartCoroutine(Co_CreateGems());
            yield return new WaitForSeconds(1.4f);

             obj = Instantiate(m_ExplodeParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 6);

            Destroy(gameObject);
        }
        IEnumerator Co_CreateGems()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject obj1 = Instantiate(m_ItemPrefabs[0]);
                obj1.transform.position = transform.position;
                Vector3 v = Helper.RotatedLenght(i * 36, 10) + new Vector3(0, 12, 0);
                obj1.GetComponent<Rigidbody>().linearVelocity = v;

                yield return new WaitForSeconds(.1f);
                //obj1.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20));
            }
        }
    }
}