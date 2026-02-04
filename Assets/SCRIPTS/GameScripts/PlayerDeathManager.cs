using System;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    public static PlayerDeathManager Instance;
    public bool hasPlayerDiedInPreviousScene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void SetHasPlayerDiedInPreviousSceneToTrue()
    {
        hasPlayerDiedInPreviousScene = true;
    }
    
    private void SetHasPlayerDiedInPreviousSceneToFalse()
    {
        hasPlayerDiedInPreviousScene = false;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += SetHasPlayerDiedInPreviousSceneToTrue;
        GameEvents.OnResetHasPlayerDiedInPreviousScene += SetHasPlayerDiedInPreviousSceneToFalse;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= SetHasPlayerDiedInPreviousSceneToTrue;
        GameEvents.OnResetHasPlayerDiedInPreviousScene -= SetHasPlayerDiedInPreviousSceneToFalse;
    }
}
