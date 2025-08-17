using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelProgression : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject level1AreaObject;
    [SerializeField] private GameObject level2AreaObject;
    [SerializeField] private GameObject level3AreaObject;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject loadingLevelTextObject;
    private Vector3 adjustedPlayerPosition;

    private Collider level1Area;
    private Collider level2Area;
    private Collider level3Area;

    private MeshRenderer level1AreaMr;
    private MeshRenderer level2AreaMr;
    private MeshRenderer level3AreaMr;

    private float levelLoadingTime = 3;
    private float currentTimer = 0;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
        if (player == null) return;
        adjustedPlayerPosition = player.transform.position + Vector3.down;

        if (isPlayerInsideBounds(level1Area.bounds, adjustedPlayerPosition) && PlayerPrefs.GetInt("TutorialCompleted") == 1)
        {
            level1AreaMr.enabled = true;
            loadingLevelTextObject.SetActive(true);
            currentTimer += Time.deltaTime;
            if (currentTimer >= levelLoadingTime)
            {
                SceneManager.LoadScene("Level1");
            }
        }
        else if (isPlayerInsideBounds(level2Area.bounds, adjustedPlayerPosition) && PlayerPrefs.GetInt("Level1") == 1)
        {
            level2AreaMr.enabled = true;
            loadingLevelTextObject.SetActive(true);
            currentTimer += Time.deltaTime;
            if (currentTimer >= levelLoadingTime)
            {
                SceneManager.LoadScene("Level2");
            }
        }
        else if (isPlayerInsideBounds(level3Area.bounds, adjustedPlayerPosition) && PlayerPrefs.GetInt("Level2") == 1)
        {
            level3AreaMr.enabled = true;
            loadingLevelTextObject.SetActive(true);
            currentTimer += Time.deltaTime;
            if (currentTimer >= levelLoadingTime)
            {
                SceneManager.LoadScene("Level3");
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
}
