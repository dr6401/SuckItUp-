using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    [SerializeField] private MMFeedbacks loadMainMenuSceneFeedback;
    public void LoadMainMenuScene()
    {
        loadMainMenuSceneFeedback?.PlayFeedbacks();
    }
}
