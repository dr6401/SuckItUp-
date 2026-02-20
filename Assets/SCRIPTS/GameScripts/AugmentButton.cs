using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AugmentButton : MonoBehaviour
{
    [SerializeField] private TMP_Text augmentName;
    [SerializeField] private TMP_Text augmentDescription;
    [SerializeField] private Image iconImage;

    private Augment augment;
    private GameObject player;
    private AugmentSelectionUI augmentSelectionUI;

    
    void Start()
    {
        if (augmentName == null)
        {
            augmentName = GetComponentInChildren<TMP_Text>();
        }
        if (iconImage == null)
        {
            iconImage = GetComponent<Image>();
        }
    }
    public void Setup(Augment aug, GameObject playerRef, AugmentSelectionUI parentUI)
    {
        augment = aug;
        player = playerRef;
        augmentSelectionUI = parentUI;

        augmentName.text = augment.augmentName;
        augmentDescription.text = augment.description;
        iconImage.sprite = augment.icon;

    }
    
    public void Setup(Augment aug, GameObject playerRef)
    {
        augment = aug;
        player = playerRef;

        augmentName.text = augment.augmentName;
        augmentDescription.text = augment.description;
        iconImage.sprite = augment.icon;

    }

    public void SelectAugment()
    {
        augmentSelectionUI.StoreChosenAugment(augment);
        augment.Apply(player);
        augmentSelectionUI.CloseUI();
        Debug.Log("Selected " + augment.augmentName + "!");
        GameEvents.OnHasSettingsUICoveredUpAugmentUI?.Invoke(false);
    }
}
