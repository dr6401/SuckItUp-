using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Movement/Featherless")]
public class Featherless : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ApplyFeatherless();
    }
}