using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] private float maxHealth = 20;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject dustPickupPrefab;
    [SerializeField] private int minSpawnedDustParticles = 8;
    [SerializeField] private int maxSpawnedDustParticles = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
        //Debug.Log("Current hp:" + currentHealth + "/" + maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy " + name + " took " + damage + " damage. " + currentHealth + " health remaining");
    }

    private void Die()
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

        Destroy(gameObject);
        Debug.Log("Enemy " + name + " died");
    }
}
