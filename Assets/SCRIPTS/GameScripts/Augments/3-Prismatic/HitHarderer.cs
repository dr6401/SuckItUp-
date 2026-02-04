using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Weaponry/HitHarderer")]
public class HitHarderer : Augment
{
    public override void Apply(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyHitHarderer();
    }
}