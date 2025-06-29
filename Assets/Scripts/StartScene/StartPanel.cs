using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StartScene
{
    public class StartPanel : MonoBehaviour
    {
        public Button startButton;
        public Button continueButton;
        public Button exitButton;
        void Start()
        {
            startButton.onClick.AddListener(StartGame);
            continueButton.onClick.AddListener(ContinueGame);
            exitButton.onClick.AddListener(ExitGame);
        }

        public void StartGame()
        {
            // Start the game here
            SceneManager.LoadScene("GameScene");
        }

        public void ContinueGame()
        {
            // Continue the game here
            SceneManager.LoadScene("GameScene");
        }

        // Exit the game
        public void ExitGame()
        {
            // Exit the game here
            Application.Quit();
        }
    }
}