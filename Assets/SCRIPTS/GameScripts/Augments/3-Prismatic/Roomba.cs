using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Cleaning/Roomba")]
public class Roomba : Augment
{
    public override void Apply(GameObject player)
    {
        //Every 60 seconds, create a clone that mimics your actions for 5 seconds.
    }
}