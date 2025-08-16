using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Gold/Cleaning/OverchargedVacuum")]
public class OverchargedVacuum : Augment
{
    public override void Apply(GameObject player)
    {
        //Sucking up dust briefly charges your weapon, increasing fire rate by 50% for 3 seconds.
    }
}