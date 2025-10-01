using UnityEngine;

public class PropellerFlyingDusty : MonoBehaviour
{
    [SerializeField] private float propellerSpeed = 2500f; 

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (propellerSpeed * Time.deltaTime));
    }
}
