using System;
using UnityEngine;

public class ResetScaleOnStart : MonoBehaviour
{
    public RectTransform rect;
    private void Awake()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (rect != null && rect.localScale == Vector3.zero)
        {
            rect.localScale = Vector3.one;
        }
    }
}
