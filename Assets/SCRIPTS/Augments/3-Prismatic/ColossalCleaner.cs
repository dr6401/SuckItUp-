using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augments/Prismatic/Utility/ColossalCleaner")]
public class ColossalCleaner : Augment
{
    public override void Apply(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        playerHealth.ApplyColossalCleaner(0.5f);
        //weaponHandler.ApplyDirtyVampireOrDracula();
    }
}