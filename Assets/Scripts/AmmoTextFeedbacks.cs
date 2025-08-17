using System;
using UnityEngine;
using MoreMountains;
using MoreMountains.Feedbacks;

public class AmmoTextFeedbacks : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MMFeedbacks ammoIncrease;
    [SerializeField] private MMFeedbacks noAmmoLeft;

    
    private void PlayAmmoIncrease()
    {
        ammoIncrease?.PlayFeedbacks();
    }
    private void PlayNoAmmoLeft()
    {
        noAmmoLeft?.PlayFeedbacks();
    }

    
    
    private void OnEnable()
    {
        WeaponHandler.OnAmmoIncrease += PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft += PlayNoAmmoLeft;
    }
    private void OnDisable()
    {
        WeaponHandler.OnAmmoIncrease -= PlayAmmoIncrease;
        WeaponHandler.OnNoAmmoLeft -= PlayNoAmmoLeft;
    }
}
