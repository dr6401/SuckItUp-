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
    }
}
