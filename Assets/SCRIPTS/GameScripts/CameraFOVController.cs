using System;
using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class CameraFOVController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Camera camera;
    [SerializeField] private float fovChangeSpeed = 10f;
    private bool isAiming = false;
    
    [SerializeField] private MMFeedbacks onShootCameraShakeFeedback;
    [SerializeField] private MMF_Player onStartVacuumCameraFXFeedback;
    [SerializeField] private MMF_Player onStopVacuumCameraFXFeedback;
    private MMF_LensDistortion_URP vacuumStartlensDistortionFeedback;
    private MMF_LensDistortion_URP vacuumStoplensDistortionFeedback;
    private float timeSinceStartedLensDistortionFeedback = 0;
    
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
        vacuumStoplensDistortionFeedback = onStopVacuumCameraFXFeedback.GetFeedbackOfType<MMF_LensDistortion_URP>();
        vacuumStartlensDistortionFeedback = onStartVacuumCameraFXFeedback.GetFeedbackOfType<MMF_LensDistortion_URP>();
        
    }
    void Update()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        timeSinceStartedLensDistortionFeedback += Time.deltaTime;
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
        Debug.Log("Played shooting camera fx");
    }

    private void PlayOnStartVacuumingFXFeedback()
    {
        timeSinceStartedLensDistortionFeedback = 0;
        onStartVacuumCameraFXFeedback?.PlayFeedbacks();
        Debug.Log("Started vacuuming camera fx");
        
    }
    
    private void PlayOnStopVacuumingFXFeedback()
    {
        onStartVacuumCameraFXFeedback?.StopFeedbacks();
        float currentLensDistortion =
            vacuumStartlensDistortionFeedback.Intensity.Evaluate(timeSinceStartedLensDistortionFeedback) * vacuumStartlensDistortionFeedback.RemapIntensityOne; //lensDistortion.intensity.value;
        if (vacuumStoplensDistortionFeedback != null) vacuumStoplensDistortionFeedback.RemapIntensityOne = currentLensDistortion;
        StartCoroutine(PlayOnStopVacuumCameraFXFeedbackAfterAWhile());
    }

    private IEnumerator PlayOnStopVacuumCameraFXFeedbackAfterAWhile()
    {
        yield return null;
        onStopVacuumCameraFXFeedback?.PlayFeedbacks();
        Debug.Log("Stopped vacuuming camera fx");
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
