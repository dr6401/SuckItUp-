using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoombaHallway : MonoBehaviour
{
    private bool canBeInteractedWith = false;
    private float maxInteractableDistance = 15f;
    private GameObject player;
    private PlayerControls controls;
    private bool isChosenAugmentsUIShown = false;
    [SerializeField] private GameObject chosenAugmentsCanvas;
    [SerializeField] private TMP_Text pressEToInteractText;

    private void Awake()
    {
        controls = SettingsManager.controls;
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerNearEnoughToInteract() && !isChosenAugmentsUIShown)
        {
            pressEToInteractText.gameObject.SetActive(true);
            string key =
                controls.Player.Interact.GetBindingDisplayString(options: InputBinding.DisplayStringOptions
                    .DontIncludeInteractions)[0].ToString();
            pressEToInteractText.text = $"Press {key} to Interact";
            if (controls.Player.Interact.ReadValue<float>() > 0 && !isChosenAugmentsUIShown)
            {
                pressEToInteractText.gameObject.SetActive(false);
                ShowChosenAugmentsUI();
            }
        }
        else if (!IsPlayerNearEnoughToInteract())
        {
            pressEToInteractText.gameObject.SetActive(false);
            CloseChosenAugmentsUI();
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
        Debug.Log($"Is player close enough to interact: {Vector3.Distance(player.transform.position, transform.position) <= maxInteractableDistance}");
        return Vector3.Distance(player.transform.position, transform.position) <= maxInteractableDistance;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}
