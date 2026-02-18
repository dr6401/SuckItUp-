using System;
using System.Collections;
using System.Numerics;
using System.Xml.Schema;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

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
    [SerializeField] private Rigidbody rb;
    
    // Vortexy animation look
    //Vector3 swirlAxis = Vector3.up;
    //private float swirlAngle;
    //private float swirlSpeed = 180;


    [Header("Explosion Damping")]
    [SerializeField] private AnimationCurve dampingCurve;
    [SerializeField] private float dampingDuration = 2f;
    [SerializeField] private float linearDampingTarget = 15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager")?.GetComponent<SoundManager>();
        target = GameObject.FindGameObjectWithTag("EndOfSuctionWeapon")?.transform;
        weaponHandler = FindFirstObjectByType<WeaponHandler>(); // Cache this once, avoid every frame
        if (gameManager == null)
        {
            gameManager = GameObject.FindAnyObjectByType<GameManager>();
        }

        if (rb != null)
        {
            rb.useGravity = false;
            StartCoroutine(UseGravityAfterExplosion());   
        }
        /*int swirlDirection = Random.Range(0, 3);
        if (swirlDirection == 0)
        {
            swirlAxis = Vector3.up;
        }
        else if (swirlDirection == 1)
        {
            swirlAxis = Vector3.down;
        }
        else if (swirlDirection == 2)
        {
            swirlAxis = Vector3.left;
        }
        else if (swirlDirection == 3)
        {
            swirlAxis = Vector3.right;
        }*/
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
        //swirlAngle += swirlSpeed * Time.deltaTime;
        //Vector3 direction = (target.position - transform.position).normalized;
        //Vector3 rotatingAxis = Quaternion.AngleAxis(swirlAngle, direction) * Vector3.down;
        //Vector3 swirl = Vector3.Cross(direction, rotatingAxis);// * 5f;
        //Vector3 velocity = (direction + 1f * swirl).normalized * moveSpeed;
        //transform.position += velocity * Time.deltaTime;
        transform.position = Vector3.Slerp(transform.position, target.position, moveSpeed * Time.deltaTime);

        if ((transform.position - target.position).sqrMagnitude < minGetSuckedUpDistance * minGetSuckedUpDistance && Time.timeScale >= 1f) // use .sqrMagnitude to bypass calculating sqrRoot of target.position and transform.position (operation .magnitude would need to calculate that)
        {
            weaponHandler?.RefillAmmo(1);
            soundManager?.PlayDustSuction();
            GameEvents.OnSuckDust?.Invoke();
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.DespawnDustParticle(gameObject);   
            }
            else Destroy(gameObject);
        }
    }

    private IEnumerator UseGravityAfterExplosion()
    {
        float timeBeforeApplyGravity = 0f;
        float startDamping = rb.linearDamping;

        while (timeBeforeApplyGravity <= dampingDuration)
        {
            timeBeforeApplyGravity += Time.deltaTime;
            float t = Mathf.Clamp01(timeBeforeApplyGravity / dampingDuration);
            float curve = dampingCurve.Evaluate(t);
            rb.linearDamping = Mathf.Lerp(startDamping, linearDampingTarget, curve);
            //if (curve >= 0.8f) rb.useGravity = true;
            yield return null;
        }
        rb.linearDamping = linearDampingTarget;
        rb.useGravity = true;
    }
    
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.DustDestroyed(this.gameObject);   
        }
    }

}
