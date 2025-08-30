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
        if (camera != null)
        {
            targetFOV = camera.fieldOfView;   
        }
    }
    void Update()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }

    public void requestCameraFOVChange(float requestedFOV)
    {
        if (!isAiming)
        {
            targetFOV = requestedFOV;   
        }
    }

    public void requestCameraFOVForAiming(float requestedFOV)
    {
        targetFOV = requestedFOV;
    }
}
