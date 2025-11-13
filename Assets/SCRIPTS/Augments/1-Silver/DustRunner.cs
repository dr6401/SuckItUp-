using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Movement/DustRunner")]
public class DustRunner : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ApplyDustRunner(1.4f);
    }
}