using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Movement/GrappleSuck")]
public class GrappleSuck : Augment
{
    public override void Apply(GameObject player)
    {
        //Vacuum can now latch onto walls and enemies, pulling you toward them.
    }
}