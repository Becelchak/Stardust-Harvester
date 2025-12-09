using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Shooter.Gameplay
{
    public class Platform_A : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Appear()
        {
            StartCoroutine(Co_Appear());
        }

        IEnumerator Co_Appear()
        {
            gameObject.SetActive(true);

            for (float i = 0; i < 1; i+=.05f)
            {
                transform.localScale = i * Vector3.one;
                yield return null;
            }

            transform.localScale = Vector3.one;
            yield return null;
        }
    }
}
