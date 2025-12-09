using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Pickup : MonoBehaviour
    {
        public GameObject m_ScoreParticle;
        bool picked = false;

        [HideInInspector]
        public PlayerChar m_TouchedPlayer;

        [Space]
        public string m_ItemType = "Cash";
        public int m_ItemCount = 1;

        public bool m_CanPick = false;
        // Start is called before the first frame update
        void Start()
        {
            m_CanPick = false;
            Invoke("AllowPick", .5f);
        }

        // Update is called once per frame
        void Update()
        {
            //m_Base.localRotation = Quaternion.Euler(0, Time.deltaTime * 100, 0) * m_Base.localRotation;
            if (!picked && m_CanPick)
            {
                if (Vector3.Distance(transform.position, PlayerChar.m_Current.transform.position) <= 5f)
                {
                    m_TouchedPlayer = PlayerChar.m_Current;
                    picked = true;
                    Collider m_PhysCollider = GetComponent<Collider>();
                    if (m_PhysCollider != null)
                    {
                        m_PhysCollider.enabled = false;
                    }
                    StartCoroutine(Co_HandlePick());
                }
            }

        }


        public virtual void HandlePickup()
        {
            m_TouchedPlayer.HandlePickup(m_ItemType, m_ItemCount);
        }

        IEnumerator Co_HandlePick()
        {
            Vector3 startPos = transform.position;
            float lerp = 0;
            while (lerp <= 1)
            {
                transform.position = Vector3.Lerp(startPos, m_TouchedPlayer.transform.position+new Vector3(0,1,0), lerp);
                lerp += 10*Time.deltaTime;
                yield return null;
            }
            transform.position = m_TouchedPlayer.transform.position + new Vector3(0, 1, 0);
            HandlePickup();

            GameObject obj = Instantiate(m_ScoreParticle);
            obj.transform.position = transform.position;
            Destroy(obj, 1);

            Destroy(gameObject);

        }

        private void AllowPick()
        {
            m_CanPick = true;
        }
    }
}
