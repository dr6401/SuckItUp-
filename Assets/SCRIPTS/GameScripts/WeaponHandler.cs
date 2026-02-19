using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using System;
using DamageNumbersPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WeaponHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool isDifficultyHardcore;
    public GameObject shooterWeapon;
    public GameObject vacuumWeapon;
    private bool isShooterWeaponActive = true;
    private bool isVacuumWeaponActive;
    [SerializeField] private float shooterWeaponDamage = 5f;
    private float shootingRange = 100f;
    private float vacuumRange = 10f;
    [SerializeField] private float fireRate = 0.2f;
    private float timeSinceLastShot = 0;
    [SerializeField] private Camera camera;
    
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject enemyHitEffectPrefab;
    [SerializeField] private DamageNumber hitDamageNumber;
    
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform endOfShooterWeaponBarrel;
    public RawImage crossHair;
    private int startingAmmo;
    [SerializeField] private int normalStartingAmmo = 0;
    [SerializeField] private int hardcoreStartingAmmo = 50;
    private int currentAmmo;
    [SerializeField] private TMP_Text ammoText;
    public Animator primaryWeaponAnimator;
    [SerializeField] private float recoilAmount = 0.1f;
    public bool isAiming;
    public bool inputBlocked = false;
    private float notAminingCameraFOV = 80f;
    private float aimingCameraFOVMultiplier = 0.6f;
    private float minAimingFOV = 45f;
    private float maxAimingFOV = 60f;

    private float targetFOV;

    //private float zoomSpeed = 6f;
    public bool isAlreadySucking;

    private PlayerHealth playerHealth;
    private PlayerControls controls;
    private bool canWeaponSwitchWithScroll = true;
    private float timePassedSinceLastWeaponSwitch = 0f;
    private float weaponSwitchCooldown = 0.2f;

    private SoundManager soundManager;
    public CameraFOVController cameraFOVController;

    public static event Action OnAmmoIncrease;
    public static event Action OnNoAmmoLeft;
    public static event Action OnHealthIncrease;
    // Update is called once per frame

    [Header("Augment Stuff")]
    // AUGMENT STUFF
    private bool isVampire = false;
    private bool isAmmoRecyclerEnabled = false;
    private int chanceToRecycleAmmoThreshold = 20; // 20 is default, this is set by augmentScript anyway
    private bool isOverchargedVacuumEnabled = false;
    private float overchargedVacuumDuration = 3f;
    private float overchargedVacuumFireRate;
    private float overchargedVacuumFireRateMultiplier = 1.75f;
    private Coroutine overchargedVacuumCoroutine;
    private float nonOverchargedFireRate = 0.2f; // Set this to fireRate everytime it gets permanently changed (so like with MinigunMayhem, not with OverChargedVacuum)
    private bool isDuststormShellEnabled = false;
    [SerializeField] private GameObject shieldVFXObject;
    private bool isAmmoNationEnabled = false;
    private float ammoNationConversionIncrease = 1;

    private void Awake()
    {
        controls = SettingsManager.controls;
    }

    private void Start()
    {
        vacuumWeapon.SetActive(false);
        isVacuumWeaponActive = false;
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        crossHair.enabled = false;
        notAminingCameraFOV = PlayerPrefs.GetFloat("FOV", GameConstants.defaultFOV);
        if (playerHealth == null)
        {
            playerHealth = GetComponentInChildren<PlayerHealth>();
        }

        isDifficultyHardcore = SettingsManager.Instance.isDifficultyHardcore;
        if (!isDifficultyHardcore) startingAmmo = normalStartingAmmo;
        else startingAmmo = hardcoreStartingAmmo;
        canWeaponSwitchWithScroll = SettingsManager.Instance.isWeaponSwitchWithScrollEnabled;
        currentAmmo = startingAmmo;


        if (endOfShooterWeaponBarrel == null)
        {
            endOfShooterWeaponBarrel = GetComponentInChildren<Transform>().Find("EndOfBarrel");
        }

        if (soundManager == null)
        {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        }
    }

    void Update()
    {
        if (!inputBlocked)
        {
            timeSinceLastShot += Time.deltaTime;
            timePassedSinceLastWeaponSwitch += Time.deltaTime;

            // Shoot    
            if (isShooterWeaponActive && controls.Player.Shoot.ReadValue<float>() > 0 && timeSinceLastShot >= fireRate)
            {
                if (currentAmmo <= 0)
                {
                    OnNoAmmoLeft?.Invoke();
                }
                else
                {
                    Shoot();
                    timeSinceLastShot = 0;
                }
            }
            // Suck

            if (!isShooterWeaponActive && controls.Player.Shoot.ReadValue<float>() > 0)
            {
                Vacuum();
                if (isDuststormShellEnabled)
                {
                    shieldVFXObject.SetActive(true);
                }
            }
            else
            {
                shieldVFXObject.SetActive(false);
                StopSuckingDustParticles();
                if (isAlreadySucking)
                {
                    soundManager.PlayEndVacuuming();
                    GameEvents.OnStopSuckingDust?.Invoke();
                }
                isAlreadySucking = false;
            }

            // Aim
            if (controls.Player.Aim.IsPressed() && isShooterWeaponActive)
            {
                if (!isAiming)
                {
                    isAiming = true;
                    StartCoroutine(EnableSightsWhenAiming());
                }
                else
                {
                    crossHair.enabled = true;
                }

                primaryWeaponAnimator.SetBool("IsAiming", true);

            }
            else
            {
                isAiming = false;
                crossHair.enabled = false;
                primaryWeaponAnimator.SetBool("IsAiming", false);
            }

            //Changing FOV for aiming transition
            targetFOV = isAiming
                ? Mathf.Clamp(notAminingCameraFOV * aimingCameraFOVMultiplier, minAimingFOV, maxAimingFOV)
                : notAminingCameraFOV;
            cameraFOVController.RequestCameraFOVForAiming(targetFOV);

        }
        else // If input is blocked
        {
            StopSuckingDustParticlesWhenInputBlocked();
        }

        ammoText.text = currentAmmo.ToString();
    }

    private void ShootOrVacuum(InputAction.CallbackContext context)
    {

    }

    private void Shoot()
    {
        int layerMask = ~LayerMask.GetMask("Player", "Projectile", "PlayerHitBox", "PlayerHeadHitBox", "Dust", "RenderBehindWalls");

        GameEvents.OnShoot?.Invoke();

        RaycastHit hit;
        Vector3 shootOrigin = camera.transform.position;
        Vector3 shootDirection = camera.transform.forward;
        if (!isAiming)
        {
            shootDirection = (shootDirection + new Vector3(
                Random.Range(-recoilAmount, recoilAmount),
                Random.Range(-recoilAmount, recoilAmount),
                Random.Range(-recoilAmount, recoilAmount)
            )).normalized;
        }

        Instantiate(muzzleFlashPrefab, endOfShooterWeaponBarrel.position + endOfShooterWeaponBarrel.forward * 0.2f + endOfShooterWeaponBarrel.up * -0.025f,
            endOfShooterWeaponBarrel.rotation, endOfShooterWeaponBarrel);

        //Debug.Log("Shooting!");

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootingRange, layerMask))
        {
            //Debug.Log("hit " + hit.collider.gameObject.name + "!");

            if (hit.collider.tag == "Enemy" || hit.collider.tag == "EnemySpawner")
            {
                GameEvents.OnHit?.Invoke();
                StartCoroutine(CrossHairColourToggle());
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(shooterWeaponDamage);
                Instantiate(enemyHitEffectPrefab, hit.point, Quaternion.identity);
                hitDamageNumber.Spawn(hit.point, shooterWeaponDamage);
            }
            else
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
            }
        }

        currentAmmo--;
    }

    private void Vacuum()
    {
        if (!isAlreadySucking)
        {
            soundManager.PlayStartVacuuming();
            isAlreadySucking = true;
            GameEvents.OnStartSuckingDust?.Invoke();
        }

        SuckDustParticlesIn(true);
    }

    public void RefillAmmo(int reloadAmount)
    {
        if (isVampire && currentAmmo >= 100 && !playerHealth.IsPlayerAtMaxHealth())
        {
            OnHealthIncrease?.Invoke();
        }

        else if (reloadAmount > 0)
        {
            OnAmmoIncrease?.Invoke();
            if (isAmmoNationEnabled) reloadAmount = Mathf.RoundToInt(reloadAmount * ammoNationConversionIncrease);
            currentAmmo += reloadAmount;
        }

        if (isAlreadySucking && isOverchargedVacuumEnabled) // Overcharging should happen regardless of DirtyVampire being active
        {
            if (overchargedVacuumCoroutine != null)
            {
                StopCoroutine(overchargedVacuumCoroutine);
                Debug.Log($"overchargedVacuumCoroutine was active, so I stopped it and gonna start a new one");
            }
            Debug.Log($"Current fireRate: {fireRate}, overchargedFireRate: {overchargedVacuumFireRate}");
            overchargedVacuumCoroutine = StartCoroutine(OverchargeVacuum(overchargedVacuumDuration, overchargedVacuumFireRate));
        }
    }


    private void WeaponSwitch(InputAction.CallbackContext context)
    {
        if (inputBlocked) return;
        isShooterWeaponActive = !isShooterWeaponActive;
        isVacuumWeaponActive = !isVacuumWeaponActive;
        shooterWeapon.SetActive(isShooterWeaponActive);
        vacuumWeapon.SetActive(isVacuumWeaponActive);
    }

    private void WeaponSwitchWithScroll(InputAction.CallbackContext context)
    {
        if (timePassedSinceLastWeaponSwitch >= weaponSwitchCooldown)
        {
            if (inputBlocked || !canWeaponSwitchWithScroll) return;
            timePassedSinceLastWeaponSwitch = 0f;
            isShooterWeaponActive = !isShooterWeaponActive;
            isVacuumWeaponActive = !isVacuumWeaponActive;
            shooterWeapon.SetActive(isShooterWeaponActive);
            vacuumWeapon.SetActive(isVacuumWeaponActive);
        }
    }

    private void ToggleWeaponSwitchWithScroll(bool canWeaponSwitchWithScrollBtn)
    {
        canWeaponSwitchWithScroll = canWeaponSwitchWithScrollBtn;
    }

    private IEnumerator EnableSightsWhenAiming()
    {
        float animationTimeForAimSightsToBeEnabled = 0.15f;
        yield return new WaitForSeconds(animationTimeForAimSightsToBeEnabled);
        crossHair.enabled = true;

    }

    private IEnumerator CrossHairColourToggle()
    {
        float colourToggleDuration = 0.1f;
        //Debug.Log("Finna change crosshair colour to green");
        crossHair.color = Color.green;

        yield return new WaitForSeconds(colourToggleDuration);

        crossHair.color = Color.white;
        //Debug.Log("Changed crosshair colour to white");
    }

    private void SuckDustParticlesIn(bool isSucking)
    {
        Collider[]
            dustPickups =
                Physics.OverlapSphere(transform.position, vacuumRange); // vacuumRange = radius in which player will detect if any dusts are going to become suckable

        foreach (Collider dust in dustPickups)
        {
            if (dust.CompareTag("DustPickup"))
            {
                dust.GetComponent<DustPickup>().isGettingSucked = isSucking;
            }
        }
    }

    // When have time refactor this, so that it will only excecute SuckDustParticlesIn(false) once,
    // so it won't need to do the Collider Physics.Overlap checking constantly, but turn it off just once
    private void StopSuckingDustParticles()
    {
        SuckDustParticlesIn(false);
    }

    private void StopSuckingDustParticlesWhenInputBlocked()
    {
        StopSuckingDustParticles();
        if (isAlreadySucking)
        {
            soundManager.PlayEndVacuuming();
            GameEvents.OnStopSuckingDust?.Invoke();
        }

        isAlreadySucking = false;
    }

    public int ReturnCurrentAmmo()
    {
        return currentAmmo;
    }

    public void SetNewFOV(float newFov)
    {
        notAminingCameraFOV = newFov;
    }

    #region Augments

    public void ApplyMinigunMayhem()
    {
        nonOverchargedFireRate = fireRate;
        fireRate = nonOverchargedFireRate / 1.25f;
        nonOverchargedFireRate = fireRate;
        // If OverchargedVacuum is selected before selecting MinigunMayhem, update overchargedVacuumFireRate by using new fireRate for its calculation;
        overchargedVacuumFireRate = fireRate / overchargedVacuumFireRateMultiplier;
    }

    public void ApplyMinigunCarnage()
    {
        nonOverchargedFireRate = fireRate;
        fireRate = nonOverchargedFireRate / 1.5f;
        nonOverchargedFireRate = fireRate;
        // If OverchargedVacuum is selected before selecting MinigunCarnage, update overchargedVacuumFireRate by using new fireRate for its calculation;
        overchargedVacuumFireRate = fireRate / overchargedVacuumFireRateMultiplier;
    }

    public void ApplyHitHarder()
    {
        shooterWeaponDamage *= 1.4f;
    }

    public void ApplyHitHarderer()
    {
        shooterWeaponDamage *= 2f;
    }

    public void ApplyDirtyVampireOrDracula()
    {
        isVampire = true;
    }

    public void ApplyColossalCleaner(float dmgMultiplier)
    {
        shooterWeaponDamage *= dmgMultiplier;
    }

    public void ApplyDustMagnet(float rangeMultiplier)
    {
        vacuumRange *= rangeMultiplier;
    }

    public void ApplyAmmoRecycler(int chanceThreshold)
    {
        isAmmoRecyclerEnabled = true;
        chanceToRecycleAmmoThreshold = chanceThreshold;
    }

    private void ExcecuteAmmoRecycler()
    {
        if (isAmmoRecyclerEnabled)
        {
            int chanceToRecycleAmmo = Random.Range(1, 100);
            if (chanceToRecycleAmmo <= chanceToRecycleAmmoThreshold)
            {
                RefillAmmo(1);
                Debug.Log($"Chance was {chanceToRecycleAmmo}, recycling ammo");
            }
        }
    }

    public void ApplyOverchargedVacuum(float duration, float fireRateMultiplier)
    {
        isOverchargedVacuumEnabled = true;
        overchargedVacuumDuration = duration;
        overchargedVacuumFireRateMultiplier = fireRateMultiplier;
        overchargedVacuumFireRate = fireRate / overchargedVacuumFireRateMultiplier;
        nonOverchargedFireRate = fireRate;
    }

    private IEnumerator OverchargeVacuum(float duration, float overchargedFireRate)
    {
        fireRate = overchargedFireRate;
        Debug.Log($"Setting fireRate to {fireRate} for {duration} seconds");
        yield return new WaitForSeconds(duration);
        fireRate = nonOverchargedFireRate;
        Debug.Log($"Setting fireRate back to {fireRate} (oldFireRate: {nonOverchargedFireRate})");
    }

    public void ApplyDuststormShell()
    {
        isDuststormShellEnabled = true;
    }

    public void ApplyAmmoNation(float conversionRateIncrease)
    {
        isAmmoNationEnabled = true;
        ammoNationConversionIncrease = conversionRateIncrease;
    }

