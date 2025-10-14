using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static SettingsManager Instance;
    
    public static Action<int> OnMouseInvertedFromSettingsManager;
    public static Action<bool> OnDifficultySetToHardcoreFromSettingsManager;

    //[SerializeField] private Slider sensitivitySlider;
    //[SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Texture2D cursorSprite;
    public bool isMouseInverted = false;
    public bool isDifficultyHardcore = false;

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
        isDifficultyHardcore = PlayerPrefs.GetInt("HardCoreDifficulty", 0) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs

        
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
            OnMouseInvertedFromSettingsManager?.Invoke(-1); // Send event to player to invert mouse
        }
        else OnMouseInvertedFromSettingsManager?.Invoke(1);
    }
    
    private void UpdateDifficulty(bool hasDifficultyBeenSetToHardcore)
    {
        isDifficultyHardcore = hasDifficultyBeenSetToHardcore;
        if (isDifficultyHardcore)
        {
            OnDifficultySetToHardcoreFromSettingsManager?.Invoke(true); // this unnecessary code, as changing the difficulty during the game is not possible
        }
        else OnDifficultySetToHardcoreFromSettingsManager?.Invoke(false); // this unnecessary code, as changing the difficulty during the game is not possible
    }

    private void OnEnable()
    {
        GameEvents.OnMouseInverted += UpdateIsMouseInverted;
        GameEvents.OnDifficultyChangedToHardcore += UpdateDifficulty;
    }
    
    private void OnDisable()
    {
        GameEvents.OnMouseInverted -= UpdateIsMouseInverted;
        GameEvents.OnDifficultyChangedToHardcore -= UpdateDifficulty;
    }
}
