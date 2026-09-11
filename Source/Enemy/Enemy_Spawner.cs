using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> tierOneEnemies = new List<Enemy>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private bool autoSpawn = true;
    [SerializeField] private int initialEnemyCount = 5;

    private float spawnTimer;
    private int currentEnemyCount;
    private Dictionary<Enemy, Transform> enemySpawnPoints = new Dictionary<Enemy, Transform>();

    private void Start()
    {
        spawnTimer = spawnInterval;

        // Spawn initial enemies
        for (int i = 0; i < initialEnemyCount; i++)
        {
            SpawnRandomEnemy();
        }
    }

    private void Update()
    {
        if (autoSpawn && currentEnemyCount < maxEnemies)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnRandomEnemy();
                spawnTimer = spawnInterval;
            }
        }
    }

    private void SpawnRandomEnemy()
    {
        if (tierOneEnemies.Count == 0 || spawnPoints.Count == 0)
            return;

        // Get available spawn points (not occupied)
        List<Transform> availableSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!enemySpawnPoints.ContainsValue(spawnPoint))
            {
                availableSpawnPoints.Add(spawnPoint);
            }
        }

        // If no spawn points available, return
        if (availableSpawnPoints.Count == 0)
            return;

        Enemy randomEnemy = tierOneEnemies[Random.Range(0, tierOneEnemies.Count)];
        Transform randomSpawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];

        SpawnEnemyAt(randomEnemy, randomSpawnPoint);
    }

    private void SpawnEnemyAt(Enemy enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab == null || spawnPoint == null)
            return;

        // Check if spawn point is already occupied
        if (enemySpawnPoints.ContainsValue(spawnPoint))
            return;

        Enemy spawnedEnemy = ObjectPoolManager.SpawnObject<Enemy>(enemyPrefab, spawnPoint.position, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDestroyed;
        enemySpawnPoints[spawnedEnemy] = spawnPoint;
        currentEnemyCount++;
    }

    private void OnEnemyDestroyed(Enemy enemy)
    {
        currentEnemyCount--;
        enemy.OnDeath -= OnEnemyDestroyed;
        
        // Free up the spawn point
        if (enemySpawnPoints.ContainsKey(enemy))
        {
            enemySpawnPoints.Remove(enemy);
        }
    }
}
