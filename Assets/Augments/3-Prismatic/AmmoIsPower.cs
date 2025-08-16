using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Weaponry/AmmoIsPower")]
public class AmmoIsPower : Augment
{
    public override void Apply(GameObject player)
    {
        //Damage scales with current ammo \u2014 +0.2% per bullet over 100.
    }
}