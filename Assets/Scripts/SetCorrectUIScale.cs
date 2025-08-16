using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class SetCorrectUIScale : MonoBehaviour
{
    [SerializeField] private float size = 7.2f;

    void Start()
    {
        transform.localScale = new Vector3(size, size, size);
    }
}

