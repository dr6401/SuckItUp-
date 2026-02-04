using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Utility/ColossalCleaner")]
public class ColossalCleaner : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerHealth.ApplyColossalCleaner(0.5f);
        weaponHandler.ApplyColossalCleaner(1.15f);
        playerMovement.ApplyColossalCleaner(3f);
    }
}