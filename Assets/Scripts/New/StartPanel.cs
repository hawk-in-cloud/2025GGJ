using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace New
{
    public class StartPanel : MonoBehaviour
    {
        public string gameScene = "Racio_Test";
        public Button startButton;
        public Button continueButton;
        public Button exitButton;

        void Start()
        {
            startButton.onClick.AddListener(StartGame);
            continueButton.onClick.AddListener(ContinueGame);
            exitButton.onClick.AddListener(ExitGame);
            
        }
        

        void StartGame()
        {
            SceneManager.LoadSceneAsync(gameScene);
            
        }
        void ContinueGame()
        {
            SceneManager.LoadSceneAsync(gameScene);
        }
        void ExitGame()
        {
            Application.Quit();
        }

    }
}