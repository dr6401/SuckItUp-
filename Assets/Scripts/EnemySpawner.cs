using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //NOTE: Position this object in a place around which you want the enemies to spawn in radious "spawnOffset"
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval = 3f;
    public float spawnOffset = 20f;
    private float timeSinceSpawned = 0f;
    private Transform enemiesFolder;
    void Start()
    {
        enemiesFolder = transform.Find("Enemies");
        if (enemiesFolder == null)
        {
            GameObject newFolder = new GameObject("Enemies");
            newFolder.transform.SetParent(transform);
            enemiesFolder = newFolder.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSpawned += Time.deltaTime;

        if (timeSinceSpawned > spawnInterval)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 thisObjectPosition = transform.position;
        float xOffset = Random.Range(-spawnOffset, spawnOffset);
        float zOffset = Random.Range(-spawnOffset, spawnOffset);

        Vector3 spawnPosition = new Vector3(thisObjectPosition.x + xOffset, thisObjectPosition.y, thisObjectPosition.z + zOffset);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        timeSinceSpawned = 0;

        enemy.transform.SetParent(enemiesFolder);
    }
}
