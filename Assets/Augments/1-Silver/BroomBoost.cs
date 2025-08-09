using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Movement/BroomBoost")]
public class BroomBoost : Augment
{
    public override void Apply(GameObject player)
    {
        //Slight movement speed increase (+10%) when actively vacuuming.
    }
}