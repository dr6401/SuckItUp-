using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AugmentManager : MonoBehaviour
{
    private bool didPlayerDieInLastRun = false;
    
    private bool isDifficultyHardcore = false;
    private int currentSuckedDust = 0;
    [SerializeField] private int defaultAugmentTriggerTreshold = 100;
    [SerializeField] private int augmentTriggerTreshold = 20;
    public bool use1stLevelFreebieThresholdForFirstAugment = false;
    private int freebieThresholdForFirstAugment = 10;
    
    [SerializeField] private GameObject player;
    [SerializeField] private AugmentSelectionUI augmentSelectionUI;
    [SerializeField] private TMP_Text dustScoreText;
    [SerializeField] private GameManager gameManager;
    
    public float augmentTriggerThresholdDuplicator;
    [SerializeField] public float normalAugmentTriggerThresholdDuplicator = 1.25f;
    [SerializeField] public float hardcoreAugmentTriggerThresholdDuplicator = 1.25f;
    
    private float silverAugmentChance = 0.5f;
    private float goldAugmentChance = 0.3f;
    
    [Header("TESTING")] [SerializeField] private bool useTestingEqualAugmentOdds = false;

    public static AugmentManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        if (augmentSelectionUI == null)
        {
            augmentSelectionUI = GameObject.FindAnyObjectByType<AugmentSelectionUI>();
        }

        if (dustScoreText == null)
        {
            Debug.Log("DustScoreText not found, looking for it in the scene");
            dustScoreText = GameObject.FindGameObjectWithTag("DustScoreText")?.GetComponent<TMP_Text>();
        }

        isDifficultyHardcore = SettingsManager.Instance.isDifficultyHardcore;
        if (!isDifficultyHardcore) augmentTriggerThresholdDuplicator = normalAugmentTriggerThresholdDuplicator;
        else augmentTriggerThresholdDuplicator = hardcoreAugmentTriggerThresholdDuplicator;
        if (use1stLevelFreebieThresholdForFirstAugment) augmentTriggerTreshold = freebieThresholdForFirstAugment;
    }

    // Update is called once per frame
    void Update()
    {
        if (dustScoreText != null)
        {
            dustScoreText.text = augmentSelectionUI.areAllAugmentsTaken() ? "" : "Dust Score: " + currentSuckedDust + "/" + augmentTriggerTreshold;
        }
        if (currentSuckedDust >= augmentTriggerTreshold && !augmentSelectionUI.areAllAugmentsTaken())
        {
            gameManager.TogglePauseGameWithoutSettingsMenu();
            GameEvents.OnHasSettingsUICoveredUpAugmentUI?.Invoke(true);
            float tempAugmentTriggerTreshold;
            if (use1stLevelFreebieThresholdForFirstAugment && !SettingsManager.Instance.isDifficultyHardcore)
            {
                tempAugmentTriggerTreshold = defaultAugmentTriggerTreshold;
            }
            else tempAugmentTriggerTreshold = augmentTriggerTreshold * augmentTriggerThresholdDuplicator;
            use1stLevelFreebieThresholdForFirstAugment = false;
            Debug.Log("Current sucked dust was " + currentSuckedDust + "! Setting new currentSuckedDust to 0 and the threshold to " + (int) tempAugmentTriggerTreshold);
            currentSuckedDust = 0;
            int augmentChance = Random.Range(1, 101);
            AugmentTier augmentTier;
            if (augmentChance <= silverAugmentChance * 100) augmentTier = AugmentTier.Silver;
            else if (augmentChance <= (silverAugmentChance + goldAugmentChance) * 100) augmentTier = AugmentTier.Gold;
            else augmentTier = AugmentTier.Prismatic;
            if (useTestingEqualAugmentOdds)
            {
                augmentTier = augmentChance switch
                {
                    <= 33 => AugmentTier.Silver,
                    <= 67 => AugmentTier.Gold,
                    _ => AugmentTier.Prismatic
                };
            }
            Debug.Log($"AugmentChance: {augmentChance}, Augment Tier: {augmentTier}");
            augmentTriggerTreshold = (int) tempAugmentTriggerTreshold;
            augmentSelectionUI.TriggerAugmentSelection(player, augmentTier);
        }
    }

    private void IncreaseSuckedDust()
    {
        currentSuckedDust++;
    }

    private void ResetSceneParametersAndReferences(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = FindAnyObjectByType<GameManager>();
        augmentSelectionUI = FindAnyObjectByType<AugmentSelectionUI>();
        dustScoreText = GameObject.FindGameObjectWithTag("DustScoreText")?.GetComponent<TMP_Text>();
        currentSuckedDust = 0;
        if (PlayerDeathManager.Instance.hasPlayerDiedInPreviousScene)
        {
            augmentTriggerTreshold = defaultAugmentTriggerTreshold;
        }
        
        if (use1stLevelFreebieThresholdForFirstAugment && !SettingsManager.Instance.isDifficultyHardcore) augmentTriggerTreshold = freebieThresholdForFirstAugment;
    }

    private void ResetAugmentTriggerThreshold() // Reset progress when exiting to main menu - remove this when adding level state persistence
    {
        currentSuckedDust = 0;
        augmentTriggerTreshold = defaultAugmentTriggerTreshold;
    }
    
    private void OnEnable()
    {
        GameEvents.OnSuckDust += IncreaseSuckedDust;
        SceneManager.sceneLoaded += ResetSceneParametersAndReferences;
        GameEvents.OnEnteredMainMenu += ResetAugmentTriggerThreshold;
    }
    
    private void OnDisable()
    {
        GameEvents.OnSuckDust -= IncreaseSuckedDust;
        SceneManager.sceneLoaded -= ResetSceneParametersAndReferences;
        GameEvents.OnEnteredMainMenu -= ResetAugmentTriggerThreshold;
    }
    
    
}
