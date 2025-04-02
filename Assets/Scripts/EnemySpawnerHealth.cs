using UnityEngine;

public class EnemySpawnerHealth : EnemyHealth
{
    [SerializeField] private EnemySpawnManager enemySpawnManager;

    private void Start()
    {
        currentHealth = maxHealth;
        enemySpawnManager = GetComponentInParent<EnemySpawnManager>();
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
