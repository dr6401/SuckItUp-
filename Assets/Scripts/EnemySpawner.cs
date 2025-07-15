using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //NOTE: Position this object in a place around which you want the enemies to spawn in radious "spawnOffset"
    [SerializeField] GameObject fluffyDustyPrefab;
    [SerializeField] private GameObject flyingDustyPrefab;
    [SerializeField] float spawnInterval = 3f;
    public float spawnOffset = 20f;
    private float timeSinceSpawned = 0f;
    private Transform enemiesFolder;
    private bool canSpawnEnemies = true;
    [SerializeField] private int maxSpawnedEnemies = 20;
    void Start()
    {
        Transform parent = transform.parent;
        GameObject folder = GameObject.Find("EnemiesFolder");
        if (folder == null)
        {
            Debug.Log("Folder is null, creating new folder");
            folder = new GameObject("EnemiesFolder");
            folder.transform.parent = parent;
        }
        enemiesFolder = folder.transform;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSpawned += Time.deltaTime;

        if (timeSinceSpawned > spawnInterval && canSpawnEnemies && enemiesFolder.childCount < maxSpawnedEnemies)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 thisObjectPosition = transform.position;
        float xOffset = Random.Range(-spawnOffset, spawnOffset);
        float zOffset = Random.Range(-spawnOffset, spawnOffset);

        Vector3 spawnPosition = new Vector3(thisObjectPosition.x + xOffset, thisObjectPosition.y, thisObjectPosition.z + zOffset);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 2f, NavMesh.AllAreas))
        {
            int spawnFluffyOrFlyingDustyChance = Random.Range(1, 10);
            if (spawnFluffyOrFlyingDustyChance < 1)// 70% chance to spawn fluffyDusty
            {
                GameObject enemy = Instantiate(fluffyDustyPrefab, spawnPosition, Quaternion.identity);
                enemy.transform.SetParent(enemiesFolder);
            }
            else// 30% chance to spawn flyingDusty
            {
                GameObject enemy = Instantiate(flyingDustyPrefab, spawnPosition, Quaternion.identity);
                enemy.transform.SetParent(enemiesFolder);
            }
            timeSinceSpawned = 0;
        }
        else
        {
            Debug.Log("Couldn't spawn enemy on NavMesh, trying to spawn again");
        }
    }

    private void DisableEnemySpawning()
    {
        canSpawnEnemies = false;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += DisableEnemySpawning;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= DisableEnemySpawning;
    }
}
