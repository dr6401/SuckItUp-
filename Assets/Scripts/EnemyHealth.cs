using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] protected float maxHealth = 20;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected GameObject dustPickupPrefab;
    [SerializeField] protected GameObject deathExplosionPrefab;
    [SerializeField] protected int minSpawnedDustParticles = 8;
    [SerializeField] protected int maxSpawnedDustParticles = 20;
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

    protected virtual void Die()
    {
        if (dustPickupPrefab != null)
        {
            for (int i = 0; i < Random.Range(minSpawnedDustParticles, maxSpawnedDustParticles); i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 1f;
                spawnPosition.y += 1f; // Increase the spawn height so things don't get spawned in the ground
                Instantiate(dustPickupPrefab, spawnPosition, Quaternion.identity);
                /*if (spawnedDust.GetComponent<Rigidbody>() != null)
                {
                    float launchForce = 1f;
                    Vector3 launchDirection = new Vector3(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1)).normalized;
                    spawnedDust.GetComponent<Rigidbody>().AddForce(launchDirection * launchForce, ForceMode.VelocityChange);
                }*/
            }
        }

        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
        Debug.Log("Enemy " + name + " died!");
    }
}
