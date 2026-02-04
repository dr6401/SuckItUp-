using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool isDifficultyHardcore = false;
    [SerializeField] GameObject player;
    [Header("Stats")]
    [SerializeField] float minPlayerChasingDistance = 50f;
    [SerializeField] float attackRange = 4f;
    [SerializeField] float attackCooldown = 1f;
    private int attackDamage;
    [SerializeField] private int normalAttackDamage = 20;
    [SerializeField] private int hardcoreAttackDamage = 20;
    private float timeSinceAttack = 5;
    //[SerializeField] float chaseSpeed = 5f;
    private Vector3 playerPosition;
    private NavMeshAgent agent;
    [SerializeField] PlayerHealth playerHealth;
    private bool canChasePlayer = true;
    private Animator animator;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerHealth = player.GetComponent<PlayerHealth>();
        animator = GetComponentInChildren<Animator>();
        isDifficultyHardcore = SettingsManager.Instance.isDifficultyHardcore;
        if (!isDifficultyHardcore) attackDamage = normalAttackDamage;
        else attackDamage = hardcoreAttackDamage;
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
                Debug.Log("Ready for attack, gonna strike");
                StartCoroutine(AttackSequence());
            }
            timeSinceAttack += Time.deltaTime;
            LookAtPlayer();
        }
    }
    
    private void LookAtPlayer()
    {
        Vector3 lookDirection = playerPosition - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void ChasePlayer()
    {
        if ((transform.position - playerPosition).sqrMagnitude < minPlayerChasingDistance * minPlayerChasingDistance && agent.isOnNavMesh)
        {
            agent.SetDestination(playerPosition);
        }
    }

    private IEnumerator AttackSequence()
    {
        Debug.Log("Hit yo ass");
        timeSinceAttack = 0;
        Debug.Log("Attack started and attack Timer reset");
        animator.Play("FluffyDustyAttack");
        yield return new WaitForSeconds(0.45f); // It takes 0.225s for the "hit" part of the animation to be played
        if ((transform.position - playerPosition).sqrMagnitude <  attackRange * attackRange * 0.76) // If the player is near enough the enemy at the time of "hit"
        // part of the animation being played he takes the dmg, if he moved away in time, he doesn't 
        {
            playerHealth.TakeDamage((int) attackDamage);
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

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.color.WithAlpha(0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }*/
}
