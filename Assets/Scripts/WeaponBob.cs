using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.XR;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WeaponBob : MonoBehaviour
{

    private Vector3 defaultPosition;
    private UnityEngine.Quaternion defaultRotation;
    [SerializeField] private float yFrequency = 2f;
    [SerializeField] private float xFrequency = 1f;
    [SerializeField] private float amplitude = 0.01f;
    private WeaponHandler weaponHandler;
    private float isAimingPositionFactor = 1; // Factor that is set if the player is Aiming
    private float isAimingRotationFactor = 1;
    private float isAimingRotationStepFactor = 1; // to decrease the step (max swing distance) when aiming
    
    // External script stuff
    private Vector2 lookInput;
    public float rotationStep = 6f;
    public float maxRotationStep = 7f;
    Vector3 swayEulerRot; 
    public float smoothRot = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPosition = transform.localPosition;
        weaponHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponHandler>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        GetInput();
        SwayRotation();
        Bob();
        Debug.Log("Smooth rot: " + smoothRot);
    }

    private void Bob()
    {
        if (weaponHandler.isAiming)
        {
            isAimingPositionFactor = 0.0f; // If player is Aiming reduce the bobbing position sensitivity
            isAimingRotationFactor = 0.4f; // If player is Aiming reduce the bobbing rotation sensitivity
            isAimingRotationStepFactor = 0.5f;
        }
        else
        {
            isAimingPositionFactor = 1f; // If not aiming, ignore this factor (set it to 1)
            isAimingRotationFactor = 1f; // If player is Aiming reduce the bobbing rotation sensitivity
            isAimingRotationStepFactor = 1f;
        }
        
        HandlePosition();
        HandleRotation();
    }

    private void HandlePosition()
    {
        transform.localPosition = defaultPosition + 
                                  Vector3.up * Mathf.Sin(Time.time * yFrequency) * amplitude * isAimingPositionFactor +
                                  Vector3.right * Mathf.Sin(Time.time * xFrequency) * amplitude * isAimingPositionFactor;
    }
    private void HandleRotation()
    {
        transform.localRotation = UnityEngine.Quaternion.Slerp(transform.localRotation, UnityEngine.Quaternion.Euler(swayEulerRot), Time.deltaTime * smoothRot * isAimingRotationFactor);
    }

    void GetInput(){
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
    }
    
    void SwayRotation(){
        Vector2 invertLook =  lookInput * -rotationStep * isAimingRotationFactor * isAimingRotationStepFactor;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }
}