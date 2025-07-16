using System;
using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int attackDamage = 10;
    
    [SerializeField] private GameObject projectileExplotandoVFX;
    [SerializeField] private GameObject player;

    private PlayerHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    // Update is called once per frame

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            playerHealth.TakeDamage(attackDamage);
        }
        Instantiate(projectileExplotandoVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
