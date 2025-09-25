using UnityEngine;

public class PropellerFlyingDusty : MonoBehaviour
{
    [SerializeField] private float propellerSpeed = 20000f; 

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * (propellerSpeed * Time.deltaTime));
    }
}
