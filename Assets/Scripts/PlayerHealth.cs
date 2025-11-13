using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private int maxHealth = 200;
    private static int health;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text tryAgainText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WeaponHandler weaponHandler;
    
    // AUGMENTS STUFF
    private int healFromVampireAmount = 0;
    private float damageReduction = 1f;
    private float dustStormDamageReduction = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        if (weaponHandler == null)
        {
            weaponHandler = GetComponent<WeaponHandler>();
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
        gameManager.gameOver = true;
        gameManager.gameLost = true;
        GameEvents.OnPlayerDeath?.Invoke();
        gameOverText.gameObject.SetActive(true);
        var restartAction = SettingsManager.controls.asset.FindAction("Restart");
        if (restartAction != null)
        {
            var path = restartAction.bindings[0].effectivePath;
            tryAgainText.text = $"Press {InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice)} to Try Again!";   
        }
        tryAgainText.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        float dustStormDamageReductionMultiplier = weaponHandler.isAlreadySucking ? dustStormDamageReduction : 1f;
        health -= (int) (damage * damageReduction * dustStormDamageReductionMultiplier);
        GameEvents.OnDamageTaken?.Invoke();
        Debug.Log("taken " + (int)(damage * damageReduction * dustStormDamageReductionMultiplier) + " damage");
    }

    public void ApplyDirtyVampire(int healAmount)
    {
        healFromVampireAmount += healAmount;
    }

    public void ApplyDirtyDracula(int healAmount)
    {
        healFromVampireAmount += healAmount;
    }

    private void HealFromVampire()
    {
        if (weaponHandler.ReturnCurrentAmmo() >= 100) health += healFromVampireAmount;
    }

    public void ApplyColossalCleaner(float dmgReduction)
    {
        damageReduction *= dmgReduction;
    }
    
    
    public void ApplyDustStormShell(float dmgReduction)
    {
        dustStormDamageReduction = dmgReduction;
    }

    private void OnEnable()
    {
        GameEvents.OnSuckDust += HealFromVampire;
        GameEvents.OnLevelTimeRanOut += Die;
    }
    private void OnDisable()
    {
        GameEvents.OnSuckDust -= HealFromVampire;
        GameEvents.OnLevelTimeRanOut -= Die;
    }
}
