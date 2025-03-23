using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject enemyHitEffectPrefab;
    public RawImage crossHair;

    // Update is called once per frame

    private void Start()
    {
        vacuumWeapon.SetActive(false);
        isVacuumWeaponActive = false;
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        crossHair.enabled = false;
    }
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            WeaponSwitch();
        }

        if (isShooterWeaponActive && Input.GetMouseButton(0) && timeSinceLastShot >= fireRate)
        {
            Shoot();
            timeSinceLastShot = 0;
        }
        // Aim
        if (Input.GetMouseButton(1))
        {
            crossHair.enabled = true;
        }
        else
        {
            crossHair.enabled = false;
        }
    }

    private void Shoot()
    {
        int layerMask = ~LayerMask.GetMask("Player");

        RaycastHit hit;
        Vector3 shootOrigin = camera.transform.position;
        Vector3 shootDirection = camera.transform.forward;

        Debug.Log("Shooting!");

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootingRange, layerMask))
        {
            Debug.Log("hit " + hit.collider.gameObject.name + "!");

            if (hit.collider.tag == "Enemy")
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
    }

    private void WeaponSwitch()
    {
        isShooterWeaponActive = !isShooterWeaponActive;
        isVacuumWeaponActive = !isVacuumWeaponActive;
        shooterWeapon.SetActive(isShooterWeaponActive);
        vacuumWeapon.SetActive(isVacuumWeaponActive);
    }

    private IEnumerator CrossHairColourToggle()
    {
        float colourToggleDuration = 0.1f;
        Debug.Log("Finna change crosshair colour to green");
        crossHair.color = Color.green;

        yield return new WaitForSeconds(colourToggleDuration);

        crossHair.color = Color.white;
        Debug.Log("Changed crosshair colour to white");
    }

    private void OnDrawGizmos()
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
    }
}
