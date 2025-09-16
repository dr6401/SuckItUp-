using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class AugmentManager : MonoBehaviour
{
    private int currentSuckedDust = 0;
    [SerializeField] private int augmentTriggerTreshold = 20;
    [SerializeField] private GameObject player;
    [SerializeField] private AugmentSelectionUI augmentSelectionUI;
    [SerializeField] private TMP_Text dustScoreText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] public float augmentTriggerTresholdDuplicator = 1f;

    private static AugmentManager instance;
    private void Awake()
    {

        AugmentManager[] managers = FindObjectsByType<AugmentManager>(sortMode: FindObjectsSortMode.InstanceID);

        if (managers.Length > 1)
        {
            foreach (var m in managers)
            {
                if (m != this && m.gameObject.scene.name == "DontDestroyOnLoad")
                {
                    Destroy(gameObject);
                    return;
                }
            }
            
            
            foreach (var m in managers)
            {
                if (m != this)
                {
                    if (managers[0] != this)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
        
        if (instance != null && instance != this)
        {
            Debug.Log("Another Augment Manager found in the scene. This one with id " + GetInstanceID() + " will self destruct now.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Augment Manager ID: " + GetInstanceID());
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
            dustScoreText = GameObject.FindGameObjectWithTag("DustScoreText").GetComponent<TMP_Text>();
        }
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
            float tempAugmentTriggerTreshold = augmentTriggerTreshold * augmentTriggerTresholdDuplicator;
            Debug.Log("Current sucked dust was " + currentSuckedDust + "! Setting new currentSuckedDust to 0 and the threshold to " + (int) tempAugmentTriggerTreshold);
            currentSuckedDust = 0;
            int augmentChance = Random.Range(1, 100);
            AugmentTier augmentTier = augmentChance switch
            {
                <= 40 => AugmentTier.Silver,
                <= 75 => AugmentTier.Gold,
                _ => AugmentTier.Prismatic
            };
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
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        augmentSelectionUI = GameObject.FindAnyObjectByType<AugmentSelectionUI>();
        dustScoreText = GameObject.FindGameObjectWithTag("DustScoreText").GetComponent<TMP_Text>();
        currentSuckedDust = 0;
    }
    
    private void OnEnable()
    {
        GameEvents.OnSuckDust += IncreaseSuckedDust;
        SceneManager.sceneLoaded += ResetSceneParametersAndReferences;
    }
    
    private void OnDisable()
    {
        GameEvents.OnSuckDust -= IncreaseSuckedDust;
        SceneManager.sceneLoaded -= ResetSceneParametersAndReferences;
    }
    
    
}
