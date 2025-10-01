using UnityEngine;

public class QuiteGame : MonoBehaviour
{
    public void QuitGameButtonQuiteGame()
    {
        Debug.Log("Quitting application");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
