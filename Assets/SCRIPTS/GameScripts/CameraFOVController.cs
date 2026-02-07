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
    [SerializeField] private Camera camera;
    [SerializeField] private float fovChangeSpeed = 10f;
    private bool isAiming = false;
    
    [SerializeField] private MMFeedbacks onShootCameraShakeFeedback;
    [SerializeField] private MMF_Player onStartVacuumCameraFXFeedback;
    [SerializeField] private MMF_Player onStopVacuumCameraFXFeedback;
    private MMF_LensDistortion_URP vacuumStartlensDistortionFeedback;
    private MMF_LensDistortion_URP vacuumStoplensDistortionFeedback;
    private float timeSinceStartedLensDistortionFeedback = 0;
    //private float timeSinceStoppedLensDistortionFeedback = 0;

    
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
        //fixedTimeStep = 1f / Application.targetFrameRate;)

    }
    void Update()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        timeSinceStartedLensDistortionFeedback += Time.deltaTime;
        //timeSinceStoppedLensDistortionFeedback += Time.deltaTime;
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
        //float normalizedTime = timeSinceStoppedLensDistortionFeedback / vacuumStoplensDistortionFeedback.Duration;
        //float currentLensDistortion =
        //    vacuumStoplensDistortionFeedback.Intensity.Evaluate(normalizedTime) * vacuumStoplensDistortionFeedback.RemapIntensityZero;
        //if (vacuumStartlensDistortionFeedback != null) vacuumStartlensDistortionFeedback.RemapIntensityOne = currentLensDistortion;
        onStopVacuumCameraFXFeedback.StopFeedbacks();
        timeSinceStartedLensDistortionFeedback = 0;
        onStartVacuumCameraFXFeedback?.PlayFeedbacks();
        //Debug.Log($"timeSinceStoppedLensDistortionFeedback: {timeSinceStoppedLensDistortionFeedback}, normalizedTime: {normalizedTime}, evaluated stopping lens distortion: {currentLensDistortion}");
        
    }
    
    private void PlayOnStopVacuumingFXFeedback()
    {
        float normalizedTime = timeSinceStartedLensDistortionFeedback / vacuumStartlensDistortionFeedback.Duration;
        onStartVacuumCameraFXFeedback?.StopFeedbacks();
        //timeSinceStoppedLensDistortionFeedback = 0;
        float currentLensDistortion =
            vacuumStartlensDistortionFeedback.Intensity.Evaluate(normalizedTime) * vacuumStartlensDistortionFeedback.RemapIntensityOne;
        Debug.Log($"timeSinceStartedLensDistortionedback: {timeSinceStartedLensDistortionFeedback}, normalizedTime: {normalizedTime}, evaluated starting lens distortion: {currentLensDistortion}");
        if (vacuumStoplensDistortionFeedback != null) vacuumStoplensDistortionFeedback.RemapIntensityOne = currentLensDistortion;
        onStopVacuumCameraFXFeedback?.PlayFeedbacks();
        //StartCoroutine(PlayOnStopVacuumCameraFXFeedbackAfterAWhile());
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
