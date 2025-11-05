using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetInt("Level1", 0); // Lock lvl2 and lvl3 when starting game
        PlayerPrefs.SetInt("Level2", 0);
        GameEvents.OnEnteredMainMenu?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadHallwayScene()
    {
        SceneManager.LoadScene("Hallway");
    }

    public void QuiteGame()
    {
        Application.Quit();
    }
}
