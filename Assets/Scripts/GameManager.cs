using System;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private GameObject victoryText;
    private bool areAllSpawnersDestroyed = false;
    private bool areAllEnemiesKilled = false;
    private bool keyBindingTextToggled = false;
    public bool gameNotOver = true;
    
    List<GameObject> aliveEnemies = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisableText());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (Input.GetKeyDown(KeyCode.Escape) && gameNotOver){
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

        if (!gameNotOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Level1");
            }
        }

        if (areAllSpawnersDestroyed && areAllEnemiesKilled)
        {
            victoryText.SetActive(true);
            Time.timeScale = 0f;
            gameNotOver = false;
            playerMovement.inputBlocked = true;
            weaponHandler.inputBlocked = true;
        }
    }

    private void HandleAllSpawnersDead()
    {
        areAllSpawnersDestroyed = true;
        Debug.Log("AllSpawnersDestroyed Action received. Setting areAllSpawnersDestroyed => true");
        startTrackingEnemies();
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(objectiveTextDuration);
        objectiveText.SetActive(false);
    }

    private void startTrackingEnemies()
    {
        EnemyScript[] enemies = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
        foreach(EnemyScript enemy in enemies)
        {
            aliveEnemies.Add(enemy.gameObject);
        }
    }
}
