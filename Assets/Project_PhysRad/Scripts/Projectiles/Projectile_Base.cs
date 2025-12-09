using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Projectile_Base : MonoBehaviour
    {
        public GameObject HitParticlePrefab1;
        [HideInInspector]
        public GameObject Creator;

        public float Speed = 100;
        public int Damage = 1;
        public float m_Radius = 1f;
        public float m_Range = 10;

        Vector3 startPosition;
        Vector3 targetPosition;

        public void Initialize(Vector3 target, Vector3 start)
        {
            startPosition = start;
            targetPosition = target;

            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        }

        void Update()
        {
            if (Vector3.Distance(startPosition,transform.position)>=m_Range)
            {
                Destroy(gameObject);
                return;
            }

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_Radius, transform.forward, Speed * Time.deltaTime);
            foreach (RaycastHit hit in hits)
            {
                Collider col = hit.collider;

                if (col.gameObject.tag == "Enemy")
                {

                    IDamageable dam = col.gameObject.GetComponent<IDamageable>();
                    if (dam != null)
                    {
                        dam.TakeDamage(Damage);
                    }
                    Destroyed(hit.point);
                }

            }

            transform.position += Speed * Time.deltaTime * transform.forward;
        }

        public virtual void Destroyed(Vector3 pos)
        {
            Destroy(gameObject);
            GameObject obj = Instantiate(HitParticlePrefab1);
            obj.transform.position = pos;
            Destroy(obj, 6);
        }
    }
}