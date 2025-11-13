using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Utility/DuststormShell")]
public class DuststormShell : Augment
{
    public override void Apply(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        weaponHandler.ApplyDuststormShell();
        playerHealth.ApplyDustStormShell(1- 0.4f); //40% dmg reduction is same as 60% damage taken
    }
}