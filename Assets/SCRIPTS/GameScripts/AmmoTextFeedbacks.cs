using System;
using UnityEngine;
using MoreMountains;
using MoreMountains.Feedbacks;

public class AmmoTextFeedbacks : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MMFeedbacks ammoIncrease;
    [SerializeField] private MMFeedbacks dustScoreIncrease;
    [SerializeField] private MMFeedbacks noAmmoLeft;
    [SerializeField] private MMFeedbacks healthIncrease;
    [SerializeField] private MMFeedbacks healthDecrease;
    [SerializeField] private MMFeedbacks lowLevelTimer;

    
    private void PlayAmmoIncrease()
    {
        ammoIncrease?.PlayFeedbacks();
        dustScoreIncrease?.PlayFeedbacks();
    }
    private void PlayNoAmmoLeft()
    {
        noAmmoLeft?.PlayFeedbacks();
    }

    private void PlayHealthIncrease(int increaseAmount)
    {
        Debug.Log($"Health Increase: {increaseAmount}");
        healthIncrease?.PlayFeedbacks();
    }

    private void PlayHealthDecrease(int damageTaken)
    {
        healthDecrease?.PlayFeedbacks();
    }
    
    private void PlayLowLevelTimer()
    {
        lowLevelTimer?.PlayFeedbacks();
    }

    private void StopLowLevelTimer()
    {
        lowLevelTimer?.StopFeedbacks();
    }
    
    private void OnEnable()
    {
        WeaponHandler.OnAmmoIncrease += PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft += PlayNoAmmoLeft;
        GameEvents.OnTriggerHealthIncreaseFeedback += PlayHealthIncrease;
        GameEvents.OnDamageTaken += PlayHealthDecrease;
        GameEvents.OnLowLevelTimer += PlayLowLevelTimer;
        GameEvents.OnLevelTimeRanOut += StopLowLevelTimer;
        GameEvents.OnPlayerDeath += StopLowLevelTimer;
    }
    private void OnDisable()
    {
        WeaponHandler.OnAmmoIncrease -= PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeft;
        GameEvents.OnTriggerHealthIncreaseFeedback -= PlayHealthIncrease;
        GameEvents.OnDamageTaken -= PlayHealthDecrease;
        GameEvents.OnLowLevelTimer -= PlayLowLevelTimer;
        GameEvents.OnLevelTimeRanOut -= StopLowLevelTimer;
        GameEvents.OnPlayerDeath -= StopLowLevelTimer;
    }
}
