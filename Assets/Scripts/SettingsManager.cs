using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static SettingsManager Instance;

    //[SerializeField] private Slider sensitivitySlider;
    //[SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Texture2D cursorSprite;
    public bool isMouseInverted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        isMouseInverted = PlayerPrefs.GetInt("Inverted", 0) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs

    }
    void Start()
    {
        /*if (playerMovement == null)
        {
            playerMovement = GameObject.FindFirstObjectByType<PlayerMovement>();
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        Vector2 hotspot = new Vector2(cursorSprite.width / 2f, cursorSprite.height / 2f);
        Cursor.SetCursor(cursorSprite, hotspot, CursorMode.Auto);*/
    }

    /*private void OnSensitivityChanged(float sensitivity)
    {
        playerMovement.SetSensitivity(sensitivity);
    }*/

    private void UpdateIsMouseInverted(bool hasMouseBeenInverted)
    {
        isMouseInverted = hasMouseBeenInverted;
        if (isMouseInverted)
        {
            GameEvents.OnMouseInvertedFromSettingsManager?.Invoke(-1); // Send event to player to invert mouse
        }
        else GameEvents.OnMouseInvertedFromSettingsManager?.Invoke(1);
    }

    private void OnEnable()
    {
        GameEvents.OnMouseInverted += UpdateIsMouseInverted;
    }
    
    private void OnDisable()
    {
        GameEvents.OnMouseInverted -= UpdateIsMouseInverted;
    }
}
