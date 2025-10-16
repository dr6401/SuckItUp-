using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputRebindingScroll : MonoBehaviour
{
    [SerializeField] private InputActionReference actionToRebindWithButton;
    [SerializeField] private InputActionReference actionToRebindWithScroll;
    [SerializeField] private int bindingIndex = 0;
    [SerializeField] private int bindingIndexForScrollAction = 0;
    [SerializeField] private Button rebindingButton;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject keyConformation;
    [SerializeField] private bool canMouseBtnBeUsedToRebind = false;

    private InputActionRebindingExtensions.RebindingOperation currentRebind;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateButtonText();
        LoadSavedBinding();
    }

    private void UpdateButtonText()
    {
        if (actionToRebindWithButton != null)
        {
            var path = actionToRebindWithButton.action.bindings[bindingIndex].effectivePath;
            buttonText.text =
                InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }

    public void RebindInput()
    {
        if (actionToRebindWithButton == null) return;
        if (currentRebind != null) return; // already rebinding

        actionToRebindWithButton.action.Disable();
        rebindingButton.interactable = false;
        buttonText.text = "Press a key...";

        Debug.Log(
            $"Default binding for action {actionToRebindWithButton.action.name}: {InputControlPath.ToHumanReadableString(actionToRebindWithButton.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice)}");
        
        currentRebind = actionToRebindWithButton.action.PerformInteractiveRebinding(bindingIndex);
            if (!canMouseBtnBeUsedToRebind) currentRebind.WithControlsExcluding("Mouse"); // Disable Mouse rebinding if button shouldn't have it
            
            currentRebind
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                FinishedRebind();
            });
            currentRebind.Start();
    }

    private void FinishedRebind()
    {
        if (currentRebind == null) return;

        var control = currentRebind.selectedControl;

        if (control.layout == "Button")
        {
            
        }
        
        currentRebind.Dispose();
        currentRebind = null;
        
        actionToRebindWithButton.action.Enable();
        rebindingButton.interactable = true;

        UpdateButtonText();
        
        PlayerPrefs.SetString(actionToRebindWithButton.action.name + "_binding" + bindingIndex,
            actionToRebindWithButton.action.bindings[bindingIndex].overridePath);
        Debug.Log($"\n New binding for action {actionToRebindWithButton.action.name}: {InputControlPath.ToHumanReadableString(actionToRebindWithButton.action.bindings[bindingIndex].overridePath, InputControlPath.HumanReadableStringOptions.OmitDevice)}");
        keyConformation.SetActive(false);
    }

    
    private void CancelRebindingProcess()
    {
        if (currentRebind != null)
        {
            currentRebind.Cancel();
            currentRebind.Dispose();
            currentRebind = null;
            Debug.Log("Rebinding cancelled");
            
            actionToRebindWithButton.action.Enable();
            rebindingButton.interactable = true;
            keyConformation.SetActive(false);
            UpdateButtonText();
        }
    }
    public void LoadSavedBinding()
    {
        string saved = PlayerPrefs.GetString(actionToRebindWithButton.action.name + "_binding" + bindingIndex, "");
        if (!string.IsNullOrEmpty(saved))
        {
            actionToRebindWithButton.action.ApplyBindingOverride(bindingIndex, saved);
            UpdateButtonText();
        }
    }
    

    private void OnEnable()
    {
        CancelInputRebind.OnCancelRebind += CancelRebindingProcess;
        rebindingButton.onClick.AddListener(RebindInput);
        Debug.Log($"Added function RebindInput on button {rebindingButton.name} for action {actionToRebindWithButton.action.name}");
    }

    private void OnDisable()
    {
        CancelInputRebind.OnCancelRebind -= CancelRebindingProcess;
        rebindingButton.onClick.RemoveListener(RebindInput);
    }
}
