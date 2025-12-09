using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class ProjectileCollision : MonoBehaviour
    {
        public GameObject m_Creator;
        public GameObject m_HitParticle;
        public float m_Damage = 1;
        public bool m_IsEnemyTeam = true;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, .5f);
            foreach (Collider col in hits)
            {
                if (col.gameObject == m_Creator)
                    continue;

                if (col.gameObject.tag == "Player" && m_IsEnemyTeam)
                {

                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(m_Damage, transform.forward, 1);
                    }
                    PlayerChar p = col.gameObject.GetComponent<PlayerChar>();
                    CreateHitParticle();
                    Destroy(gameObject);

                }
                else if (col.gameObject.tag == "Block")
                {
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(m_Damage, transform.forward, 1);
                    }
                    CreateHitParticle();
                    Destroy(gameObject);
                }
                else if (col.gameObject.tag == "Enemy" && !m_IsEnemyTeam)
                {
                    DamageControl d = col.gameObject.GetComponent<DamageControl>();
                    if (d != null)
                    {
                        d.ApplyDamage(m_Damage, transform.forward, 1);
                    }
                    CreateHitParticle();
                    Destroy(gameObject);
                }

            }
        }

        public void CreateHitParticle()
        {
            GameObject obj = Instantiate(m_HitParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 3);
        }
    }
}
