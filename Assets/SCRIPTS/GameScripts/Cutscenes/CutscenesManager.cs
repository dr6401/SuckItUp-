using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class CutscenesManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Dictionary<CutsceneSO.CutsceneID, CutsceneSO> lookup;
    [SerializeField] private CutsceneSO current;
    [SerializeField] private CutsceneSO[] allCutscenes;
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float canvasFadeDuration = 5f;
    [SerializeField] private GameObject warningText;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private MMFeedbacks loadMainMenuFeedback;
    [SerializeField] private MMFeedbacks loadPigLevelFeedback;
    [SerializeField] private MMFeedbacks loadSheepLevelFeedback;
    [SerializeField] private MMFeedbacks loadCowLevelFeedback;
    [SerializeField] private MMFeedbacks loadChickLevelFeedback;
    [SerializeField] private MMFeedbacks loadEndlessLevelFeedback;
    private Coroutine canvasFadeCoroutine;

    private void Awake()
    {
        LoadAllCutscenes();
    }

    void Start()
    {
        SelectCutscene();
        Time.timeScale = 1f;
        canvasGroup.interactable = true; // reset canvasGroup values
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 0; 
        StartCoroutine(PlayCutsceneSequence());
    }

    private void LoadAllCutscenes()
    {
        allCutscenes = Resources.LoadAll<CutsceneSO>("Cutscenes");
        lookup = new Dictionary<CutsceneSO.CutsceneID, CutsceneSO>();

        foreach (var cutscene in allCutscenes)
        {
            if (!lookup.TryAdd(cutscene.id, cutscene))
            {
                Debug.LogWarning("Duplicate CutsceneID found: " + cutscene.id);
            }
        }
    }

    private void SelectCutscene()
    {
        //SaveData saveData = SaveSystem.Load();
        //int levelProgression = saveData.campaignData.campaignProgressLevel;
        int levelProgression = PlayerPrefs.GetInt("NextCutscene", 0);
        var id = (CutsceneSO.CutsceneID)levelProgression; // If no next scene exists just play the intro
        
        if (!lookup.TryGetValue(id, out current))
        {
            Debug.Log($"current selected cutsceneSO was null, skipping to MainMenu");
            loadMainMenuFeedback.PlayFeedbacks();
            return;
        }
        Debug.Log($"levelProgression: {levelProgression}, lookup.Count: {lookup.Count}");
        current = lookup[id];
        //if (id == 0) warningText.SetActive(true);
        Debug.Log($"PlayerPrefs.NextCutscene: {(CutsceneSO.CutsceneID)PlayerPrefs.GetInt("NextCutscene")}");
    }

    private IEnumerator PlayCutsceneSequence()
    {
        if (current == null)
        {
            loadMainMenuFeedback.PlayFeedbacks();
            yield break;
        }
        if (current.music != null)
        {
            musicAudioSource.PlayOneShot(current.music);
        }
        
        PlayerPrefs.SetInt("NextCutscene", PlayerPrefs.GetInt("NextCutscene", 0) + 1);

        foreach (string line in current.text)
        {
            if (canvasFadeCoroutine != null) StopCoroutine(canvasFadeCoroutine);
            canvasFadeCoroutine = StartCoroutine(FadeCanvas(1f));
            //yield return new WaitForSeconds(canvasFadeDuration);
            text.text = line;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
            if (canvasFadeCoroutine != null) StopCoroutine(canvasFadeCoroutine);
            canvasFadeCoroutine = StartCoroutine(FadeCanvas(0));
            yield return new WaitForSeconds(canvasFadeDuration);
        }

        PlayerPrefs.Save();
        //Debug.Log($"PlayerPrefs.NextScene: {(CutsceneSO.CutsceneID)PlayerPrefs.GetInt("NextCutscene")}");
        switch (current.id)
        {
            case CutsceneSO.CutsceneID.Intro: 
                //GameAnalyticsManager.Instance?.FunnelFinished(1, "watched_intro_cutscene");
                loadMainMenuFeedback.PlayFeedbacks();
                break;
            /*case CutsceneSO.CutsceneID.ChickenToPig: 
                //GameAnalyticsManager.Instance?.FunnelFinished(9, "watched_chicken_to_pig_cutscene");
                loadPigLevelFeedback.PlayFeedbacks();
                break;
            case CutsceneSO.CutsceneID.PigToSheep: 
                //GameAnalyticsManager.Instance?.FunnelFinished(16, "watched_pig_to_sheep_cutscene");
                loadSheepLevelFeedback.PlayFeedbacks();
                break;
            case CutsceneSO.CutsceneID.SheepToCow: 
                //GameAnalyticsManager.Instance?.FunnelFinished(23, "watched_sheep_to_cow_cutscene");
                loadCowLevelFeedback.PlayFeedbacks();
                break;
            case CutsceneSO.CutsceneID.CowToChick: 
                //GameAnalyticsManager.Instance?.FunnelFinished(31, "watched_cow_to_chicks_cutscene");
                loadChickLevelFeedback.PlayFeedbacks();
                break;
            case CutsceneSO.CutsceneID.ChickToEndless: 
                loadMainMenuFeedback.PlayFeedbacks();
                break;*/
            default:
                Debug.LogWarning($"current.id was {(CutsceneSO.CutsceneID)current.id}, loading main menu as backup");
                loadMainMenuFeedback.PlayFeedbacks();
                break;
        }
        //SceneManager.LoadScene(current.nextScene);
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;
        while (time < canvasFadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / canvasFadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
