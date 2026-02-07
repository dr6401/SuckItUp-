using System;
using DamageNumbersPro;
using UnityEngine;
using MoreMountains;
using MoreMountains.Feedbacks;
using Random = UnityEngine.Random;

public class AmmoTextFeedbacks : MonoBehaviour
{
    [SerializeField] private RectTransform dustScorePosition;
    [SerializeField] private DamageNumber dustScoreIncreaseDmgNumbers;
    
    [SerializeField] private MMFeedbacks ammoIncrease;
    [SerializeField] private MMFeedbacks dustScoreIncrease;
    [SerializeField] private MMFeedbacks noAmmoLeft;
    [SerializeField] private MMFeedbacks healthIncrease;
    [SerializeField] private MMFeedbacks healthDecrease;
    [SerializeField] private MMFeedbacks lowLevelTimer;

    private void PlayDustScoreIncrease()
    {
        dustScoreIncrease?.PlayFeedbacks();
        dustScoreIncreaseDmgNumbers?.SpawnGUI(dustScorePosition,
            new Vector2(Random.Range(-20,20), -20));
    }
    
    private void PlayAmmoIncrease()
    {
        ammoIncrease?.PlayFeedbacks();
    }
    private void PlayNoAmmoLeft()
    {
        noAmmoLeft?.PlayFeedbacks();
    }

    private void PlayHealthIncrease(int increaseAmount)
    {
        //Debug.Log($"Health Increase: {increaseAmount}");
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
        GameEvents.OnSuckDust += PlayDustScoreIncrease;
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
        GameEvents.OnSuckDust -= PlayDustScoreIncrease;
        WeaponHandler.OnAmmoIncrease -= PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeft;
        GameEvents.OnTriggerHealthIncreaseFeedback -= PlayHealthIncrease;
        GameEvents.OnDamageTaken -= PlayHealthDecrease;
        GameEvents.OnLowLevelTimer -= PlayLowLevelTimer;
        GameEvents.OnLevelTimeRanOut -= StopLowLevelTimer;
        GameEvents.OnPlayerDeath -= StopLowLevelTimer;
    }
}
