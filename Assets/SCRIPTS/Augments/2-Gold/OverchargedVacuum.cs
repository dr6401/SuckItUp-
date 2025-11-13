using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Cleaning/OverchargedVacuum")]
public class OverchargedVacuum : Augment
{
    public override void Apply(GameObject player)
    {
        //Sucking up dust briefly charges your weapon, increasing fire rate by 75% for 3 seconds.
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyOverchargedVacuum(3f, 1.75f);
    }
}