using System.Collections.Generic;
using UnityEngine;

public class AugmentSelectionUI : MonoBehaviour
{
    public GameObject player;
    public Transform buttonParent;
    public GameObject augmentButtonPrefab;
    public List<Augment> silverAugments, goldAugments, prismaticAugments;
    public List<Augment> chosenAugments;
    [SerializeField] private int numberOfChoices = 3;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void OpenUI(GameObject playerRef, AugmentTier tier)
    {
        player = playerRef;
        List<Augment> pool = GetPoolByTier(tier);
        pool.RemoveAll(augment => chosenAugments.Contains(augment));
        List<Augment> choices = GetRandomAugments(pool, numberOfChoices);

        foreach (var choice in choices)
        {
            var btnObj = Instantiate(augmentButtonPrefab, buttonParent);
            var btnObjScript = btnObj.GetComponent<AugmentButton>();
            btnObjScript.Setup(choice, player, this);
        }
        
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }

    private List<Augment> GetRandomAugments(List<Augment> pool, int count)
    {
        var offeredAugments = new List<Augment>();
        var availableAugments = new List<Augment>(pool);

        for (int i = 0; i < count && availableAugments.Count > 0; i++)
        {
            int idx = Random.Range(0, availableAugments.Count);
            offeredAugments.Add(availableAugments[idx]);
            offeredAugments.RemoveAt(idx);
        }

        return offeredAugments;
    }

    public void StoreChosenAugment(Augment augment)
    {
        chosenAugments.Add(augment);
    }

    private List<Augment> GetPoolByTier(AugmentTier tier)
    {
        return tier switch
        {
            AugmentTier.Silver => silverAugments,
            AugmentTier.Gold => goldAugments,
            AugmentTier.Prismatic => prismaticAugments,
            _ => silverAugments,
        };
    }
}
