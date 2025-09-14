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

    
    private void PlayAmmoIncrease()
    {
        ammoIncrease?.PlayFeedbacks();
        dustScoreIncrease?.PlayFeedbacks();
    }
    private void PlayNoAmmoLeft()
    {
        noAmmoLeft?.PlayFeedbacks();
    }

    private void PlayHealthIncrease()
    {
        healthIncrease?.PlayFeedbacks();
    }
        
    
    private void OnEnable()
    {
        WeaponHandler.OnAmmoIncrease += PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft += PlayNoAmmoLeft;
        WeaponHandler.OnHealthIncrease += PlayHealthIncrease;
    }
    private void OnDisable()
    {
        WeaponHandler.OnAmmoIncrease -= PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeft;
        WeaponHandler.OnHealthIncrease -= PlayHealthIncrease;
    }
}
