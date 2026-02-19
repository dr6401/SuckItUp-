using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToOpenTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    [SerializeField] private MMFeedbacks openFeedbacksTooltip;
    public void OnPointerEnter(PointerEventData eventData)
    {
        openFeedbacksTooltip?.PlayFeedbacks();
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
