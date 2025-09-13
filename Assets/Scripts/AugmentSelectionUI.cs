using System;
using System.Collections;
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
    [SerializeField] private int numberOfChoices = 3;
    private int availableAugmentsAtStart;
    [SerializeField] private CanvasGroup canvasGroup;
    private Coroutine fadeInCoroutine;
    [Header("Augment Persistence")]
    [SerializeField] private RunAugmentData runAugmentData;
    [Header("-----TESTING-----")]
    [SerializeField] private bool testing_offerOnlySilverAugments = false;

    private bool hasSettingsCoveredUpAugmentUI;
    
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
        
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        foreach (var silverAugment in silverAugments)
        {
            availableAugmentsAtStart++;
        }
        foreach (var goldAugment in goldAugments)
        {
            availableAugmentsAtStart++;
        }
        foreach (var prismaticAugment in prismaticAugments)
        {
            availableAugmentsAtStart++;
        }
        //Debug.Log("availableAugmentsAtStart: " + availableAugmentsAtStart);
    }

    /*public void TestAugmentSelection()
    {
        List<Augment> silverPool = GetPoolByTier(AugmentTier.Silver);
        silverPool.RemoveAll(augment => runAugmentData.IsAugmentInChosenAugments(augment));
        List<Augment> goldPool = GetPoolByTier(AugmentTier.Gold);
        goldPool.RemoveAll(augment => runAugmentData.IsAugmentInChosenAugments(augment));
        List<Augment> prismaticPool = GetPoolByTier(AugmentTier.Prismatic);
        prismaticPool.RemoveAll(augment => runAugmentData.IsAugmentInChosenAugments(augment));
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
    }*/

    public void TriggerAugmentSelection(GameObject playerRef, AugmentTier tier)
    {
        player = playerRef;
        List<Augment> pool = testing_offerOnlySilverAugments ? GetPoolByTier(AugmentTier.Silver) : GetPoolByTier(tier);// if we're testing, enable only silver augments
        pool.RemoveAll(augment => runAugmentData.IsAugmentInChosenAugments(augment));
        Debug.Log(tier + " pool: " + string.Join(", ", pool.Select(a => a.augmentName)));
        List<Augment> choices = GetRandomAugments(pool, numberOfChoices);
        Debug.Log(tier + " choices: " + string.Join(", ", choices.Select(a => a.augmentName)));
        
        Debug.Log("Current pool of " + tier + " augments: " + string.Join(", ", pool.Select(a => a.augmentName)));

        if (pool.Count <= 0) // If there are no more augments left in this tier, try again
        {
            if (areAllAugmentsTaken())
            {
                Debug.Log("All Augments taken!");
                return;
            }
            int augmentChance = Random.Range(1, 100);
            AugmentTier augmentTier = augmentChance switch
            {
                <= 50 => AugmentTier.Silver,
                <= 80 => AugmentTier.Gold,
                _ => AugmentTier.Prismatic
            };
            Debug.Log("Tier: " + tier + " did not have any augments left. Retrying augments with tier: " + augmentTier);
            TriggerAugmentSelection(playerRef, augmentTier);
            return;
        }

        foreach (var choice in choices)
        {
            Debug.Log("Given you the choice: " + choice.augmentName);
            var btnObj = Instantiate(augmentButtonPrefab, buttonParent);
            var btnObjScript = btnObj.GetComponent<AugmentButton>();
            btnObjScript.Setup(choice, player, this);
        }
        FadeInAugmentsUI();
    }

    public void CloseUI()
    {
        gameManager.TogglePauseGameWithoutSettingsMenu();
        FadeOutAugmentsUI();
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
        runAugmentData.AddToChosenAugments(augment);
    }

    private List<Augment> GetPoolByTier(AugmentTier tier)
    {
        return tier switch
        {
            AugmentTier.Silver => silverAugments,
            AugmentTier.Gold => goldAugments,
            AugmentTier.Prismatic => prismaticAugments,
            //_ => silverAugments,
        };
    }

    public bool areAllAugmentsTaken()
    {
        return runAugmentData.NumberOfChosenAugments() >= availableAugmentsAtStart;
    }

    public void FadeInAugmentsUI()
    {
        if (hasSettingsCoveredUpAugmentUI) return;
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        gameObject.SetActive(true);
        fadeInCoroutine = StartCoroutine(FadeInOrOutAugmentsUI(1));
    }
    public void FadeOutAugmentsUI()
    {
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        fadeInCoroutine = StartCoroutine(FadeInOrOutAugmentsUI(0));
    }
    
    private IEnumerator FadeInOrOutAugmentsUI(float targetAlpha)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;
        float easeTime = GameConstants.fadeInOrOutDuration;
        if (targetAlpha == 0) easeTime = GameConstants.shortFadeInOrOutDuration;
        while (elapsed < easeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha,(elapsed / easeTime) * (elapsed / easeTime)); // Multiply, so we get a squared function instead of linear
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        if (targetAlpha == 0) // If fading out, destroy augment buttons and deactivate the game object
        {
            foreach (Transform child in buttonParent)
            {
                Destroy(child.gameObject);
            }
            gameObject.SetActive(false);
        }
        fadeInCoroutine = null;
    }
    
    public void FadeOutAugmentsUIWithoutDestroyingIt()
    {
        if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);
        fadeInCoroutine = StartCoroutine(FadeInOrOutAugmentsUIWithoutDestroyingIt(0));
    }
    private IEnumerator FadeInOrOutAugmentsUIWithoutDestroyingIt(float targetAlpha)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;
        float easeTime = GameConstants.fadeInOrOutDuration;
        if (targetAlpha == 0) easeTime = GameConstants.shortFadeInOrOutDuration;
        while (elapsed < easeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha,(elapsed / easeTime) * (elapsed / easeTime)); // Multiply, so we get a squared function instead of linear
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        fadeInCoroutine = null;
    }
}
