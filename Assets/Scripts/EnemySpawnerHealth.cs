using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemySpawnerHealth : EnemyHealth
{
    [SerializeField] private EnemySpawnManager enemySpawnManager;
    [SerializeField] private GameObject smokeBleed1VFXprefab;
    private bool hasSmokeBleedAlreadySpawnedOnce = false;
    private bool hasSmokeBleedAlreadySpawnedTwice = false;
    private void Start()
    {
        currentHealth = maxHealth;
        enemySpawnManager = GetComponentInParent<EnemySpawnManager>();
    }
    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
        if (currentHealth < 67 && !hasSmokeBleedAlreadySpawnedOnce)
        {
            Instantiate(smokeBleed1VFXprefab, transform.position, Quaternion.Euler(0, Random.Range(0f,359f), 0), transform);
            hasSmokeBleedAlreadySpawnedOnce = true;
        }
        if (currentHealth < 33 && !hasSmokeBleedAlreadySpawnedTwice)
        {
            Instantiate(smokeBleed1VFXprefab, transform.position, Quaternion.Euler(0, Random.Range(0f,359f), 0), transform);
            hasSmokeBleedAlreadySpawnedTwice = true;
        }
    }
    protected override void Die()
    {
        if (dustPickupPrefab != null)
        {
            for (int i = 0; i < Random.Range(minSpawnedDustParticles, maxSpawnedDustParticles); i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 1f;
                spawnPosition.y += 1f; // Increase the spawn height so things don't get spawned in the ground
                Instantiate(dustPickupPrefab, spawnPosition, Quaternion.identity);
            }
        }
        enemySpawnManager.DecrementAliveSpawnersCounter();
        Destroy(gameObject);
        Debug.Log("Enemy " + name + " died");
    }
}
