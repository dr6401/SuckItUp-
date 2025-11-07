using System;
using UnityEngine;

public class RunAugmentDataManager : MonoBehaviour
{
    public RunAugmentData runAugmentData;
    public static RunAugmentDataManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        GameEvents.OnEnteredMainMenu += RemoveAllChosenAugments;
        GameEvents.OnEnteredMainMenu += RemoveAllPermanentlyChosenAugments;
    }

    private void RemoveAllChosenAugments()
    {
        runAugmentData.ResetChosenAugments();
        Debug.Log($"Removed all chosen augments, ChosenAugments list: {runAugmentData.chosenAugments}");
    }

    private void RemoveAllPermanentlyChosenAugments()
    {
        runAugmentData.ResetPermanentlyChosenAugments();
        Debug.Log($"Removed all permanently chosen augments, PermanentlyChosenAugments list: {runAugmentData.permanentlyChosenAugments}");
    }

    private void OnDestroy()
    {
        GameEvents.OnEnteredMainMenu -= RemoveAllChosenAugments;
        GameEvents.OnEnteredMainMenu -= RemoveAllPermanentlyChosenAugments;
    }
}
