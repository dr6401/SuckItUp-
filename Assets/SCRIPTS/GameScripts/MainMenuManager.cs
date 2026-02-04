using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private MMFeedbacks loadHallwaySceneFeedback;

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
        loadHallwaySceneFeedback?.PlayFeedbacks();
    }

    public void QuiteGame()
    {
        Application.Quit();
    }
}
