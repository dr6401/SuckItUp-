using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class FTUEGameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject fTUECanvas;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private TMP_Text nextButtonText;
    [SerializeField] private List<string> tutorialTexts;
    [SerializeField] private MMFeedbacks fadeInFtueCanvasFeedback;
    [SerializeField] private int currentTextIndex = 0;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;

    private bool isFtueInProgress = false;

    
    private void Start()
    {
        /*if (!SettingsManager.Instance.hasPlayerCompletedFTUE)
        {
            StartCoroutine(InitiateFTUE());
            SettingsManager.Instance.hasPlayerCompletedFTUE = true;
        }*/
        if (PlayerPrefs.GetInt("GameplayFTUECompleted", 0) == 0)
        {
            StartCoroutine(InitiateGameplayFTUE());
            StartCoroutine(BlockWeaponHandler());   
        }
    }

    private IEnumerator BlockWeaponHandler()
    {
        yield return new WaitForSeconds(2.5f);
        weaponHandler.inputBlocked = true;
    }
    
    
    private IEnumerator InitiateGameplayFTUE()
    {
        yield return new WaitForSeconds(3f);
        PlayerPrefs.SetInt("GameplayFTUECompleted", 1);
        GameEvents.OnGameplayFTUETriggered?.Invoke();
    }

    private void SetFTUECanvasActive()
    {
        playerMovement.inputBlocked = true;
        isFtueInProgress = true;
        Time.timeScale = 0f;
        Debug.Log("Gameplay FTUE initiated");
        //GameEvents.OnGamePaused?.Invoke(true);
        fTUECanvas?.SetActive(true);
        tutorialText.text = tutorialTexts[currentTextIndex];
        fadeInFtueCanvasFeedback?.PlayFeedbacks();
    }

    public void ShowNextText()
    {
        if (currentTextIndex == tutorialTexts.Count - 1)
        {
            GameEvents.OnGameplayFTUEEnded?.Invoke();
            fTUECanvas.SetActive(false);
            //GameEvents.OnGamePaused?.Invoke(false);
            playerMovement.inputBlocked = false;
            isFtueInProgress = false;
            Time.timeScale = 1f;
            return;
        }
        currentTextIndex++;
        tutorialText.text = tutorialTexts[currentTextIndex];
        if (currentTextIndex == tutorialTexts.Count - 1)
        {
            nextButtonText.text = "GO!";
        }

        currentTextIndex = Mathf.Clamp(currentTextIndex, 0, tutorialTexts.Count - 1);
    }

    private void OnEnable()
    {
        GameEvents.OnGameplayFTUETriggered += SetFTUECanvasActive;
    }
    
    private void OnDisable()
    {
        GameEvents.OnGameplayFTUETriggered -= SetFTUECanvasActive;
    }
}
