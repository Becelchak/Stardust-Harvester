using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Barrel : MonoBehaviour
    {
        [HideInInspector]
        public DamageControl m_DamageControl;

        public GameObject m_ExplodeParticle;

        bool exploded = false;
        // Start is called before the first frame update
        void Start()
        {
            m_DamageControl = GetComponent<DamageControl>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!exploded)
            {
                if (m_DamageControl.IsDead)
                {
                    exploded = true;
                    Invoke("Explode", .2f);
                }
            }
        }

        public void Explode()
        {
            GameObject obj = Instantiate(m_ExplodeParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 6);
            Destroy(gameObject);
        }
    }
}