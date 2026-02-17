using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    //NOTE: Position this object in a place around which you want the enemies to spawn in radious "spawnOffset"
    //
    [SerializeField] GameObject fluffyDustyPrefab;
    [SerializeField] private GameObject flyingDustyPrefab;
    float spawnInterval;
    [SerializeField] float normalspawnInterval = 10f;
    [SerializeField] float hardcoreSpawnInterval = 3f;
    [SerializeField] public float spawnOffset = 20f;
    private float timeSinceSpawned = 0f;
    [SerializeField] private Transform enemyFoldersFolder;
    [SerializeField] private Transform enemiesFolder;
    private bool canSpawnEnemies = true;
    private int maxSpawnedEnemies;
    [SerializeField] private int normalMaxSpawnedEnemies = 20;
    [SerializeField] private int hardcoreMaxSpawnedEnemies = 80;
    private bool isClear;
    public bool isDifficultyHardcore = false;
    
    [Header("Flying Dusty stuff")]
    [SerializeField] private float chanceToSpawnFluffyDusty = 0f;
    [SerializeField] private float enemyClearanceRadius = 2f;
    [SerializeField] private int maxSpawnAttempts = 100;
    [SerializeField] private Collider nonSpawningZoneCollider;

    void Start()
    {
        if (enemiesFolder == null)
        {
            Debug.Log("Folder is null, creating new folder");
            GameObject newFolder = new GameObject($"localEnemiesFolder{GetInstanceID()}");
            if (enemyFoldersFolder != null) newFolder.transform.parent = enemyFoldersFolder;
            enemiesFolder = newFolder.transform; }
        else
        {
            enemiesFolder = enemiesFolder.transform;   
        }
        isDifficultyHardcore = SettingsManager.Instance.isDifficultyHardcore;
        if (!isDifficultyHardcore)
        {
            spawnInterval = normalspawnInterval;
            maxSpawnedEnemies = normalMaxSpawnedEnemies;
        }
        else
        {
            spawnInterval = hardcoreSpawnInterval;
            maxSpawnedEnemies = hardcoreMaxSpawnedEnemies;
        }

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
        bool foundValidSpot = false;
        Vector3 thisObjectPosition = transform.position;
        
        float spawnFluffyOrFlyingDustyChance = Random.Range(0.1f, 1f);
        
        // Ground-based FluffyDusty
        if (spawnFluffyOrFlyingDustyChance > chanceToSpawnFluffyDusty)
        {
            
            Vector3 spawnPosition = thisObjectPosition;
            Vector3 finalSpawnPosition = Vector3.zero;
            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                spawnPosition = thisObjectPosition + new Vector3(
                    Random.Range(-spawnOffset, spawnOffset),
                    Random.Range(0f, 1f),
                    Random.Range(-spawnOffset, spawnOffset)
                );

                if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    isClear = !Physics.CheckSphere(spawnPosition, enemyClearanceRadius * 0.01f);
                    if (isClear)
                    {
                        if (nonSpawningZoneCollider != null)
                        {
                            if (!nonSpawningZoneCollider.bounds.Contains(spawnPosition))
                            {
                                foundValidSpot = true;
                                finalSpawnPosition = hit.position;
                                break;
                            }
                        }
                        else
                        {
                            foundValidSpot = true;
                            finalSpawnPosition = hit.position;
                            //Debug.Log("You might have forgotten to assign a Non-Spawn-Area-Collider Object to spawner " + name);
                            break;
                        }
                    }
                }
            }
            if (foundValidSpot)
            {
                ObjectPooler.Instance?.SpawnEnemy(finalSpawnPosition, Quaternion.identity, EnemyTypes.EnemyType.FluffyDusty);
                //GameObject enemy = Instantiate(fluffyDustyPrefab, finalSpawnPosition, Quaternion.identity);
                //enemy.transform.SetParent(enemiesFolder);
                timeSinceSpawned = 0;
                foundValidSpot = false;
            }
            else Debug.Log("Couldn't spawn FluffyDusty on NavMesh. " + " Name of spawner: " + name + ", canSpawnEnemies: " + canSpawnEnemies + ", isClear: " + isClear);
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
                ObjectPooler.Instance?.SpawnEnemy(flyingSpawnPos, Quaternion.identity, EnemyTypes.EnemyType.FlyingDusty);
                //GameObject enemy = Instantiate(flyingDustyPrefab, flyingSpawnPos, Quaternion.identity);
                //enemy.transform.SetParent(enemiesFolder);
                timeSinceSpawned = 0;
                //foundValidSpot = false;
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

    private void UpdateDifficulty(bool hasDifficultyBeenSetToHardcore)
    {
        isDifficultyHardcore = hasDifficultyBeenSetToHardcore;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += DisableEnemySpawning;
        SettingsManager.OnDifficultySetToHardcoreFromSettingsManager += UpdateDifficulty;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= DisableEnemySpawning;
        SettingsManager.OnDifficultySetToHardcoreFromSettingsManager -= UpdateDifficulty;
    }
    
     /*private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // orange with transparency
        Gizmos.DrawSphere(transform.position, spawnOffset);//spawnOffset); // visualize spawn area

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
            Gizmos.DrawWireSphere(samplePoint, enemyClearanceRadius * 0.75f); // show attempted spawn positions
        }
    }*/
}
