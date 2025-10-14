using System;
using UnityEngine;

public class CameraFOVController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Camera camera;
    [SerializeField] private float fovChangeSpeed = 10f;
    private bool isAiming = false;

    private float targetFOV;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Start()
    {
        targetFOV = PlayerPrefs.GetFloat("FOV", 80);
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

    private void OnEnable()
    {
        GameEvents.OnFOVChanged += RequestCameraFOVForAiming;
    }
    private void OnDisable()
    {
        GameEvents.OnFOVChanged -= RequestCameraFOVForAiming;
    }
}
