using System;
using UnityEngine;

public class PlayerVfxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject landedVfx;
    [SerializeField] private Transform playerFeetCollider;
    private float landedVfxInterval = GameConstants.playerLandedInterval;
    private float landedVfxTime;

    private void Update()
    {
        landedVfxTime += Time.deltaTime;
    }

    private void SpawnLandedVfx()
    {
        if (landedVfxTime < landedVfxInterval) return;
        landedVfxTime = 0;
        Instantiate(landedVfx, playerFeetCollider.position, Quaternion.identity);
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerLanded +=  SpawnLandedVfx;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerLanded -=  SpawnLandedVfx;
    }
}
