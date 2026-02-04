using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Weaponry/MinigunCarnage")]
public class MinigunCarnage : Augment
{
    public override void Apply(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyMinigunCarnage();
    }
}