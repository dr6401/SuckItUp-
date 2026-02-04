using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Weaponry/HitHarder")]
public class HitHarder : Augment
{
    public override void Apply(GameObject player)
    {
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyHitHarder();
    }
}