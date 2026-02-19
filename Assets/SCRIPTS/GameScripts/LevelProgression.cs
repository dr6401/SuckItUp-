using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MoreMountains.Feedbacks;

public class LevelProgression : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject level1AreaObject;
    [SerializeField] private GameObject level2AreaObject;
    [SerializeField] private GameObject level3AreaObject;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject loadingLevelTextObject;
    [SerializeField] private MMFeedbacks loadLvl1SceneFeedback;
    [SerializeField] private MMFeedbacks loadLvl2SceneFeedback;
    [SerializeField] private MMFeedbacks loadLvl3SceneFeedback;
    private Vector3 adjustedPlayerPosition;

    private Collider level1Area;
    private Collider level2Area;
    private Collider level3Area;

    private MeshRenderer level1AreaMr;
    private MeshRenderer level2AreaMr;
    private MeshRenderer level3AreaMr;

    private float levelLoadingTime = 2;
    private float currentTimer = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        loadingLevelTextObject.SetActive(false);
        if (level1AreaObject != null && level2AreaObject != null && level3AreaObject != null)
        {
            level1Area = level1AreaObject.GetComponent<Collider>();
            level2Area = level2AreaObject.GetComponent<Collider>();
            level3Area = level3AreaObject.GetComponent<Collider>();

            level1AreaMr = level1AreaObject.GetComponent<MeshRenderer>();
            level2AreaMr = level2AreaObject.GetComponent<MeshRenderer>();
            level3AreaMr = level3AreaObject.GetComponent<MeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        adjustedPlayerPosition = player.transform.position + Vector3.down;

        if (isPlayerInsideBounds(level1Area.bounds, adjustedPlayerPosition))// && PlayerPrefs.GetInt("TutorialCompleted") == 1) - Lvl1 should always be accessible for play
        {                                                                   // since users can always just skip tutorial 
            level1AreaMr.enabled = true;
            loadingLevelTextObject.SetActive(true);
            currentTimer += Time.deltaTime;
            if (currentTimer >= levelLoadingTime)
            {
                LoadLvl1Scene();
            }
        }
        else if (isPlayerInsideBounds(level2Area.bounds, adjustedPlayerPosition) && PlayerPrefs.GetInt("Level1") == 1)
        {
            level2AreaMr.enabled = true;
            loadingLevelTextObject.SetActive(true);
            currentTimer += Time.deltaTime;
            if (currentTimer >= levelLoadingTime)
            {
                LoadLvl2Scene();
            }
        }
        else if (isPlayerInsideBounds(level3Area.bounds, adjustedPlayerPosition) && PlayerPrefs.GetInt("Level2") == 1)
        {
            level3AreaMr.enabled = true;
            loadingLevelTextObject.SetActive(true);
            currentTimer += Time.deltaTime;
            if (currentTimer >= levelLoadingTime)
            {
                LoadLvl3Scene();
            }
        }
        else {
            currentTimer = 0;
            loadingLevelTextObject.SetActive(false);
            level1AreaMr.enabled = false;
            level2AreaMr.enabled = false;
            level3AreaMr.enabled = false;
        }
    }

    private bool isPlayerInsideBounds(Bounds bounds, Vector3 player)
    {
        return player.x >= bounds.min.x && player.x <= bounds.max.x
            && player.z >= bounds.min.z && player.z <= bounds.max.z;
    }
    
    private void LoadLvl1Scene()
    {
        loadLvl1SceneFeedback?.PlayFeedbacks();
    }
    private void LoadLvl2Scene()
    {
        loadLvl2SceneFeedback?.PlayFeedbacks();
    }
    private void LoadLvl3Scene()
    {
        loadLvl3SceneFeedback?.PlayFeedbacks();
    }
}
