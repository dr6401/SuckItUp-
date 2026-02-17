using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject fluffyDustyPrefab;
    [SerializeField] private GameObject flyingDustyPrefab;
    private int preloadAmount = 1000;
    [SerializeField] private GameObject dustParticlePrefab;
    [SerializeField] private int currentAliveEnemies = 0;
    public int maxCurrentAliveAnimals = 500;

    public Transform animalFoldersFolder;
    public Transform fluffyDustyParentFolder;
    public Transform flyingDustyParentFolder;
    public Transform dustParticlesParentFolder;
    private Queue<GameObject> fluffyDustyPool = new Queue<GameObject>();
    private Queue<GameObject> flyingDustyPool = new Queue<GameObject>();
    private Queue<GameObject> vfxPool = new Queue<GameObject>();

    public bool isEndlessLevel = false;
    public bool isIdlerLevel = false;

    //public static ObjectPooler Instance;
    
    public static ObjectPooler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log($"No ObjectPooler in scene {SceneManager.GetActiveScene().name}!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Preload();
    }

    public void Preload()
    {
        for (int i = 0; i < preloadAmount; i++)
        {
            GameObject fluffyDustyObject = Instantiate(fluffyDustyPrefab, fluffyDustyParentFolder);
            fluffyDustyObject.SetActive(false);
            fluffyDustyPool.Enqueue(fluffyDustyObject);
            
            GameObject flyingDustyObject = Instantiate(flyingDustyPrefab, flyingDustyParentFolder);
            flyingDustyObject.SetActive(false);
            flyingDustyPool.Enqueue(flyingDustyObject);
            
            GameObject dustParticle = Instantiate(dustParticlePrefab, dustParticlesParentFolder);
            dustParticle.SetActive(false);
            vfxPool?.Enqueue(dustParticle);
        }
    }

    public GameObject SpawnFluffyDusty(Vector3 position, Quaternion rotation)
    {
        GameObject dusty;

        if (fluffyDustyPool.Count > 0)
        {
            dusty = fluffyDustyPool.Dequeue();
            dusty.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            dusty = Instantiate(fluffyDustyPrefab, position, rotation, fluffyDustyParentFolder);
            //Debug.Log("Pool ran out! Instantiating animal by normal means");
        }
        
        dusty.SetActive(true);
        currentAliveEnemies++;
        return dusty;
    }
    
    public GameObject SpawnFlyingDusty(Vector3 position, Quaternion rotation)
    {
        GameObject dusty;

        if (flyingDustyPool.Count > 0)
        {
            dusty = flyingDustyPool.Dequeue();
            dusty.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            dusty = Instantiate(flyingDustyPrefab, position, rotation, flyingDustyParentFolder);
            //Debug.Log("Pool ran out! Instantiating animal by normal means");
        }
        
        dusty.SetActive(true);
        currentAliveEnemies++;
        return dusty;
    }
    
    public GameObject SpawnDustParticle(Vector3 position, Quaternion rotation)
    {
        GameObject dust;

        if (vfxPool?.Count > 0)
        {
            dust = vfxPool.Dequeue();
            dust.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            dust = Instantiate(dustParticlePrefab, position, rotation, dustParticlesParentFolder);
        }
        
        dust.SetActive(true);
        return dust;
    }

    public void DespawnFluffyDusty(GameObject fluffy)
    {
        fluffy.SetActive(false);
        fluffy.transform.SetParent(fluffyDustyParentFolder);
        fluffyDustyPool.Enqueue(fluffy);
        currentAliveEnemies--;
    }
    
    public void DespawnFlyingDusty(GameObject flying)
    {
        flying.SetActive(false);
        flying.transform.SetParent(flyingDustyParentFolder);
        fluffyDustyPool.Enqueue(flying);
        currentAliveEnemies--;
    }
    
    public void DespawnDustParticle(GameObject dust)
    {
        dust.SetActive(false);
        dust.transform.SetParent(dustParticlesParentFolder);
        vfxPool?.Enqueue(dust);
    }

    public void SetAllFluffyDustiesInactive()
    {
        foreach (Transform fluffy in fluffyDustyParentFolder)
        {
            if (fluffy.gameObject.activeSelf)
            {
                DespawnFluffyDusty(fluffy.gameObject);
            }
        }
    }
    
    public void SetAllFllyingDustiesInactive()
    {
        foreach (Transform flying in flyingDustyParentFolder)
        {
            if (flying.gameObject.activeSelf)
            {
                DespawnFlyingDusty(flying.gameObject);
            }
        }
    }
    
    public void SetAllDustInactive()
    {
        foreach (Transform dust in dustParticlesParentFolder)
        {
            if (dust.gameObject.activeSelf)
            {
                DespawnDustParticle(dust.gameObject);
            }
        }
    }

    public bool CanSpawnEnemies()
    {
        return currentAliveEnemies < maxCurrentAliveAnimals;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetAllFluffyDustiesInactive();
        SetAllDustInactive();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
