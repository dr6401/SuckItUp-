using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Silver/Movement/Swiftness")]
public class Swiftness : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.baseMoveSpeed *= 1.25f;
        playerMovement.UpdateHalvedMovementSpeed(); // when choosing this augment update the halvedMoveSpeed (used for crouching) which is only set in Start() of PlayerMovement
        Debug.Log("new base MoveSpeed: " + playerMovement.baseMoveSpeed);
    }
}