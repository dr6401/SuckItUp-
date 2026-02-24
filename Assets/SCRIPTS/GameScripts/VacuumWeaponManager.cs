using System;
using System.Collections.Generic;
using UnityEngine;

public class VacuumWeaponManager : MonoBehaviour
{
    private float damageInterval = 0.75f;
    private float timeSinceDamage = 0f;
    private int damage = 1;
    [SerializeField] private CapsuleCollider vacuumWeaponDamageCollider;
    
    public static Action<Collider, int> OnEnemyStayInVacuumZone;
    [SerializeField] private WeaponHandler weaponHandler;
    private HashSet<EnemyHealth> enemiesInRange = new();
    private HashSet<EnemySpawnerHealth> spawnersInRange = new();

    private void Start()
    {
        vacuumWeaponDamageCollider = GetComponent<CapsuleCollider>();
        if (weaponHandler != null) damage = (int) weaponHandler.vacuumWeaponDamage;
    }

    private void Update()
    {
        timeSinceDamage += Time.deltaTime;
        if (timeSinceDamage >= damageInterval && (enemiesInRange.Count > 0 || spawnersInRange.Count > 0))
        {
            enemiesInRange.RemoveWhere(enemy => enemy == null || !enemy.gameObject.activeInHierarchy); // Enemies
            foreach (var enemy in enemiesInRange)
            {
                enemy.TakeDamage(damage);
            }
            spawnersInRange.RemoveWhere(enemySpawner => enemySpawner == null || !enemySpawner.gameObject.activeInHierarchy); // Spawners
            foreach (var enemySpawner in spawnersInRange)
            {
                enemySpawner.TakeDamage(damage);
            }
            
            timeSinceDamage = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("EnemySpawner")) return;
        if (other.TryGetComponent(out EnemyHealth health))
        {
            enemiesInRange.Add(health);
        }
        if (other.TryGetComponent(out EnemySpawnerHealth spawnerHealth))
        {
            enemiesInRange.Add(spawnerHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy") && !other.CompareTag("EnemySpawner")) return;
        if (other.TryGetComponent(out EnemyHealth health))
        {
            enemiesInRange.Remove(health);
        }
        if (other.TryGetComponent(out EnemySpawnerHealth spawnerHealth))
        {
            enemiesInRange.Remove(spawnerHealth);
        }
    }

    public void EnableCollider(bool enable)
    {
        vacuumWeaponDamageCollider.enabled = enable;
        if (!enable)
        {
            enemiesInRange.Clear();
            spawnersInRange.Clear();
        }
        //Debug.Log($"Enabled collider: {enable}");
    }
    
    public void ExtendVacuumRange(float multiplier)
    {
        vacuumWeaponDamageCollider.height *= multiplier;
        vacuumWeaponDamageCollider.radius *= multiplier;
    }
    
    
    private void OnEnable()
    {
        timeSinceDamage = 10f; // Make vacuum deal damage when enabled and start sucking
    }

    private void OnDisable()
    {
        enemiesInRange.Clear();
        spawnersInRange.Clear();
    }
}
