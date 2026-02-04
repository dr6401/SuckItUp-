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
            backupCamera.enabled = true;
            _backupAudioListener.enabled = true;
        }
    }
}
