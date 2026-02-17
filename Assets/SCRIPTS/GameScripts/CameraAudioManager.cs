using System;
using UnityEngine;

public class CameraAudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera backupCamera;
    
    private AudioListener _backupAudioListener;
    void Start()
    {
        if (backupCamera != null)
        {
            _backupAudioListener = backupCamera.GetComponent<AudioListener>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCamera == null && backupCamera != null && !_backupAudioListener.enabled)
        {
            backupCamera.gameObject.SetActive(true);
            backupCamera.enabled = true;
            _backupAudioListener.enabled = true;
        }
    }

    private void EnableBackupCamera()
    {
        if (backupCamera == null) return;
        backupCamera.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += EnableBackupCamera;
        GameEvents.OnLevelTimeRanOut += EnableBackupCamera;
        GameEvents.OnLevelCompleted += EnableBackupCamera;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= EnableBackupCamera;
        GameEvents.OnLevelTimeRanOut -= EnableBackupCamera;
        GameEvents.OnLevelCompleted -= EnableBackupCamera;
    }
}
