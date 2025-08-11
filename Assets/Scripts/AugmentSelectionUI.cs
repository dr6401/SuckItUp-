using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AugmentSelectionUI : MonoBehaviour
{
    public GameObject player;
    public Transform buttonParent;
    public GameObject augmentButtonPrefab;
    [SerializeField] private GameManager gameManager;
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
        if (gameManager == null)
        {
            gameManager = GameObject.FindAnyObjectByType<GameManager>();
        }
        gameObject.SetActive(false);
    }

    public void TestAugmentSelection()
    {
        List<Augment> silverPool = GetPoolByTier(AugmentTier.Silver);
        silverPool.RemoveAll(augment => chosenAugments.Contains(augment));
        List<Augment> goldPool = GetPoolByTier(AugmentTier.Gold);
        goldPool.RemoveAll(augment => chosenAugments.Contains(augment));
        List<Augment> prismaticPool = GetPoolByTier(AugmentTier.Prismatic);
        prismaticPool.RemoveAll(augment => chosenAugments.Contains(augment));
        if (silverPool.Count == 0 && goldPool.Count == 0 && prismaticPool.Count == 0)
        {
            Debug.Log("No more Augments left!");
            return;
        }
        AugmentTier tier = (AugmentTier)Random.Range(0, System.Enum.GetValues(typeof(AugmentTier)).Length);
        if (GetPoolByTier(tier).Count != 0)
        {
            TriggerAugmentSelection(player, tier);
        }
        else
        {
            TestAugmentSelection();
        }
    }

    public void TriggerAugmentSelection(GameObject playerRef, AugmentTier tier)
    {
        player = playerRef;
        List<Augment> pool = GetPoolByTier(tier);
        pool.RemoveAll(augment => chosenAugments.Contains(augment));
        List<Augment> choices = GetRandomAugments(pool, numberOfChoices);
        
        Debug.Log("Current pool of " + tier + " augments: " + string.Join(", ", pool.Select(a => a.augmentName)));

        foreach (var choice in choices)
        {
            Debug.Log("Given you the choice: " + choice.augmentName);
            var btnObj = Instantiate(augmentButtonPrefab, buttonParent);
            var btnObjScript = btnObj.GetComponent<AugmentButton>();
            btnObjScript.Setup(choice, player, this);
        }
        
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        gameManager.TogglePauseGame();
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
            availableAugments.RemoveAt(idx);
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
