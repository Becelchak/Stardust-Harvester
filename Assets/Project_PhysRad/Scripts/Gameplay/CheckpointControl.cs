using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class CheckpointControl : MonoBehaviour
    {
        public static CheckpointControl m_Main;
        public Checkpoint[] m_Checkpoints;
        void Awake()
        {
            m_Main = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
