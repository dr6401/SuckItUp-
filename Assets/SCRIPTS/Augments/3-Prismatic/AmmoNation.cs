using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Weaponry/AmmoNation")]
public class AmmoNation : Augment
{
    public override void Apply(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyAmmoNation(2f);
    }
}