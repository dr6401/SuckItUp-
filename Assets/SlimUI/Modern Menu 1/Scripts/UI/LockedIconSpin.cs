using UnityEngine;

public class LockedIconSpin : MonoBehaviour
{

    private float rotateSpeed = 50f;
    
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
