using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Utility/DustToGold")]
public class DustToGold : Augment
{
    public override void Apply(GameObject player)
    {
        //Every 200 dust converts into a random Gold-tier augment.
    }
}