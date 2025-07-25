using UnityEngine;

public class DustPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Transform target;
    private float baseMoveSpeed = 0.5f;
    private float moveSpeed;
    public bool isGettingSucked; //winky face
    private float accelerationFactor = 1.05f;
    private float minGetSuckedUpDistance = 1f;
    private SoundManager soundManager;
    private WeaponHandler weaponHandler;

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager")?.GetComponent<SoundManager>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        weaponHandler = FindFirstObjectByType<WeaponHandler>(); // Cache this once, avoid every frame
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        if (!isGettingSucked)
        {
            moveSpeed = baseMoveSpeed;
            return;
        }

        moveSpeed *= accelerationFactor;
        transform.position = Vector3.Slerp(transform.position, target.position, moveSpeed * Time.deltaTime);

        if ((transform.position - target.position).sqrMagnitude < minGetSuckedUpDistance * minGetSuckedUpDistance)
        {
            weaponHandler?.RefillAmmo(1);
            soundManager?.PlayDustSuction();
            Destroy(gameObject);
        }
    }

}
