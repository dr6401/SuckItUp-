using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static SettingsManager Instance;
    
    public static Action<int> OnMouseInvertedFromSettingsManager;
    public static Action<bool> OnDifficultySetToHardcoreFromSettingsManager;
    public static Action<bool> OnWeaponSwitchWithScrollEnabledFromSettingsManager;

    //[SerializeField] private Slider sensitivitySlider;
    //[SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Texture2D cursorSprite;
    public bool isMouseInverted = false;
    public bool isDifficultyHardcore = false;
    public float fOV;
    public bool isWeaponSwitchWithScrollEnabled = true;

    public static PlayerControls controls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Player.Enable();
            controls.UI.Enable();
            controls.GameEvents.Enable();
        }
        ApplyAllSavedBindings();
        fOV = PlayerPrefs.GetFloat("FOV", 70);
        isMouseInverted = PlayerPrefs.GetInt("Inverted", 0) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs
        isDifficultyHardcore = PlayerPrefs.GetInt("HardCoreDifficulty", 0) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs
        isWeaponSwitchWithScrollEnabled = PlayerPrefs.GetInt("WeaponSwitchScroll", 0) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs
    }

    private void ApplyAllSavedBindings()
    {
        foreach (var map in controls.asset.actionMaps) // Loop over all actionMaps (Player, UI, GameEvents)
        {
            foreach (var action in map.actions) // Loop over all actions in that actionMap
            {
                for (int i = 0; i < action.bindings.Count; i++) // loop over all bindings for the action and load those which exist
                {
                    string key = action.name + "_binding" + i;
                    if (PlayerPrefs.HasKey(key))
                    {
                        string savedPath = PlayerPrefs.GetString(key);
                        action.ApplyBindingOverride(i, savedPath);
                    }
                }
            }
        }
    }

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

    private void UpdateFOV(float targetFOV)
    {
        fOV = targetFOV;
        PlayerPrefs.SetFloat("FOV", targetFOV);
    }
    
    private void UpdateWeaponSwitchWithScroll(bool canWeaponSwitchWithScrollBtn)
    {
        isWeaponSwitchWithScrollEnabled = canWeaponSwitchWithScrollBtn;
        if (isWeaponSwitchWithScrollEnabled)
        {
            OnWeaponSwitchWithScrollEnabledFromSettingsManager?.Invoke(true); // this unnecessary code, as changing the difficulty during the game is not possible
        }
        else OnWeaponSwitchWithScrollEnabledFromSettingsManager?.Invoke(false); // this unnecessary code, as changing the difficulty during the game is not possible
    }

    private void OnEnable()
    {
        GameEvents.OnMouseInverted += UpdateIsMouseInverted;
        GameEvents.OnDifficultyChangedToHardcore += UpdateDifficulty;
        GameEvents.OnFOVChanged += UpdateFOV;
        GameEvents.OnWeaponSwitchScrollChanged += UpdateWeaponSwitchWithScroll;
    }
    
    private void OnDisable()
    {
        GameEvents.OnMouseInverted -= UpdateIsMouseInverted;
        GameEvents.OnDifficultyChangedToHardcore -= UpdateDifficulty;
        GameEvents.OnFOVChanged -= UpdateFOV;
        GameEvents.OnWeaponSwitchScrollChanged -= UpdateWeaponSwitchWithScroll;
    }
}
