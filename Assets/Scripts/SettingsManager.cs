using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Texture2D cursorSprite;
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
        Vector2 hotspot = new Vector2(cursorSprite.width / 2f, cursorSprite.height / 2f);
        Cursor.SetCursor(cursorSprite, hotspot, CursorMode.Auto);
    }

    private void OnSensitivityChanged(float sensitivity)
    {
        playerMovement.SetSensitivity(sensitivity);
    }
}
