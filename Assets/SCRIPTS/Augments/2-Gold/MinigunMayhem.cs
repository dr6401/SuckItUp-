using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Weaponry/MinigunMayhem")]
public class MinigunMayhem : Augment
{
    public override void Apply(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyMinigunMayhem();
    }
}