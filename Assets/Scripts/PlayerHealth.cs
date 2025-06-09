using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private int maxHealth = 100;
    private static int health;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text tryAgainText;
    [SerializeField] private GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_gameManager == null)
        {
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        
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
        _gameManager.gameOver = true;
        GameEvents.OnPlayerDeath?.Invoke();
        gameOverText.GameObject().SetActive(true);
        tryAgainText.GameObject().SetActive(true);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
