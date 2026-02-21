using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoombaHallway : MonoBehaviour
{
    private bool canBeInteractedWith = false;
    private float maxInteractableDistance = 15;
    private GameObject player;
    private PlayerControls controls;
    private bool isChosenAugmentsUIShown = false;
    private float scrollSensitivity = 25;
    [SerializeField] private GameObject chosenAugmentsCanvas;
    [SerializeField] private TMP_Text pressEToInteractText;
    private Vector3 pressEToInteractOriginalPosition;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform chosenAugmentsDisplay;
    [SerializeField] private GameObject noAugmentsText;
    [SerializeField] private RunAugmentData runAugmentData;
    public GameObject augmentButtonPrefab;
    private float blockInputTimer;
    private float blockInputInterval = 0.25f;



    private void Awake()
    {
        controls = SettingsManager.controls;
    }
    
    void Start()
    {
        pressEToInteractOriginalPosition = pressEToInteractText.gameObject.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(PopulateAugmentsDisplay());
        scrollRect.scrollSensitivity = scrollSensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        string interactKey =
            controls.Player.Interact.GetBindingDisplayString(options: InputBinding.DisplayStringOptions
                .DontIncludeInteractions)[0].ToString();
        if (IsPlayerNearEnoughToInteract() && !isChosenAugmentsUIShown)
        {
            pressEToInteractText.gameObject.transform.position = pressEToInteractOriginalPosition;
            pressEToInteractText.gameObject.SetActive(true);
            pressEToInteractText.text = $"Press {interactKey} to show Augments";
            if (controls.Player.Interact.WasPressedThisFrame() && !isChosenAugmentsUIShown)
            {
                pressEToInteractText.gameObject.transform.position = pressEToInteractOriginalPosition - new Vector3(0, 90, 0);
                pressEToInteractText.text = $"Press {interactKey} to close";    
                ShowChosenAugmentsUI();
            }
        }
        else if (!IsPlayerNearEnoughToInteract())
        {
            pressEToInteractText.gameObject.SetActive(false);
            CloseChosenAugmentsUI();
        }
        else if (isChosenAugmentsUIShown)
        {
            float scrollInput = controls.UI.ScrollWheel.ReadValue<Vector2>().y;
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                scrollRect.verticalNormalizedPosition += scrollInput * scrollSensitivity * Time.deltaTime; 
            }
            if (controls.Player.Interact.WasPressedThisFrame())
            {
                CloseChosenAugmentsUI();
            }
        }
    }

    private void ShowChosenAugmentsUI()
    {
        isChosenAugmentsUIShown = true;
        chosenAugmentsCanvas?.SetActive(true);
    }

    private void CloseChosenAugmentsUI()
    {
        isChosenAugmentsUIShown = false;
        chosenAugmentsCanvas?.SetActive(false);
    }

    private bool IsPlayerNearEnoughToInteract()
    {
        //Debug.Log($"Is player close enough to interact: {Vector3.Distance(player.transform.position, transform.position) <= maxInteractableDistance}");
        return Vector3.Distance(player.transform.position, transform.position) <= maxInteractableDistance;
    }

    private IEnumerator PopulateAugmentsDisplay()
    {
        yield return new WaitForSeconds(1f);
        foreach (var choice in runAugmentData.chosenAugments)
        {
            Debug.Log("Given you the choice: " + choice.augmentName);
            var btnObj = Instantiate(augmentButtonPrefab, chosenAugmentsDisplay);
            var btnObjScript = btnObj.GetComponent<AugmentButton>();
            btnObjScript.Setup(choice, player);
            var btnObjButton = btnObj.GetComponent<Button>();
            btnObjButton.interactable = false;
        }

        if (runAugmentData.chosenAugments.Count <= 0)
        {
            noAugmentsText.gameObject.SetActive(true);
        }
        else noAugmentsText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UI.Disable();
    }
}
