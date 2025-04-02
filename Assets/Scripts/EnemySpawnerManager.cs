using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private TMP_Text spawnerText;
    [SerializeField] private int maxSpawners;
    private int aliveSpawners;
    List<GameObject> spawners = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemySpawner[] enemySpawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach(EnemySpawner enemySpawner in enemySpawners)
        {
            spawners.Add(enemySpawner.gameObject);
        }
        maxSpawners = spawners.Count;
        aliveSpawners = maxSpawners;
    }

    // Update is called once per frame
    void Update()
    {
        spawnerText.text = aliveSpawners + "/" + maxSpawners;
    }

    public void DecrementAliveSpawnersCounter()
    {
        aliveSpawners--;
        Debug.Log("DecrementAliveSpawnersCounter called, aliveSpawners should be reduced");
    }
}
