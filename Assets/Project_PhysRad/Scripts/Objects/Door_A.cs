using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Door_A : MonoBehaviour
    {
        public Transform[] m_DoorBases;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Open()
        {
            //m_DoorBody.gameObject.SetActive(false);
            StartCoroutine(Co_Open());
        }

        public void Close()
        {
            //m_DoorBody.gameObject.SetActive(true);
        }

        IEnumerator Co_Open()
        {
            for (int i = 0; i < 50; i++)
            {
                m_DoorBases[2].localPosition = new Vector3( 0,-3.74f * i * 0.02f, 0);
                yield return null;
            }
            yield return new WaitForSeconds(.5f);

            for (int i = 0; i < 100; i++)
            {
                m_DoorBases[0].localPosition = new Vector3(4 * i * 0.01f, 0, 0);
                m_DoorBases[1].localPosition = new Vector3(-4 * i * 0.01f, 0, 0);
                yield return null;
            }
        }
    }
}