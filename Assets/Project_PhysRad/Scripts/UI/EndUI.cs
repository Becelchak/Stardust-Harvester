using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Shooter
{
    public class EndUI : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BtnExit()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}