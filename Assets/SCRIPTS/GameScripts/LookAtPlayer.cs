using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private bool setXRotationToZero = false;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = playerTransform.position - transform.position;
        
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 EulerAngles = lookRotation.eulerAngles;
        if (setXRotationToZero) EulerAngles.x = 0f;
        else EulerAngles.x = -20f;
        EulerAngles.z = 0f;
        transform.rotation = Quaternion.Euler(EulerAngles);
    }
}
