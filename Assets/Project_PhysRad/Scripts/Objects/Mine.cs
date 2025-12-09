using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter
{
    public class Mine : MonoBehaviour
    {
        public GameObject m_ExplodeParticle;

        bool exploded = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider coll)
        {
            if (!exploded)
            {
                if (coll.gameObject.tag != "Player")
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