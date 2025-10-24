using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
