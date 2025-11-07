using MoreMountains.Tools;
using UnityEngine;

public class AugmentSpinManager : MonoBehaviour
{
    [Header("Augment Persistence")]
    [SerializeField] private RunAugmentData runAugmentData;
    private Augment augmentToKeep;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerDeathManager.Instance.hasPlayerDiedInPreviousScene) return;
        if (!SettingsManager.Instance.isDifficultyHardcore)
        {
            Debug.Log("You get to keep all your augments!");
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
        augmentToKeep = runAugmentData.chosenAugments.MMRandom();
        runAugmentData.AddToPermanentlyChosenAugments(augmentToKeep);
        Debug.Log($"Random augment chosen was {augmentToKeep.name}");
        runAugmentData.ResetChosenAugments();
        Debug.Log($"Deleted all other augments");
        runAugmentData.AddPermanentlyChosenAugmentsToChosenAugments();
    }
}
