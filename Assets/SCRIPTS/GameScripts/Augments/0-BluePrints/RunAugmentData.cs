using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "RunAugmentData", menuName = "Runtime Augment Data/Run Data")]
public class RunAugmentData : ScriptableObject
{
    public List<Augment> chosenAugments = new List<Augment>();
    public List<Augment> permanentlyChosenAugments = new List<Augment>();

    public void AddToChosenAugments(Augment augment)
    {
        if (!chosenAugments.Contains(augment))
        {
            chosenAugments.Add(augment);   
        }
    }
    public void AddToPermanentlyChosenAugments(Augment augment)
    {
        if (!permanentlyChosenAugments.Contains(augment))
        {
            permanentlyChosenAugments.Add(augment);   
        }
    }

    public bool IsAugmentInChosenAugments(Augment augment)
    {
        return chosenAugments.Contains(augment);
    }
    public bool IsAugmentInPermanentlyChosenAugments(Augment augment)
    {
        return permanentlyChosenAugments.Contains(augment);
    }

    public int NumberOfChosenAugments()
    {
        return chosenAugments.Count;
    }
    public int NumberOfPermanentlyChosenAugments()
    {
        return permanentlyChosenAugments.Count;
    }
    
    public void ResetChosenAugments()
    {
        chosenAugments.Clear();
    }
    public void ResetPermanentlyChosenAugments()
    {
        permanentlyChosenAugments.Clear();
    }

    public void AddChosenAugmentsToPermanentlyChosenAugments()
    {
        foreach (Augment chosenAugment in chosenAugments)
        {
            permanentlyChosenAugments.Add(chosenAugment);
            Debug.Log($"Added {chosenAugment.name} from permanentlyChosenAugments to chosenAugments");
        }
    }

    public void AddPermanentlyChosenAugmentsToChosenAugments()
    {
        foreach (Augment permanentAugment in permanentlyChosenAugments)
        {
            chosenAugments.Add(permanentAugment);
            Debug.Log($"Added {permanentAugment.name} from permanentlyChosenAugments to chosenAugments");
        }
    }
    
// This was meant to clear the augments when exiting play mode, but it's not even needed apparently lol
#if UNITY_EDITOR
    private void OnEnable() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            ResetChosenAugments();
            ResetPermanentlyChosenAugments();
        }
    }
#endif
}
