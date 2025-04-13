using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    private float objectiveTextDuration = 7.5f;
    [SerializeField] private GameObject objectiveText;
    [SerializeField] private GameObject keyBindingsText;
    [FormerlySerializedAs("toggleWeaponText")] [SerializeField] public GameObject toggleWeaponTextObject;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WeaponHandler weaponHandler;
    private bool keyBindingTextToggled = false;
    private bool firstTimeTutorial = true;
    
    private int maxNumberOfDust;
    public int aliveDustParticles;
    List<GameObject> dustParticles = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        
        if (Input.GetKeyDown(KeyCode.Escape)){
            keyBindingTextToggled = !keyBindingTextToggled;
            keyBindingsText.SetActive(keyBindingTextToggled);

            objectiveText.SetActive(!keyBindingTextToggled);
            toggleWeaponTextObject.SetActive(false);

            Time.timeScale = keyBindingTextToggled ? 0f : 1f;
            playerMovement.inputBlocked = keyBindingTextToggled;
            weaponHandler.inputBlocked = keyBindingTextToggled;
        }

        if (Input.GetKeyDown(KeyCode.Q) && firstTimeTutorial)
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
        toggleWeaponTextObject.GetComponent<TMP_Text>().text = "Hold Left Click with Vacuum 3000 to suck up dust. Sucking dust up fills your ammo";
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
}