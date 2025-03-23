using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] private float maxHealth = 20;
    [SerializeField] private float currentHealth;
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
        Destroy(gameObject);
        Debug.Log("Enemy " + name + " died");
    }
}
