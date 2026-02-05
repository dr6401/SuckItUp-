using System;
using System.Collections;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour
{
    private bool isDifficultyHardcore;
    protected float maxHealth;
    [SerializeField] protected float normalMaxHealth = 20;
    [SerializeField] protected float hardcoreMaxHealth = 50;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected GameObject dustPickupPrefab;
    [SerializeField] protected GameObject deathExplosionPrefab;
    [SerializeField] protected int minSpawnedDustParticles = 8;
    [SerializeField] protected int maxSpawnedDustParticles = 20;
    
    protected EnemyMMHealthBar EnemyHealthBar;

    [SerializeField] private Material mat;
    [SerializeField] private Color originalEmissionColor;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");


    private void Awake()
    {
        mat = GetComponentInChildren<MeshRenderer>(false).material;
        originalEmissionColor = mat.GetColor(EmissionColor);
    }

    protected virtual void Start()
    {
        isDifficultyHardcore = SettingsManager.Instance.isDifficultyHardcore;
        if (!isDifficultyHardcore) maxHealth = normalMaxHealth;
        else maxHealth = hardcoreMaxHealth;
        currentHealth = maxHealth;
        EnemyHealthBar = GetComponent<EnemyMMHealthBar>();
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
        EnemyHealthBar.UpdateBar(currentHealth, 0, maxHealth, true);
        Flash();
        //healthBarImage.fillAmount = currentHealth / maxHealth;
        //Debug.Log("Enemy " + name + " took " + damage + " damage. " + currentHealth + " health remaining");
    }

    protected virtual void Die()
    {
        if (dustPickupPrefab != null)
        {
            int amountOfSpawnedDustParticles = Random.Range(minSpawnedDustParticles, maxSpawnedDustParticles); 
            float angleStep = 360f / amountOfSpawnedDustParticles;
            for (int i = 0; i < amountOfSpawnedDustParticles; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;

                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 1f;
                spawnPosition.y += 1f; // Increase the spawn height so things don't get spawned in the ground
                GameObject spawnedDust = Instantiate(dustPickupPrefab, spawnPosition, Quaternion.identity);
                
                Rigidbody rb = spawnedDust.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float launchForce = 150;
                    Vector3 launchDirection = new Vector3(Mathf.Cos(angle),
                        0,
                        Mathf.Cos(angle)).normalized;
                    launchDirection.y = 1f;
                    
                    
                    rb.AddForce(launchDirection * launchForce, ForceMode.Acceleration);
                    Debug.Log($"Launched that b with force: {launchForce}");
                }
            }
        }

        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }
        GameEvents.OnEnemyDeath?.Invoke();
        Destroy(gameObject);
        //Debug.Log("Enemy " + name + " died!");
    }

    public void Flash()
    {
        if (mat == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        mat.SetColor(EmissionColor, Color.white);
        yield return new WaitForSeconds(0.075f);
        mat.SetColor(EmissionColor, originalEmissionColor);
    }
}
