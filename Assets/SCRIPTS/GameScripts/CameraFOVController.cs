using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFOVController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Camera camera;
    [SerializeField] private float fovChangeSpeed = 10f;
    private bool isAiming = false;

    [SerializeField] private MMFeedbacks onShootCameraShakeFeedback;
    [SerializeField] private MMFeedbacks onVacuumCameraFXFeedback;
    private float targetFOV;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        targetFOV = PlayerPrefs.GetFloat("FOV", GameConstants.defaultFOV);
        if (camera != null)
        {
            camera.fieldOfView = targetFOV;   
        }
    }
    void Update()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }

    public void RequestCameraFOVChange(float requestedFOV)
    {
        if (!isAiming)
        {
            targetFOV = requestedFOV;   
        }
    }

    public void RequestCameraFOVForAiming(float requestedFOV)
    {
        targetFOV = requestedFOV;
    }

    private void PlayOnShootCameraShakeFeedback()
    {
        onShootCameraShakeFeedback?.PlayFeedbacks();
    }

    private void PlayOnStartVacuumingFXFeedback()
    {
        onVacuumCameraFXFeedback?.PlayFeedbacks();
    }
    
    private void PlayOnStopVacuumingFXFeedback()
    {
        onVacuumCameraFXFeedback.StopFeedbacks();
    }

    private void OnEnable()
    {
        GameEvents.OnFOVChanged += RequestCameraFOVForAiming;
        GameEvents.OnShoot += PlayOnShootCameraShakeFeedback;
        GameEvents.OnStartSuckingDust += PlayOnStartVacuumingFXFeedback;
        GameEvents.OnStopSuckingDust += PlayOnStopVacuumingFXFeedback;
        GameEvents.OnPlayerDeath += PlayOnStopVacuumingFXFeedback;
        GameEvents.OnLevelCompleted += PlayOnStopVacuumingFXFeedback;
    }
    private void OnDisable()
    {
        GameEvents.OnFOVChanged -= RequestCameraFOVForAiming;
        GameEvents.OnShoot -= PlayOnShootCameraShakeFeedback;
        GameEvents.OnStartSuckingDust -= PlayOnStartVacuumingFXFeedback;
        GameEvents.OnStopSuckingDust -= PlayOnStopVacuumingFXFeedback;
        GameEvents.OnPlayerDeath -= PlayOnStopVacuumingFXFeedback;
        GameEvents.OnLevelCompleted -= PlayOnStopVacuumingFXFeedback;

    }
}
