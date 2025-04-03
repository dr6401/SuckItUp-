using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private int maxHealth = 100;
    private static int health;
    [SerializeField] private TMP_Text healthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;   
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
        healthText.text = health.ToString();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
