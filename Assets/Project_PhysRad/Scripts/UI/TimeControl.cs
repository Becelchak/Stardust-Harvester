using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class TimeControl : MonoBehaviour
    {
        [HideInInspector]
        public float m_DayTime = 0;
        [HideInInspector]
        public int m_DaysPassed = 0;
        [HideInInspector]
        public float m_TimeSpeed = 1;

        public Light m_GlobalLight;

        public static TimeControl Current;

        void Awake()
        {
            Current = this;
        }

        void Start()
        {
            m_DayTime = 0;
            m_DaysPassed = 0;
            m_TimeSpeed = .01f;
        }

        // Update is called once per frame
        void Update()
        {
            m_DayTime += m_TimeSpeed * Time.deltaTime;
            if (m_DayTime > 1)
            {
                m_DaysPassed++;
                m_DayTime = 0;
            }

            //m_GlobalLight.intensity = 0.6f + .4f * Mathf.Sin(2 * m_DayTime * Mathf.PI);

            //Vector3 lightTargetPoint = new Vector3(Mathf.Cos(2 * m_DayTime * Mathf.PI), -1, .4f);
            //m_GlobalLight.transform.forward = lightTargetPoint;
        }
    }
}