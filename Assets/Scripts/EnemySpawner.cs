using UnityEngine;
using UnityEngine.AI;

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
        Transform parent = transform.parent;
        GameObject folder =GameObject.Find("EnemiesFolder");
        if (folder == null)
        {
            Debug.Log("Folder is null, creating new folder");
            folder = new GameObject("EnemiesFolder");
            folder.transform.parent = parent;
        }
        enemiesFolder = folder.transform;
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

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 2f, NavMesh.AllAreas))
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            timeSinceSpawned = 0;
            enemy.transform.SetParent(enemiesFolder);
        }
        else
        {
            Debug.Log("Couldn't spawn enemy on NavMesh, trying to spawn again");
        }
    }
}
