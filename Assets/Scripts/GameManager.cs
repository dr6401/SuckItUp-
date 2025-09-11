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
    }

    private void OnEnable()
    {
        EnemySpawnManager.AllSpawnerDead += HandleAllSpawnersDead;
    }
    
    private void OnDisable()
    {
        EnemySpawnManager.AllSpawnerDead -= HandleAllSpawnersDead;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver){
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
        settingsCanvas.SetActive(keyBindingTextToggled);

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
        float easeTime = 2f;

        while (elapsed < easeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, targetTime, (elapsed / easeTime) * (elapsed / easeTime)); // Multiply, so we get a squared function instead of linear
            yield return null;
        }
        Time.timeScale = targetTime;
        timeScaleCoroutine = null;
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
}
