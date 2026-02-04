using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Utility/DirtyDracula")]
public class DirtyDracula : Augment
{
    public override void Apply(GameObject player)
    {
        //When above 100 ammo, each dust sucked heals you for HP.
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        playerHealth.ApplyDirtyDracula(2);
        weaponHandler.ApplyDirtyVampireOrDracula();
    }
}