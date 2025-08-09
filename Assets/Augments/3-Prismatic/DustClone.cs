using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Utility/DustClone")]
public class DustClone : Augment
{
    public override void Apply(GameObject player)
    {
        //Every 60 seconds, create a clone that mimics your actions for 5 seconds.
    }
}