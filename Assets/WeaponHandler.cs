using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject shooterWeapon;
    public GameObject vacuumWeapon;
    private bool isShooterWeaponActive = true;
    private bool isVacuumWeaponActive;

    // Update is called once per frame

    private void Start()
    {
        vacuumWeapon.SetActive(false);
        isVacuumWeaponActive = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WeaponSwitch();
        }
    }

    private void WeaponSwitch()
    {
        isShooterWeaponActive = !isShooterWeaponActive;
        isVacuumWeaponActive = !isVacuumWeaponActive;
        shooterWeapon.SetActive(isShooterWeaponActive);
        vacuumWeapon.SetActive(isVacuumWeaponActive);
    }
}
