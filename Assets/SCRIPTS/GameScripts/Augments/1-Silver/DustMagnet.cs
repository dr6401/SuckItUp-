using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Cleaning/DustMagnet")]
public class DustMagnet : Augment
{
    public override void Apply(GameObject player)
    {
        //Increases vacuum suction range by 30%
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        weaponHandler.ApplyDustMagnet(1.3f);
    }
}