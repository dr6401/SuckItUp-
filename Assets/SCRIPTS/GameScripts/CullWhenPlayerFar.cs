using TMPro;
using UnityEngine;

public class CullWhenPlayerFar : MonoBehaviour
{
    [SerializeField] private float distance = 50f;
    private Transform playerTransform;
    private TMP_Text tmp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        tmp = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("sdkfh");
        if (playerTransform == null) return;
        //Debug.Log($"Distance between player and {name}: {Vector3.Distance(transform.position, playerTransform.position)}");
        if (Vector3.Distance(transform.position, playerTransform.position) <= distance)
        {
            tmp.enabled = true;
        }
        else
        {
            tmp.enabled = false;
        }
    }
}
