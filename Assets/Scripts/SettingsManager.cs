using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private PlayerMovement playerMovement;
    void Start()
    {
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindFirstObjectByType<PlayerMovement>();
        }

        if (PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
        }
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float sensitivity)
    {
        playerMovement.SetSensitivity(sensitivity);
    }
}
