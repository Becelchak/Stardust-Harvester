using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class ProjectileMovement : MonoBehaviour
    {
        public float m_Speed = 50;
        public float m_TurnSpeed = 0;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Co_InitGrow());
        }

        // Update is called once per frame
        void Update()
        {
            if (m_TurnSpeed!=0)
            {
                transform.forward = Quaternion.Euler(0, Time.deltaTime * m_TurnSpeed, 0) * transform.forward;
            }
            transform.position += m_Speed * Time.deltaTime * transform.forward;
        }

        IEnumerator Co_InitGrow()
        {
            float lerp = 0;
            while(true)
            {
                transform.localScale = lerp * Vector3.one;
                lerp += 5*Time.deltaTime;
                yield return null;

                if (lerp >= 1)
                    break;
            }

            transform.localScale = Vector3.one;
        }
    }
}