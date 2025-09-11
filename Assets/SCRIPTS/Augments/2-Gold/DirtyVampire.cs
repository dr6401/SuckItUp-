using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Utility/DirtyVampire")]
public class DirtyVampire : Augment
{
    public override void Apply(GameObject player)
    {
        //When above 100 ammo, each dust sucked heals you for 2 HP.
    }
}