using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]private float objectiveTextDuration = 10;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject settingsCanvas;
    [FormerlySerializedAs("toggleWeaponText")] [SerializeField] public GameObject toggleWeaponTextObject;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    private bool keyBindingTextToggled = false;
    private bool firstTimeTutorial = true;
    private PlayerControls controls;
    
    private int maxNumberOfDust;
    public int aliveDustParticles;
    List<GameObject> dustParticles = new List<GameObject>();

    public bool hasInputBeenGranted = true;

    private void Awake()
    {
        controls = SettingsManager.controls;
    }

    void Start()
    {
        objectiveText.SetActive(false);

        GameObject[] dustParticlesInRoom = GameObject.FindGameObjectsWithTag("DustPickup");
        foreach(GameObject dustParticle in dustParticlesInRoom)
        {
            if (!dustParticles.Contains(dustParticle))
            {
                dustParticles.Add(dustParticle.gameObject);
            }
        }
        maxNumberOfDust = dustParticles.Count;
        aliveDustParticles = maxNumberOfDust;
        
        toggleWeaponTextObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] dustParticlesInRoom = GameObject.FindGameObjectsWithTag("DustPickup");
        foreach(GameObject dustParticle in dustParticlesInRoom)
        {
            if (!dustParticles.Contains(dustParticle))
            {
                dustParticles.Add(dustParticle.gameObject);
            }
        }
        
        if (controls.GameEvents.PauseGame.triggered && hasInputBeenGranted){
            keyBindingTextToggled = !keyBindingTextToggled;
            settingsCanvas.SetActive(keyBindingTextToggled);

            objectiveText.SetActive(!keyBindingTextToggled);
            toggleWeaponTextObject.SetActive(false);
            
            //Enabling/Disabling the cursor if the game is paused
            if (keyBindingTextToggled)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            Time.timeScale = keyBindingTextToggled ? 0f : 1f;
            playerMovement.inputBlocked = keyBindingTextToggled;
            weaponHandler.inputBlocked = keyBindingTextToggled;
        }

        if (controls.Player.SwitchWeapon.triggered && firstTimeTutorial)
        {
            firstTimeTutorial = false;
            StartCoroutine(DisplayVacuumTutorialText());
        }
        
        aliveDustParticles = dustParticles.Count;
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(objectiveTextDuration);
        objectiveText.SetActive(false);
    }
    
    private IEnumerator DisplayVacuumTutorialText()
    {
        toggleWeaponTextObject.GetComponent<TMP_Text>().text = $"Hold Left Click with Vacuum 3000 to suck up dust. Sucking dust up fills your ammo";
        yield return new WaitForSeconds(objectiveTextDuration);
        toggleWeaponTextObject.SetActive(false);
        objectiveText.SetActive(true);
        StartCoroutine(DisableText());
    }

    public void DustDestroyed(GameObject dust)
    {
        if (dustParticles.Contains(dust))
        {
            dustParticles.Remove(dust);
            aliveDustParticles = dustParticles.Count;
        }
    }

    public void SetObjectiveTextVisible()
    {
        objectiveText.SetActive(true);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.GameEvents.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.GameEvents.Disable();
    }
}