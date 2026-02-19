using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
    public float sensitivity;
    public bool isWeaponSwitchWithScrollEnabled = true;
    
    public AudioMixer audioMixer;
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;


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
        ApplyAllSavedBindings();
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", GameConstants.defaultSensitivity);
        fOV = PlayerPrefs.GetFloat("FOV", GameConstants.defaultFOV);
        isMouseInverted = PlayerPrefs.GetInt("Inverted", GameConstants.defaultInvertedMouse) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs
        isDifficultyHardcore = PlayerPrefs.GetInt("HardCoreDifficulty", GameConstants.defaultHardcoreDifficulty) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs
        isWeaponSwitchWithScrollEnabled = PlayerPrefs.GetInt("WeaponSwitchScroll", GameConstants.defaultWeaponSwitchScroll) == 1; // Makes isMouseInverted equal to "Inverted" or to false, if there's no "Inverted" in PlayerPrefs
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", GameConstants.defaultMasterVolume);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", GameConstants.defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat("GeneralSFXVolume", GameConstants.defaultSFXVolume);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MasterVolume", GameConstants.defaultMasterVolume), 0.0001f, 1f)) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("MusicVolume", GameConstants.defaultMusicVolume), 0.0001f, 1f)) * 20 - 10);
        audioMixer.SetFloat("GeneralSFXVolume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("GeneralSFXVolume", GameConstants.defaultSFXVolume), 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        PlayerPrefs.SetFloat("FOV", fOV);
        PlayerPrefs.SetInt("Inverted", isMouseInverted ? 1 : 0);
        PlayerPrefs.SetInt("HardCoreDifficulty", isDifficultyHardcore ? 1 : 0);
        PlayerPrefs.SetInt("WeaponSwitchScroll", isWeaponSwitchWithScrollEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("GeneralSFXVolume", sfxVolume);
        PlayerPrefs.Save();
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

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        controls.Dispose();
    }
}
