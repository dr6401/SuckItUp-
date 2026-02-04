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
    private GameManager gameManager;

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager")?.GetComponent<SoundManager>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        weaponHandler = FindFirstObjectByType<WeaponHandler>(); // Cache this once, avoid every frame
        if (gameManager == null)
        {
            gameManager = GameObject.FindAnyObjectByType<GameManager>();
        }
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

        if ((transform.position - target.position).sqrMagnitude < minGetSuckedUpDistance * minGetSuckedUpDistance && Time.timeScale >= 1f) // use .sqrMagnitude to bypass calculating sqrRoot of target.position and transform.position (operation .magnitude would need to calculate that)
        {
            weaponHandler?.RefillAmmo(1);
            soundManager?.PlayDustSuction();
            GameEvents.OnSuckDust?.Invoke();
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.DustDestroyed(this.gameObject);   
        }
    }

}