#endregion

    /*private void OnDrawGizmos()
    {
        if (camera == null) return;
            int layerMask = ~LayerMask.GetMask("Player", "Projectile");

        Vector3 shootOrigin = camera.transform.position;
        Vector3 shootDirection = camera.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootingRange, layerMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(shootOrigin, shootDirection * hit.distance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(shootOrigin, shootDirection * shootingRange);
        }
    }*/
    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.SwitchWeapon.performed += WeaponSwitch;
        controls.Player.SwitchWeaponScroll.performed += WeaponSwitchWithScroll;
        GameEvents.OnFOVChanged += SetNewFOV;
        GameEvents.OnPlayerDeath += StopSuckingDustParticlesWhenInputBlocked;
        GameEvents.OnLevelCompleted += StopSuckingDustParticlesWhenInputBlocked;
        SettingsManager.OnWeaponSwitchWithScrollEnabledFromSettingsManager += ToggleWeaponSwitchWithScroll;
        
        // AUGMENTS
        GameEvents.OnEnemyDeath += ExcecuteAmmoRecycler;
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Player.SwitchWeapon.performed -= WeaponSwitch;
        controls.Player.SwitchWeaponScroll.performed -= WeaponSwitchWithScroll;
        GameEvents.OnFOVChanged -= SetNewFOV;
        GameEvents.OnPlayerDeath -= StopSuckingDustParticlesWhenInputBlocked;
        GameEvents.OnLevelCompleted -= StopSuckingDustParticlesWhenInputBlocked;
        SettingsManager.OnWeaponSwitchWithScrollEnabledFromSettingsManager -= ToggleWeaponSwitchWithScroll;
        
        // AUGMENTS
        GameEvents.OnEnemyDeath -= ExcecuteAmmoRecycler;
    }
}
