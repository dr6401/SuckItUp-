using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject player;
    [Header("Stats")]
    [SerializeField] float minPlayerChasingDistance = 50f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 5;
    private float timeSinceAttack = 5;
    //[SerializeField] float chaseSpeed = 5f;
    private Vector3 playerPosition;
    private NavMeshAgent agent;
    [SerializeField] PlayerHealth playerHealth;
    private bool canChasePlayer = true;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerHealth = player.GetComponent<PlayerHealth>();
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
                HitPlayer();
            }
            timeSinceAttack += Time.deltaTime;
        }
    }

    private void ChasePlayer()
    {
        if ((transform.position - playerPosition).sqrMagnitude < minPlayerChasingDistance * minPlayerChasingDistance)
        {
            agent.SetDestination(playerPosition);
        }
    }

    private void HitPlayer()
    {
        //Debug.Log("Hit yo ass");
        timeSinceAttack = 0;
        playerHealth.TakeDamage(attackDamage);
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
