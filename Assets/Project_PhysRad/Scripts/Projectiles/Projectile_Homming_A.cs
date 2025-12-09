using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Projectile_Homming_A : MonoBehaviour
    {
        // Start is called before the first frame update
        bool m_Chase = true;
        void Start()
        {
            m_Chase = true;
            Invoke("StopChase", 4);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Chase)
            {
                Vector3 dir = PlayerChar.m_Current.transform.position - transform.position;
                dir.y = 0;
                transform.forward = dir;
            }
        }

        void StopChase()
        {
            m_Chase = false;
        }
    }
}