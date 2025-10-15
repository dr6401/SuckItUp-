using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class InputRebinding : MonoBehaviour
{
    [SerializeField] private InputActionReference actionToRebind;
    [SerializeField] private int bindingIndex = 0;
    [SerializeField] private Button rebindingButton;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject keyConformation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateButtonText();
        LoadSavedBinding();
    }

    private void UpdateButtonText()
    {
        if (actionToRebind != null)
        {
            var path = actionToRebind.action.bindings[bindingIndex].effectivePath;
            buttonText.text =
                InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }

    public void RebindInput()
    {
        if (actionToRebind == null) return;

        actionToRebind.action.Disable();
        rebindingButton.interactable = false;
        Debug.Log(
            $"Default binding for action {actionToRebind.action.name}: {InputControlPath.ToHumanReadableString(actionToRebind.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice)}");
        
        actionToRebind.action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                operation.Dispose();
                rebindingButton.interactable = true;
                actionToRebind.action.Enable();
                UpdateButtonText();
                PlayerPrefs.SetString(actionToRebind.action.name + "_binding" + bindingIndex,
                    actionToRebind.action.bindings[bindingIndex].overridePath);
                Debug.Log($"\n New binding for action {actionToRebind.action.name}: {InputControlPath.ToHumanReadableString(actionToRebind.action.bindings[bindingIndex].overridePath, InputControlPath.HumanReadableStringOptions.OmitDevice)}");
                keyConformation.SetActive(false);
            })
            .Start();
    }

    public void LoadSavedBinding()
    {
        string saved = PlayerPrefs.GetString(actionToRebind.action.name + "_binding" + bindingIndex, "");
        if (!string.IsNullOrEmpty(saved))
        {
            actionToRebind.action.ApplyBindingOverride(bindingIndex, saved);
            UpdateButtonText();
        }
    }

}
