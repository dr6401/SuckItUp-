using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Weaponry/AmmoRecycler")]
public class AmmoRecycler : Augment
{
    public override void Apply(GameObject player)
    {
        //Killing enemies has a 20% chance to restore 1 ammo.
    }
}