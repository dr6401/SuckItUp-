using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Movement/RocketJump")]
public class RocketJump : Augment
{
    public override void Apply(GameObject player)
    {
        //Pressing jump while mid-air launches you upward and backward, consuming 10 ammo.
    }
}