using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Utility/DirtyVampire")]
public class DirtyVampire : Augment
{
    public override void Apply(GameObject player)
    {
        //When above 100 ammo, each dust sucked heals you for HP.
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        playerHealth.ApplyDirtyVampire(1);
        weaponHandler.ApplyDirtyVampireOrDracula();
    }
}