using UnityEngine;

public class CancelInputRebind : MonoBehaviour
{
    public static event System.Action OnCancelRebind;

    public void CancelRebinding()
    {
        OnCancelRebind?.Invoke();
    }

}
