using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //NOTE: Position this object in a place around which you want the enemies to spawn in radious "spawnOffset"
    //
    [SerializeField] GameObject fluffyDustyPrefab;
    [SerializeField] private GameObject flyingDustyPrefab;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] public float spawnOffset = 20f;
    private float timeSinceSpawned = 0f;
    private Transform enemiesFolder;
    private bool canSpawnEnemies = true;
    [SerializeField] private int maxSpawnedEnemies = 20;
    private bool isClear;
    private bool foundValidSpot = false;
    
    [Header("Flying Dusty stuff")]
    [SerializeField] private float chanceToSpawnFluffyDusty = 0f;
    [SerializeField] private float enemyClearanceRadius = 2f;
    [FormerlySerializedAs("maxFlyingSpawnAttempts")] [SerializeField] private int maxSpawnAttempts = 100;
    [SerializeField] private Collider nonSpawningZoneCollider;

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
        
        float spawnFluffyOrFlyingDustyChance = Random.Range(0.1f, 1f);
        
        // Ground-based FluffyDusty
        if (spawnFluffyOrFlyingDustyChance > chanceToSpawnFluffyDusty)
        {
            Vector3 spawnPosition = thisObjectPosition;
            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                spawnPosition = thisObjectPosition + new Vector3(
                    Random.Range(-spawnOffset, spawnOffset),
                    0f,
                    Random.Range(-spawnOffset, spawnOffset)
                );
            
                isClear = !Physics.CheckSphere(spawnPosition, enemyClearanceRadius * 0.01f);
                if (isClear)
                {
                    if (nonSpawningZoneCollider != null)
                    {
                        if (!nonSpawningZoneCollider.bounds.Contains(spawnPosition))
                        {
                            foundValidSpot = true;
                            break;
                        }
                    }
                    else
                    {
                        foundValidSpot = true;
                        Debug.Log("You might have forgotten to assign a Non-Spawn-Area-Collider Object to spawner " + name);
                        break;
                    }
                }
            }

            if (foundValidSpot && NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                GameObject enemy = Instantiate(fluffyDustyPrefab, hit.position, Quaternion.identity);
                enemy.transform.SetParent(enemiesFolder);
                timeSinceSpawned = 0;
            }
            else
            {
                Debug.Log("Couldn't spawn FluffyDusty on NavMesh, foundValidSpot: " + foundValidSpot + ". Name of spawner: " + name + ", canSpawnEnemies: " + canSpawnEnemies + ", isClear: " + isClear);
            }
        }
        
        else
        {
            // FlyingDusty
            Vector3 flyingSpawnPos = Vector3.zero;

            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                Vector3 offsetXZ = new Vector3(
                    Random.Range(-spawnOffset, spawnOffset),
                    Random.Range(-1f, 3f),
                    Random.Range(-spawnOffset, spawnOffset)
                );

                flyingSpawnPos = thisObjectPosition + offsetXZ;

                isClear = !Physics.CheckSphere(flyingSpawnPos, enemyClearanceRadius);

                if (isClear)
                {
                    if (nonSpawningZoneCollider != null)
                    {
                        if (!nonSpawningZoneCollider.bounds.Contains(flyingSpawnPos))
                        {
                            foundValidSpot = true;
                            break;
                        }
                    }
                    else
                    {
                        foundValidSpot = true;
                        Debug.Log("Assign a Non-Spawn-Area-Collider Object to spawner " + name + " !!!");
                        break;
                    }
                }
            }

            if (foundValidSpot)
            {
                GameObject enemy = Instantiate(flyingDustyPrefab, flyingSpawnPos, Quaternion.identity);
                enemy.transform.SetParent(enemiesFolder);
                timeSinceSpawned = 0;
            }
            else
            {
                Debug.LogWarning("Could not find a valid flying enemy spawn point near spawner.");
            }
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
    
     /*private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // orange with transparency
        Gizmos.DrawSphere(transform.position, spawnOffset); // visualize spawn area

        // Optional: draw individual sample points
        Gizmos.color = Color.cyan;
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnOffset, spawnOffset),
                Random.Range(-1f, 3f), // vertical scatter for flying enemies
                Random.Range(-spawnOffset, spawnOffset)
            );
            Vector3 samplePoint = transform.position + randomOffset;
            Gizmos.DrawWireSphere(samplePoint, 2f); // show attempted spawn positions
        }
    }*/
}
