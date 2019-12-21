using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameScripts
{
    public class MenuScript : MonoBehaviour
    {
        public GameObject helpCanvas;
        public GameObject menuCanvas;

        public GameObject btnStart;
        public GameObject btnReturn;
        public void Start()
        {
            //Nothing yet
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Scenes/Alpha", LoadSceneMode.Single);
        }


        public void Quit()
        {
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void Help()
        {
            menuCanvas.SetActive(false);
            helpCanvas.SetActive(true);
            btnReturn.GetComponent<Button>().Select();
        }

        public void Return()
        {
            menuCanvas.SetActive(true);
            helpCanvas.SetActive(false);
            btnStart.GetComponent<Button>().Select();
        }
    }
}