using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputRebinding : MonoBehaviour
{
    [SerializeField] private string actionName;
    [SerializeField] private int bindingIndex = 0;
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

    private InputAction GetAction()
    {
        foreach (var map in SettingsManager.controls.asset.actionMaps)
        {
            var action = map.FindAction(actionName);
            if (action != null) return action;
        }
        Debug.Log($"No actions with name {actionName} found in InputMaps");
        return null;
    }

    private void UpdateButtonText()
    {
        var action = GetAction();
        if (action != null)
        {
            var path = action.bindings[bindingIndex].effectivePath;
            buttonText.text =
                InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }

    public void RebindInput()
    {
        var action = GetAction();
        if (action == null) return;
        if (currentRebind != null) return; // already rebinding

        action.Disable();
        rebindingButton.interactable = false;
        buttonText.text = "Press a key...";

        Debug.Log(
            $"Default binding for action {action.name}: {InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice)}");
        
        currentRebind = action.PerformInteractiveRebinding(bindingIndex);
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
        var action = GetAction();
        if (currentRebind == null || action == null) return;
        currentRebind.Dispose();
        currentRebind = null;
        
        action.Enable();
        rebindingButton.interactable = true;

        UpdateButtonText();
        
        PlayerPrefs.SetString(action.name + "_binding" + bindingIndex,
            action.bindings[bindingIndex].overridePath);
        Debug.Log($"\n New binding for action {action.name}: {InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].overridePath, InputControlPath.HumanReadableStringOptions.OmitDevice)}");
        keyConformation.SetActive(false);
    }

    
    private void CancelRebindingProcess()
    {
        var action = GetAction();
        if (currentRebind != null && action != null)
        {
            currentRebind.Cancel();
            currentRebind.Dispose();
            currentRebind = null;
            Debug.Log("Rebinding cancelled");
            
            action.Enable();
            rebindingButton.interactable = true;
            keyConformation.SetActive(false);
            UpdateButtonText();
        }
    }
    public void LoadSavedBinding()
    {
        var action = GetAction();
        string saved = PlayerPrefs.GetString(action.name + "_binding" + bindingIndex, "");
        if (!string.IsNullOrEmpty(saved))
        {
            action.ApplyBindingOverride(bindingIndex, saved);
            UpdateButtonText();
        }
    }
    

    private void OnEnable()
    {
        //var action = GetAction();
        CancelInputRebind.OnCancelRebind += CancelRebindingProcess;
        rebindingButton.onClick.AddListener(RebindInput);
        Debug.Log($"Added function RebindInput on button {rebindingButton.name} for action {actionName}");
    }

    private void OnDisable()
    {
        CancelInputRebind.OnCancelRebind -= CancelRebindingProcess;
        rebindingButton.onClick.RemoveListener(RebindInput);
    }
}
