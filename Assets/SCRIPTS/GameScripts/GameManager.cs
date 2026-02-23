using System;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private TMP_Text killedAllEnemiesText;
    private bool hasKilledAllEnemiesTextBeenShown = false;
    private bool areAllSpawnersDestroyed = false;
    private bool showShortenedKillAllEnemiesText = false;
    private bool keyBindingTextToggled = false;
    public bool gameOver = false;
    public bool gameLost = false;
    private bool hasAllEnemiesKilledEventBeenFired = false;
    private GameObject player;
    [SerializeField] private Transform enemyFoldersFolder;
    private float timeToLoadNextScene = 5f;
    private Coroutine timeScaleCoroutine;
    private Coroutine settingsFadeCoroutine;
    private CanvasGroup settingsCanvasGroup;
    [SerializeField] private CanvasGroup gameCanvas;
    [SerializeField] private AugmentSelectionUI augmentSelectionUI;
    private bool isAugmentUIOpenedEvenMaybeUnderSettingsCanvas = false;
    private bool isSettingsCanvasCoveringAugmentUI = false;
    private bool hasInputBeenGranted = true;
    private PlayerControls controls;
    [SerializeField] private bool lastLevelOfCampaign = false;
    [SerializeField] private MMFeedbacks loadHallwaySceneFeedback;
    [SerializeField] private MMFeedbacks loadMainMenuSceneFeedback;
    
    List<GameObject> dustParticles = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        StartCoroutine(DisableText());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = GameObject.FindWithTag("Player");
        
        // Getting access to the enemies folder for checking if enemies <= 0
        if (enemyFoldersFolder == null)
        {
            Debug.Log("Folder is null, Finding it in the scene");
            enemyFoldersFolder = GameObject.FindGameObjectWithTag("EnemyFoldersFolder")?.transform;
        }
        // Canvas stuff
        if (augmentSelectionUI == null)
        {
            foreach (var ui in Resources.FindObjectsOfTypeAll<AugmentSelectionUI>())
            {
                if (ui.CompareTag("AugmentSelectionUI") && ui.gameObject.scene.IsValid())
                {
                    augmentSelectionUI = ui;
                }
            }
        }       
        if (settingsCanvas != null)
        {
            settingsCanvasGroup = settingsCanvas.GetComponent<CanvasGroup>();
            settingsCanvasGroup.alpha = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"current alive enemies: {ObjectPooler.Instance.currentAliveEnemies}");
        if (controls.GameEvents.PauseGame.triggered && !gameOver && isAugmentUIOpenedEvenMaybeUnderSettingsCanvas)
        {
            ToggleSettingsCanvasVisibility(1f);
        }
        else if (controls.GameEvents.PauseGame.triggered && !gameOver && hasInputBeenGranted){
            TogglePauseGame();
        }

        if (gameOver && gameLost)
        {
            //PlayerPrefs.SetInt("Level1", 0); // lock all levels again if player lost
            //PlayerPrefs.SetInt("Level2", 0);
            if (controls.GameEvents.Restart.triggered)
            {
                Debug.Log("Triggered PlayLoadSceneHallway()");
                PlayLoadSceneHallway(); // After every death spawn player in hallway
            }
        }

        if (areAllSpawnersDestroyed && !AnyEnemiesAlive())
        {
            TrackRemainingDust();
            ShowKilledAllEnemiesText();
            if (!hasAllEnemiesKilledEventBeenFired)
            {
                hasAllEnemiesKilledEventBeenFired = true;
                GameEvents.OnPlayerKilledAllEnemies?.Invoke();
            }
            if (CheckIfAllDustIsSuckedUp())
            {
                showShortenedKillAllEnemiesText = false;
                EndLevel();
            }
        }

        if (showShortenedKillAllEnemiesText)
        {
            killedAllEnemiesText.gameObject.SetActive(true);
            killedAllEnemiesText.text = $"Clean the remaining <color=#C3C3C3> {dustParticles.Count} </color> dust";
        }
    }

    public void TogglePauseGame()
    {
        keyBindingTextToggled = !keyBindingTextToggled;
        if (keyBindingTextToggled)
        {
            FadeInSettingsUI();
            gameCanvas.alpha = 0;
        }

        if (!keyBindingTextToggled)
        {
            FadeOutSettingsUI();
            gameCanvas.alpha = 1;
        }
        
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

    private bool AnyEnemiesAlive()
    {
        foreach (Transform enemyFolder in enemyFoldersFolder)
        {
            if (enemyFolder?.childCount > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void EndLevel()
    {
        GameEvents.OnLevelCompleted?.Invoke();
        Destroy(player);
        killedAllEnemiesText.gameObject.SetActive(false);
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

    private void ShowKilledAllEnemiesText()
    {
        if (!hasKilledAllEnemiesTextBeenShown)
        {
            hasKilledAllEnemiesTextBeenShown = true;
            StartCoroutine(StartShowingKilledAllEnemiesText());
        }
    }

    private IEnumerator StartShowingKilledAllEnemiesText()
    {
        killedAllEnemiesText.gameObject.SetActive(true);
        float timer = 0f;
        while (timer <= objectiveTextDuration)
        {
            killedAllEnemiesText.text = $"You exterminated all dusties!\nClean the remaining <color=#C3C3C3> {dustParticles.Count} </color> dust";
            timer += Time.deltaTime;
            yield return null;
        }
        //killedAllEnemiesText.gameObject.SetActive(false);
        showShortenedKillAllEnemiesText = true;
    }

    private IEnumerator LoadNextScene()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
        PlayerPrefs.Save();
        yield return new WaitForSeconds(timeToLoadNextScene);
        if (!lastLevelOfCampaign) PlayLoadSceneHallway();
        else PlayLoadMainMenuFeedback();
        /*yield return new WaitForSeconds(timeToLoadNextScene);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);*/
    }
    
    private void ToggleTutorialPause()
    {
        keyBindingTextToggled = !keyBindingTextToggled;
        //settingsCanvas.SetActive(keyBindingTextToggled);

        objectiveText?.SetActive(false);
            
        //Enabling/Disabling the cursor if the game is paused
        if (keyBindingTextToggled)
        {
            //gameCanvas.alpha = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            hasInputBeenGranted = false;
        }
        else
        {
            //gameCanvas.alpha = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            hasInputBeenGranted = true;
        }

        Time.timeScale = keyBindingTextToggled ? 0f : 1f;
        playerMovement.inputBlocked = keyBindingTextToggled;
        weaponHandler.inputBlocked = keyBindingTextToggled;
    }
    
    private void SetHasSettingsUICoveredUpAugmentUI(bool hasSettingsUICoveredUpAugmentUI1)
    {
        isAugmentUIOpenedEvenMaybeUnderSettingsCanvas = hasSettingsUICoveredUpAugmentUI1;
    }

    private void PlayLoadSceneHallway()
    {
        loadHallwaySceneFeedback?.PlayFeedbacks();
    }

    private void PlayLoadMainMenuFeedback()
    {
        loadMainMenuSceneFeedback?.PlayFeedbacks();
    }
    
    private void OnEnable()
    {
        controls = SettingsManager.controls;
        controls.GameEvents.Enable();
        EnemySpawnManager.AllSpawnerDead += HandleAllSpawnersDead;
        GameEvents.OnHasSettingsUICoveredUpAugmentUI += SetHasSettingsUICoveredUpAugmentUI;
        GameEvents.OnGameplayFTUETriggered += ToggleTutorialPause;
        GameEvents.OnGameplayFTUEEnded += ToggleTutorialPause;
    }
    
    private void OnDisable()
    {
        controls.GameEvents.Disable();
        EnemySpawnManager.AllSpawnerDead -= HandleAllSpawnersDead;
        GameEvents.OnHasSettingsUICoveredUpAugmentUI -= SetHasSettingsUICoveredUpAugmentUI;
        GameEvents.OnGameplayFTUETriggered -= ToggleTutorialPause;
        GameEvents.OnGameplayFTUEEnded -= ToggleTutorialPause;
    }
}
