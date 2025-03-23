using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject player;
    [SerializeField] float minPlayerChasingDistance = 50f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackCooldown = 3f;
    private float timeSinceAttack = 2;
    //[SerializeField] float chaseSpeed = 5f;
    private Vector3 playerPosition;
    private Rigidbody rb;
    private NavMeshAgent agent;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.transform.position;
        if (timeSinceAttack > attackCooldown)
        {
            ChasePlayer();
            if ((transform.position - playerPosition).sqrMagnitude < attackRange * attackRange)
            {
                HitPlayer();
            }
        }

        timeSinceAttack += Time.deltaTime;
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
    }
}
