using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Movement/Swiftness")]
public class Swiftness : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ApplySwiftness();
    }
}