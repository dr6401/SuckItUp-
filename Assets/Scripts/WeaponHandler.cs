using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WeaponHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject shooterWeapon;
    public GameObject vacuumWeapon;
    private bool isShooterWeaponActive = true;
    private bool isVacuumWeaponActive;
    private float shooterWeaponDamage = 5f;
    private float shootingRange = 100f;
    [SerializeField] private float fireRate = 0.2f;
    private float timeSinceLastShot = 0;
    [SerializeField] private new Camera camera;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject enemyHitEffectPrefab;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform endOfBarrel;
    public RawImage crossHair;
    [SerializeField] private float maxAmmo = 30f;
    private float currentAmmo;
    [SerializeField] private TMP_Text ammoText;
    public Animator primaryWeaponAnimator;
    public bool isAiming;
    public bool inputBlocked = false;
    // Update is called once per frame

    private void Start()
    {
        vacuumWeapon.SetActive(false);
        isVacuumWeaponActive = false;
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        crossHair.enabled = false;
        currentAmmo = maxAmmo;
        if (endOfBarrel == null)
        {
            endOfBarrel = GetComponentInChildren<Transform>().Find("EndOfBarrel");
        }
    }
    void Update()
    {
        if (!inputBlocked)
        {
            timeSinceLastShot += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                WeaponSwitch();
            }
            // Shoot
            if (isShooterWeaponActive && Input.GetMouseButton(0) && timeSinceLastShot >= fireRate && currentAmmo > 0)
            {
                Shoot();
                timeSinceLastShot = 0;
            }
            // Suck

            if (!isShooterWeaponActive && Input.GetMouseButton(0))
            {
                Vacuum();
            }
            else
            {
                StopSuckingDustParticles();
            }

            // Aim
            if (Input.GetMouseButton(1) && isShooterWeaponActive)
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
                //crossHair.enabled = true;
            }
            else
            {
                isAiming = false;
                crossHair.enabled = false;
                primaryWeaponAnimator.SetBool("IsAiming", false);
            }
        }
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }

    private void Shoot()
    {
        int layerMask = ~LayerMask.GetMask("Player");

        RaycastHit hit;
        Vector3 shootOrigin = camera.transform.position;
        Vector3 shootDirection = camera.transform.forward;
        Instantiate(muzzleFlashPrefab, endOfBarrel.position + endOfBarrel.forward * 0.2f + endOfBarrel.up * -0.025f, endOfBarrel.rotation, endOfBarrel);

        //Debug.Log("Shooting!");

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootingRange, layerMask))
        {
            //Debug.Log("hit " + hit.collider.gameObject.name + "!");

            if (hit.collider.tag == "Enemy" || hit.collider.tag == "EnemySpawner")
            {
                StartCoroutine(CrossHairColourToggle());
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(shooterWeaponDamage);
                Instantiate(enemyHitEffectPrefab, hit.point, Quaternion.identity);
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
        SuckDustParticlesIn(true);
    }

    public void RefillAmmo(int reloadAmmount)
    {
        currentAmmo += reloadAmmount;
    }


    private void WeaponSwitch()
    {
        isShooterWeaponActive = !isShooterWeaponActive;
        isVacuumWeaponActive = !isVacuumWeaponActive;
        shooterWeapon.SetActive(isShooterWeaponActive);
        vacuumWeapon.SetActive(isVacuumWeaponActive);
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
        Collider[] dustPickups = Physics.OverlapSphere(transform.position, 10f); // 10f = radius in which player will detect if any dusts are going to become suckable

        foreach (Collider dust in dustPickups)
        {
            if (dust.CompareTag("DustPickup"))
            {
                //Debug.Log("Mf named " + dust.name + " has been detected as a Dusty guy in dustPickups array");
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

    /*private void OnDrawGizmos()
    {
        if (camera.transform == null)
        {
            camera = Camera.main;
            if (camera == null)
            {
                return;
            }
        }
        Gizmos.color = Color.red;

        Vector3 shootOrigin = camera.transform.position;
        Vector3 shootDirection = camera.transform.forward;

        Gizmos.DrawRay(shootOrigin, shootDirection * 100f);
    }*/
}
