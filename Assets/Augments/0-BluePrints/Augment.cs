using UnityEngine;

public enum AugmentTier
{
    Silver,
    Gold,
    Prismatic
};
public enum AugmentCategory
{
    Weaponry,
    Movement,
    Cleaning,
    Utility
};

public abstract class Augment : ScriptableObject
{
    public string augmentName;
    [TextArea] public string description;
    public Sprite icon;
    public AugmentTier tier;
    public AugmentCategory category;
    
    public abstract void Apply(GameObject player);
}
