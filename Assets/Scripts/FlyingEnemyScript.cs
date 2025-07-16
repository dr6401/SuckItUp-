using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class FlyingPatrolEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject player;
    [Header("Stats")]
    [SerializeField] float minPlayerChasingDistance = 50f;
    [SerializeField] float attackRange = 40f;
    [SerializeField] float attackCooldown = 5f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private int projectileSpeed = 200;
    private float timeSinceAttack = 2;
    [SerializeField] private GameObject attackProjectilePrefab;
    [SerializeField] private Transform FiringPoint;
    private Rigidbody attackProjectileRigidbody;
    private AttackProjectile attackProjectileScript;
    
    private Vector3 playerPosition;
    [SerializeField] private Camera playerCamera;
    private bool canChasePlayer = true;
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerCamera = player.GetComponentInChildren<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canChasePlayer)
        {
            playerPosition = player.transform.position;
            ChasePlayer();
            if ((transform.position - playerPosition).sqrMagnitude < attackRange * attackRange && timeSinceAttack > attackCooldown)
            {
                AttackPlayer();
            }
            timeSinceAttack += Time.deltaTime;
        }
    }

    private void ChasePlayer()
    {
        if ((transform.position - playerPosition).sqrMagnitude < minPlayerChasingDistance * minPlayerChasingDistance)
        {
            //agent.SetDestination(playerPosition);
        }
    }

    private void AttackPlayer()
    {
        timeSinceAttack = 0;
        FireAttackAtPlayer();
    }

    private void FireAttackAtPlayer()
    {
        Vector3 attackProjectileSpawnPositionVector3 = new Vector3(FiringPoint.position.x,
            FiringPoint.position.y, FiringPoint.position.z);
        Vector3 playerDirection = new Vector3( // Attack the playerCamera position not the playerPosition
            playerCamera.transform.position.x - attackProjectileSpawnPositionVector3.x,
            playerCamera.transform.position.y - 1.5f - attackProjectileSpawnPositionVector3.y,// 1.5f here is the Y offset since without it projectile flies above the target for some reason
            playerCamera.transform.position.z - attackProjectileSpawnPositionVector3.z
        ).normalized;
        GameObject attackProjectile =
            Instantiate(attackProjectilePrefab, attackProjectileSpawnPositionVector3, Quaternion.LookRotation(playerDirection));
        if (attackProjectile.GetComponent<Rigidbody>() != null)
        {
            attackProjectileRigidbody = attackProjectile.GetComponent<Rigidbody>();
        }
       attackProjectileRigidbody.AddForce(playerDirection * projectileSpeed, ForceMode.Impulse);
        if (attackProjectile.GetComponent<AttackProjectile>() != null)
        {
            attackProjectileScript = attackProjectile.GetComponent<AttackProjectile>();
            attackProjectileScript.attackDamage = attackDamage;// Assign the projectile that much dmg as set in this (FlyingEnemyScript) script
        }
        Debug.DrawLine(attackProjectileSpawnPositionVector3, playerCamera.transform.position, Color.red, 1f);
        
    }

    private void StopChasingPlayer()
    {
        canChasePlayer = false;
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += StopChasingPlayer;
    }
    
    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= StopChasingPlayer;
    }
}