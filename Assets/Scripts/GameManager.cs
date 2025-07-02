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

            Time.timeScale = keyBindingTextToggled ? 0f : 1f;
            playerMovement.inputBlocked = keyBindingTextToggled;
            weaponHandler.inputBlocked = keyBindingTextToggled;
        }

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Level1");
            }
        }

        if (areAllSpawnersDestroyed && enemiesFolder.childCount <= 0)
        {
            Destroy(player);
            victoryText.SetActive(true);
            gameOver = true;
            StartCoroutine(LoadNextScene());
        }
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
        yield return new WaitForSeconds(timeToLoadNextScene);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
