using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Cleaning/SuctionVortex")]
public class SuctionVortex : Augment
{
    public override void Apply(GameObject player)
    {
        //Holding vacuum for 2 seconds creates a black hole that pulls enemies inward for 3 seconds.
    }
}