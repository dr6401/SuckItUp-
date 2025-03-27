using UnityEngine;

public class DustPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Transform target;
    private float baseDustParticlesMoveSpeed = 0.5f;
    private float moveSpeed;
    public bool isGettingSucked; //winky face
    private float accelerationFactor = 1.05f;
    private float minGetSuckedUpDistance = 1f;
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && isGettingSucked)
        {
            moveSpeed *= accelerationFactor;

            transform.position = Vector3.Slerp(transform.position, target.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < minGetSuckedUpDistance)
            {
                FindFirstObjectByType<WeaponHandler>().RefillAmmo(1);
                Destroy(gameObject);
            }
        }

        else
        {
            moveSpeed = baseDustParticlesMoveSpeed;
        }
    }
}
