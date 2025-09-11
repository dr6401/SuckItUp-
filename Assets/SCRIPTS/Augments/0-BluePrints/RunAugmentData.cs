using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "RunAugmentData", menuName = "Runtime Augment Data/Run Data")]
public class RunAugmentData : ScriptableObject
{
    public List<Augment> chosenAugments = new List<Augment>();

    public void AddToChosenAugments(Augment augment)
    {
        if (!chosenAugments.Contains(augment))
        {
            chosenAugments.Add(augment);   
        }
    }

    public bool IsAugmentInChosenAugments(Augment augment)
    {
        return chosenAugments.Contains(augment);
    }

    public int NumberOfChosenAugments()
    {
        return chosenAugments.Count;
    }
    
    public void ResetChosenAugments()
    {
        chosenAugments.Clear();
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
        }
    }
#endif
}
