using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class TargetObject : MonoBehaviour
    {
        public Transform m_TargetCenter;
        void Start()
        {
            TargetsControl.m_Main.AddTarget(this);
            if (m_TargetCenter==null)
            {
                m_TargetCenter = transform;
            }
        }

        void Update()
        {

        }

        public void RemoveFromTargets()
        {
            TargetsControl.m_Main.RemoveTarget(this);
        }
    }

}