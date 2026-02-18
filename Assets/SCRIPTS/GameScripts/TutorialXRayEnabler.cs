using System;
using UnityEngine;

public class TutorialXRayEnabler : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log("Got destroyed");
        GameEvents.OnPlayerKilledAllEnemies?.Invoke();
    }
}
