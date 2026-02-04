using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentSpinManager : MonoBehaviour
{
    [Header("Augment Persistence")]
    [SerializeField] private RunAugmentData runAugmentData;
    private Augment augmentToKeep;
    [SerializeField] private TMP_Text keepAugmentText;
    [SerializeField] private TMP_Text keepAugmentDescriptionText;
    [SerializeField] private TMP_Text keepAllAugmentsText;
    [SerializeField] private Image keepAugmentImage;
    [SerializeField] private CanvasGroup augmentSpinCanvas;
    [SerializeField] private CanvasGroup noNewAugmentKeptCanvas;
    [SerializeField] private CanvasGroup keepAllAugmentsCanvas;

    [Header("Death messages")]
    [SerializeField] private List<string> normalDeathMessages;
    private float augmentUIShowTime = 3;
    private float augmentUIFadeTime = 2f;
    private float augmentUICurrentFadeOutTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (augmentSpinCanvas == null)
        {
            augmentSpinCanvas = GetComponentInChildren<CanvasGroup>();
        }
        if (keepAugmentText == null)
        {
            keepAugmentText = GetComponentInChildren<TMP_Text>();
        }
        if (keepAugmentImage == null)
        {
            keepAugmentImage = GetComponentInChildren<Image>();
        }
        if (!PlayerDeathManager.Instance.hasPlayerDiedInPreviousScene) return;
        if (!SettingsManager.Instance.isDifficultyHardcore)
        {
            keepAllAugmentsText.text = GetRandomNormalDeathMessage() + "\n \n You get to keep all your augments";
            StartCoroutine(FadeOutKeepAllAugmentsCanvas());
            runAugmentData.AddChosenAugmentsToPermanentlyChosenAugments();
        }
        else
        {
            Debug.Log("Since you've chosen hardcore, you only keep 1 augment!");
            StartAugmentSpin();   
        }
        PlayerDeathManager.Instance.hasPlayerDiedInPreviousScene = false; // If player died, reset this value to false
    }

    private void StartAugmentSpin()
    {
        var notYetPermanentlyChosenAugments = runAugmentData.chosenAugments
            .Where(a => !runAugmentData.permanentlyChosenAugments.Contains((a)))
            .ToList();
        if (notYetPermanentlyChosenAugments.Count > 0)
        {
            augmentToKeep = notYetPermanentlyChosenAugments.MMRandom();
            runAugmentData.AddToPermanentlyChosenAugments(augmentToKeep);
            Debug.Log($"Random augment chosen was {augmentToKeep.augmentName}");
            runAugmentData.ResetChosenAugments();
            Debug.Log($"Deleted all other augments");
            runAugmentData.AddPermanentlyChosenAugmentsToChosenAugments();
            if (keepAugmentText != null)
            {
                keepAugmentText.text = $"You get to keep\n{augmentToKeep.augmentName}!";   
            }

            if (keepAugmentDescriptionText != null)
            {
                keepAugmentDescriptionText.text = augmentToKeep.description;
            }
            if (keepAugmentImage != null)
            {
                keepAugmentImage.sprite = augmentToKeep.icon;
            }
            StartCoroutine(FadeOutAugmentSpinCanvasUI());
        }
        else
        {
            Debug.Log("All chosen augments are already permanent, not doing anything");
            StartCoroutine(FadeOutNoNewlyKeptAugmentsCanvas());
        }
    }

    private IEnumerator FadeOutAugmentSpinCanvasUI()
    {
        while (augmentSpinCanvas?.alpha < 1)
        {
            augmentSpinCanvas.alpha = Mathf.MoveTowards(augmentSpinCanvas.alpha, 1, Time.deltaTime / augmentUIFadeTime);
            yield return null;
        }
        yield return new WaitForSeconds(augmentUIShowTime);
        while (augmentSpinCanvas?.alpha > 0)
        {
            augmentSpinCanvas.alpha = Mathf.MoveTowards(augmentSpinCanvas.alpha, 0, Time.deltaTime / (augmentUIFadeTime));
            yield return null;
        }
    }
    
    private IEnumerator FadeOutNoNewlyKeptAugmentsCanvas()
    {
        while (noNewAugmentKeptCanvas?.alpha < 1)
        {
            noNewAugmentKeptCanvas.alpha = Mathf.MoveTowards(noNewAugmentKeptCanvas.alpha, 1, Time.deltaTime / augmentUIFadeTime);
            yield return null;
        }
        yield return new WaitForSeconds(augmentUIShowTime);
        while (noNewAugmentKeptCanvas?.alpha > 0)
        {
            noNewAugmentKeptCanvas.alpha = Mathf.MoveTowards(noNewAugmentKeptCanvas.alpha, 0, Time.deltaTime / (augmentUIFadeTime));
            yield return null;
        }
    }
    
    private IEnumerator FadeOutKeepAllAugmentsCanvas()
    {
        while (keepAllAugmentsCanvas?.alpha < 1)
        {
            keepAllAugmentsCanvas.alpha = Mathf.MoveTowards(keepAllAugmentsCanvas.alpha, 1, Time.deltaTime / augmentUIFadeTime);
            yield return null;
        }
        yield return new WaitForSeconds(augmentUIShowTime);
        while (keepAllAugmentsCanvas?.alpha > 0)
        {
            keepAllAugmentsCanvas.alpha = Mathf.MoveTowards(keepAllAugmentsCanvas.alpha, 0, Time.deltaTime / (augmentUIFadeTime));
            yield return null;
        }
    }

    private string GetRandomNormalDeathMessage()
    {
        if (normalDeathMessages.Count <= 0)
        {
            return "You fall… but your powers don’t.";
        }
        int index = Random.Range(0, normalDeathMessages.Count);
        return normalDeathMessages[index];
    }
}
