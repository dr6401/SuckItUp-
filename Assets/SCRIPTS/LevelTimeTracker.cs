using System;
using UnityEngine;
using TMPro;
public class LevelTimeTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private float normalMaxTime = 180f;
    [SerializeField] private float hardcoreMaxTime = 180f;
    private bool isDifficultyHardcore;
    private float timeLeft;
    private bool timeRanOut = false;
    private bool signaledLowTimer = false;

    private bool canLevelTimerDecrease = false;

    private void Awake()
    {
        Debug.Log("LevelTimeTracker AWAKED");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDifficultyHardcore = SettingsManager.Instance.isDifficultyHardcore;
        if (!isDifficultyHardcore) timeLeft = normalMaxTime;
        else timeLeft = hardcoreMaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRanOut) return;
        if (Time.timeScale >= 1 && !canLevelTimerDecrease)
        {
            timeLeft -= Time.deltaTime;   
        }
        timeLeft = Mathf.Max(0, timeLeft); // Never goes below 0
        UpdateTimerUI();
        if (timeLeft < 20 && !signaledLowTimer)
        {
            signaledLowTimer = true;
            GameEvents.OnLowLevelTimer?.Invoke();
        }
        if (timeLeft <= 0)
        {
            //timerText.text = "";
            timeRanOut = true;
            GameEvents.OnLevelTimeRanOut?.Invoke();
            gameOverText.text = "Mom came home and the house isn't clean!!!\n \n <size=150%>YOU LOSE!</size=150%>";
        }
    }

    private void UpdateTimerUI()
    {
        if (timeLeft >= 60)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            timerText.text = $"{minutes:00}:{seconds:00}"; 
        }
        else
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            float seconds = timeLeft % 60;
            timerText.text = $"{minutes:0}:{seconds:00.0}"; 
        }
    }

    private void StopLevelTimer()
    {
        canLevelTimerDecrease = true;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += StopLevelTimer;
        GameEvents.OnLevelCompleted += StopLevelTimer;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= StopLevelTimer;
        GameEvents.OnLevelCompleted -= StopLevelTimer;
    }
}
