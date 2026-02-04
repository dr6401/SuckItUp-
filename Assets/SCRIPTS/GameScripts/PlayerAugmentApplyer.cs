using UnityEngine;

public class PlayerAugmentApplyer : MonoBehaviour
{

    [SerializeField] private RunAugmentData runAugmentData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Augment augment in runAugmentData.chosenAugments)
        {
            augment.Apply(gameObject);
            Debug.Log($"Applied {augment.name.ToUpper()} to Player at start of the game");
        }
    }
}
