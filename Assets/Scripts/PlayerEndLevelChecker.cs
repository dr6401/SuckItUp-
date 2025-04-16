using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerEndLevelChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject objectiveText;
    public int currentNumberOfEnemiesInRoom;
    private bool hasShownGoodJobMessage = false;
    private bool hasNotStartedTeleportingToLevel1Yet = true;
    private int sceneLoadingTimer = 5;
    void Start()
    {
        if (tutorialManager == null)
        {
            tutorialManager = GameObject.FindAnyObjectByType<TutorialManager>();
        }
        currentNumberOfEnemiesInRoom = GameObject.FindGameObjectsWithTag("Enemy").Length;

    }

    // Update is called once per frame
    void Update()
    {
        currentNumberOfEnemiesInRoom = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (tutorialManager.aliveDustParticles <= 0 && currentNumberOfEnemiesInRoom <= 0 && !hasShownGoodJobMessage)
        {
            objectiveText.GetComponent<TMP_Text>().text = "Good Job! You cleaned the whole place up";
            tutorialManager.SetObjectiveTextVisible();
            objectiveText.SetActive(true);
            tutorialManager.toggleWeaponTextObject.SetActive(false);
            hasShownGoodJobMessage = true;
        }

        //Debug.Log("tutorialManager.aliveDustParticles: " + tutorialManager.aliveDustParticles);
        //Debug.Log("currentNumberOfEnemiesInRoom: " + currentNumberOfEnemiesInRoom);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EndLevelArea"))
        {
            if (tutorialManager.aliveDustParticles <= 0 && currentNumberOfEnemiesInRoom <= 0)
            {
                objectiveText.GetComponent<TMP_Text>().text = "Congratulations on completing the tutorial!\n Get ready for action in " + sceneLoadingTimer;
                tutorialManager.SetObjectiveTextVisible();
                objectiveText.SetActive(true);
                tutorialManager.toggleWeaponTextObject.SetActive(false);
                if (hasNotStartedTeleportingToLevel1Yet)
                {
                    hasNotStartedTeleportingToLevel1Yet = false;
                    StartCoroutine(StartLoadingNextLevel());
                }
            }
            else
            {
                objectiveText.GetComponent<TMP_Text>().text = "There are still some dusty places you haven't checked!";
                tutorialManager.SetObjectiveTextVisible();
                objectiveText.SetActive(true);
                tutorialManager.toggleWeaponTextObject.SetActive(false);
            }
        }
    }

    private IEnumerator StartLoadingNextLevel()
    {
        while (sceneLoadingTimer > 0)
        {
            objectiveText.GetComponent<TMP_Text>().text = "Congratulations on completing the tutorial!\n Get ready for action in " + sceneLoadingTimer;
            yield return new WaitForSeconds(1);
            sceneLoadingTimer--;
        }
        SceneManager.LoadScene("Level1");
    }
}
