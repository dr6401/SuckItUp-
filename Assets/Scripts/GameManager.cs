using System;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private GameObject victoryText;
    private bool areAllSpawnersDestroyed = false;
    private bool keyBindingTextToggled = false;
    public bool gameOver = false;
    private GameObject player;
    private Transform enemiesFolder;
    private float timeToLoadNextScene = 5f;
    private Coroutine timeScaleCoroutine;
    private Coroutine settingsFadeCoroutine;
    private CanvasGroup settingsCanvasGroup;
    [SerializeField] private AugmentSelectionUI augmentSelectionUI;
    private bool isAugmentUIOpenedEvenMaybeUnderSettingsCanvas = false;
    private bool isSettingsCanvasCoveringAugmentUI = false;
    
    List<GameObject> dustParticles = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisableText());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = GameObject.FindWithTag("Player");
        
        // Getting access to the enemies folder for checking if enemies <= 0
        GameObject folder = GameObject.Find("EnemiesFolder");
        if (folder == null)
        {
            Debug.Log("Folder is null, creating new folder");
            folder = new GameObject("EnemiesFolder");
        }
        enemiesFolder = folder.transform;
        // Canvas stuff
        if (augmentSelectionUI == null)
        {
            augmentSelectionUI = FindAnyObjectByType<AugmentSelectionUI>();
        }       
        if (settingsCanvas != null)
        {
            settingsCanvasGroup = settingsCanvas.GetComponent<CanvasGroup>();
            settingsCanvasGroup.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        EnemySpawnManager.AllSpawnerDead += HandleAllSpawnersDead;
        GameEvents.OnHasSettingsUICoveredUpAugmentUI += SetHasSettingsUICoveredUpAugmentUI;
    }
    
    private void OnDisable()
    {
        EnemySpawnManager.AllSpawnerDead -= HandleAllSpawnersDead;
        GameEvents.OnHasSettingsUICoveredUpAugmentUI -= SetHasSettingsUICoveredUpAugmentUI;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && isAugmentUIOpenedEvenMaybeUnderSettingsCanvas)
        {
            ToggleSettingsCanvasVisibility(1f);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !gameOver){
            TogglePauseGame();
        }

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (areAllSpawnersDestroyed && enemiesFolder.childCount <= 0)
        {
            TrackRemainingDust();
            if (CheckIfAllDustIsSuckedUp())
            {
                EndLevel();
            }
        }
    }

    public void TogglePauseGame()
    {
        keyBindingTextToggled = !keyBindingTextToggled;
        if (keyBindingTextToggled) FadeInSettingsUI();
        if (!keyBindingTextToggled) FadeOutSettingsUI();
        
        objectiveText.SetActive(false);

        if (keyBindingTextToggled)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        float targetTime = keyBindingTextToggled ? 0f : 1f;
        SetTimeScale(targetTime);
        playerMovement.inputBlocked = keyBindingTextToggled;
        weaponHandler.inputBlocked = keyBindingTextToggled;
    }

    public void ToggleTimeStop(float target)
    {
        float targetTime = target;
        SetTimeScale(targetTime);
    }

    public void ToggleSettingsCanvasVisibility(float target)
    {
        isSettingsCanvasCoveringAugmentUI = !isSettingsCanvasCoveringAugmentUI;
        Debug.Log("In ToggleSettingsCanvasVisibility; isAugmentUIOpenedEvenMaybeUnderSettingsCanvas: " + isAugmentUIOpenedEvenMaybeUnderSettingsCanvas + ", isSettingsCanvasCoveringAugmentUI: " + isSettingsCanvasCoveringAugmentUI);
        if (isSettingsCanvasCoveringAugmentUI) FadeInSettingsUI();
        else FadeOutSettingsUI();
        
        objectiveText.SetActive(false);
    }

    public void TogglePlayerInputBlocked(float target)
    {
        if (target == 0)
        {
            playerMovement.inputBlocked = true;
            weaponHandler.inputBlocked = true;
        }
        if (target == 1)
        {
            playerMovement.inputBlocked = false;
            weaponHandler.inputBlocked = false;
        }
    }

    public void ToggleMouseVisibility(float target)
    {
        if (target == 1)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (target == 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void TogglePauseGameWithoutSettingsMenu()
    {
        keyBindingTextToggled = !keyBindingTextToggled;
        Debug.Log("In TogglePauseGameWithoutSettingsMenu; keyBindingTextToggled: " + keyBindingTextToggled);
        
        objectiveText.SetActive(false);

        //Enabling/Disabling the cursor if the game is paused
        if (keyBindingTextToggled)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        float targetTime = keyBindingTextToggled ? 0f : 1f;
        SetTimeScale(targetTime);
        playerMovement.inputBlocked = keyBindingTextToggled;
        weaponHandler.inputBlocked = keyBindingTextToggled;
    }

    private void SetTimeScale(float targetTime)
    {
        if (timeScaleCoroutine != null) // Check if there is already a Coroutine running
        {
            StopCoroutine(timeScaleCoroutine); // If it is, stop it and only then start a new one, so there are never 2 Coroutines executing at the same time
        }

        timeScaleCoroutine = StartCoroutine(EaseInOrOutPauseGame(targetTime));
    }


    private IEnumerator EaseInOrOutPauseGame(float targetTime)
    {
        float start = Time.timeScale;
        float elapsed = 0f;
        float easeTime = GameConstants.mediumFadeInOrOutDuration;

        while (elapsed < easeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, targetTime, (elapsed / easeTime) * (elapsed / easeTime)); // Multiply, so we get a squared function instead of linear
            yield return null;
        }
        Time.timeScale = targetTime;
        timeScaleCoroutine = null;
    }
    
    private void FadeInSettingsUI()
    {
        if (settingsFadeCoroutine != null)
        {
            StopCoroutine(settingsFadeCoroutine);
        }
        settingsCanvas.SetActive(true);
        settingsFadeCoroutine = StartCoroutine(FadeInOrOutSettingsCanvas(1));
        if (augmentSelectionUI.gameObject.activeSelf)
        {
            augmentSelectionUI.FadeOutAugmentsUIWithoutDestroyingIt();
            //GameEvents.OnHasSettingsUICoveredUpAugmentUI?.Invoke(true);
        }
    }
    private void FadeOutSettingsUI()
    {
        if (settingsFadeCoroutine != null)
        {
            StopCoroutine(settingsFadeCoroutine);
        }
        settingsFadeCoroutine = StartCoroutine(FadeInOrOutSettingsCanvas(0));
        if (isAugmentUIOpenedEvenMaybeUnderSettingsCanvas)
        {
            augmentSelectionUI.FadeInAugmentsUI();
            //isAugmentUIOpenedEvenMaybeUnderSettingsCanvas = false;
        }
    }
    
    private IEnumerator FadeInOrOutSettingsCanvas(float targetAlpha)
    {
        float start = settingsCanvasGroup.alpha;
        float elapsed = 0f;
        float easeTime = GameConstants.fadeInOrOutDuration;
        if (targetAlpha == 0) easeTime = GameConstants.shortFadeInOrOutDuration;
        while (elapsed < easeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            settingsCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha,(elapsed / easeTime) * (elapsed / easeTime)); // Multiply, so we get a squared function instead of linear
            yield return null;
        }
        settingsCanvasGroup.alpha = targetAlpha;
        if (targetAlpha == 0) settingsCanvas.SetActive(false);
        settingsFadeCoroutine = null;
    }
    private void TrackRemainingDust()
    {
        GameObject[] dustParticlesInRoom = GameObject.FindGameObjectsWithTag("DustPickup");
        foreach(GameObject dustParticle in dustParticlesInRoom)
        {
            if (!dustParticles.Contains(dustParticle))
            {
                dustParticles.Add(dustParticle.gameObject);
            }
        }
    }

    private bool CheckIfAllDustIsSuckedUp()
    {
        if (dustParticles.Count <= 0) return true;
        return false;
    }
    
    public void DustDestroyed(GameObject dust)
    {
        if (dustParticles.Contains(dust))
        {
            dustParticles.Remove(dust);
        }
    }

    private void EndLevel()
    {
        Destroy(player);
        victoryText.SetActive(true);
        gameOver = true;
        StartCoroutine(LoadNextScene());
    }

    private void HandleAllSpawnersDead()
    {
        areAllSpawnersDestroyed = true;
        Debug.Log("AllSpawnersDestroyed Action received. Setting areAllSpawnersDestroyed => true");
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(objectiveTextDuration);
        objectiveText.SetActive(false);
    }

    private IEnumerator LoadNextScene()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
        Debug.Log("Saved to PlayerPrefs scene: " + SceneManager.GetActiveScene().name + " with value of 1");
        PlayerPrefs.Save();
        yield return new WaitForSeconds(timeToLoadNextScene);
        SceneManager.LoadScene("Hallway");
        /*yield return new WaitForSeconds(timeToLoadNextScene);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);*/
    }
    
    private void SetHasSettingsUICoveredUpAugmentUI(bool hasSettingsUICoveredUpAugmentUI1)
    {
        isAugmentUIOpenedEvenMaybeUnderSettingsCanvas = hasSettingsUICoveredUpAugmentUI1;
    }
}
