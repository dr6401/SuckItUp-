using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroText : MonoBehaviour
{
    [SerializeField] List<string> sentanceList;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject continueTextContainer;
    [SerializeField] private Image blackBgImage;
    [SerializeField] private GameObject mainCanvas;
    private int currentText = 1;
    private PlayerControls controls;

    private GameObject player;
    private PlayerMovement playerMovement;
    private WeaponHandler weaponHandler;
    [SerializeField] private TutorialManager tutorialManager;
    private bool hasPlayerInputBeenGranted = false;


    private void Awake()
    {
        controls = SettingsManager.controls;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponentInChildren<PlayerMovement>();
        weaponHandler = player.GetComponentInChildren<WeaponHandler>();
        blackBgImage = GetComponentInChildren<Image>();
        text.text = sentanceList.First();
        StartCoroutine(DelayContinueText());
    }

    void Update()
    {
        if ((controls.UI.Click.triggered || controls.UI.Submit.triggered) && !hasPlayerInputBeenGranted)
        {
            UpdateText();
        }

        if (!hasPlayerInputBeenGranted)
        {
            playerMovement.inputBlocked = true;
            weaponHandler.inputBlocked = true;
            tutorialManager.hasInputBeenGranted = false;
        }
    }


    public void UpdateText()
    {
        if (currentText == sentanceList.Count)
        {
            hasPlayerInputBeenGranted = true;
            playerMovement.inputBlocked = false;
            weaponHandler.inputBlocked = false;
            tutorialManager.hasInputBeenGranted = true;
            text.text = "";
            continueTextContainer.SetActive(false);
            StartCoroutine(FadeOutIntroCanvas());
        }
        else
        {
            text.text = sentanceList.ElementAt(currentText);
        }
        currentText++;
    }

    private IEnumerator DelayContinueText()
    {
        yield return new WaitForSeconds(2f);
        continueTextContainer.SetActive(true);
    }

    private IEnumerator FadeOutIntroCanvas()
    {
        for (int i = 100; i >= 0; i--)
        {
            Color tempColor = blackBgImage.color;
            tempColor.a = i / 100f;
            blackBgImage.color = tempColor;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(5f);
        mainCanvas.SetActive(true);
    }

    private void OnEnable()
    {
        controls.UI.Enable();
    }
    private void OnDisable()
    {
        controls.UI.Disable();
    }
}
