using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Movement/Steady Aim")]
public class SteadyAim : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ApplySteadyAim();
    }
}