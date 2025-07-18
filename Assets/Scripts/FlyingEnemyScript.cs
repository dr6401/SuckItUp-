using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FlyingPatrolEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject player;
    [Header("Stats")]
    [SerializeField] private float movementSpeed = 100f;
    [SerializeField] float minPlayerChasingDistance = 50f;
    [SerializeField] float attackRange = 40f;
    [SerializeField] float attackCooldown = 5f;
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private int projectileSpeed = 200;
    [SerializeField] private float wanderingDistance = 30f;
    private float timeSinceAttack = 2;
    [SerializeField] private GameObject attackProjectilePrefab;
    [SerializeField] private Transform FiringPoint;
    
    private Rigidbody rb;
    private Rigidbody attackProjectileRigidbody;
    private AttackProjectile attackProjectileScript;
    private float timeSinceWanderedAround = 5f;
    private float wanderingAroundInterval = 5f;
    [SerializeField] private float yFrequency = 5f;
    [SerializeField] private float flyingAmplitude = 1.5f;
    [SerializeField] private float minFlyingHeight = -24f; // For each level check where should the lowest flyable height be and assign it
    public bool followPlayersHeight = true; // OR you can just set this to true and enemies will use players Y position as minFlyingHeight
    [SerializeField] private GameObject minFlyingHeightSetter;
    
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

        if (minFlyingHeightSetter == null)
        {
            minFlyingHeightSetter = GameObject.FindGameObjectWithTag("MinFlyingHeightSetter");
        }

        if (minFlyingHeightSetter != null)
        {
            minFlyingHeight = minFlyingHeightSetter.transform.position.y;
        }
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canChasePlayer)
        {
            timeSinceWanderedAround += Time.deltaTime;
            playerPosition = player.transform.position;
            if ((transform.position - playerPosition).sqrMagnitude < attackRange * attackRange && timeSinceAttack > attackCooldown)
            {
                AttackPlayer();
            }
            timeSinceAttack += Time.deltaTime;

            if (followPlayersHeight)
            {
                minFlyingHeight = playerPosition.y;
            }
        }
    }

    private void FixedUpdate()
    {
        LookAtPlayer();
        FlyUpAndDown();
        if (timeSinceWanderedAround >= wanderingAroundInterval)
        {
            WanderAround();   
        }
        ChasePlayer();
    }

    private void LookAtPlayer()
    {
        Vector3 lookDirection = playerPosition - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void WanderAround()
    {
        int layerMask = ~LayerMask.GetMask("Enemy", "Projectile");
        Vector3 randomDirection = Random.onUnitSphere.normalized; //Normalized meaning every movement will be the same length For not same length random movement just remove the .normalized

        if (transform.position.y <= minFlyingHeight) // If enemy is near the floor
        {
            randomDirection.y = Mathf.Abs(randomDirection.y); // Make sure the next direction will be upwards not downwards
            randomDirection.y *= 2f; // Make sure that y direction is not only positive but actually points steeply high
            randomDirection.Normalize();
            Debug.Log("Enemy was low, so his next vector will be: X: " + randomDirection.x + " + Y: " + randomDirection.y + " + Z: "+ randomDirection.z);
        }
                                                                  
        if (!Physics.Raycast(transform.position, randomDirection, wanderingDistance, layerMask)) {
            rb.AddForce(randomDirection * movementSpeed, ForceMode.Force);
            timeSinceWanderedAround = 0f;
        }
    }

    private void FlyUpAndDown()
    {
        float verticalVelocity = Mathf.Cos(Time.time * yFrequency) * flyingAmplitude * yFrequency; // We add yFrequency at the end since we use .Cos which is a derivative of sin
        Vector3 velocity = rb.linearVelocity;

        velocity.y += verticalVelocity;
        rb.linearVelocity = velocity;
        
        // More clunky, try if suicidal
        //float forceY = Mathf.Cos(Time.time * yFrequency) * flyingAmplitude * yFrequency;
        //rb.AddForce(new Vector3(0, forceY, 0), ForceMode.Acceleration);

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