using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnerList> _enemySpawnerLists; // List of EnemySpawnerList

    private ObjectPool _objectPool; // Reference to the ObjectPool

    // Called before the first frame update
    private void Start()
    {
        // Get the ObjectPool from the ServiceLocator
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        // Start the SpawnEnemies coroutine for each EnemySpawnerList
        foreach (var spawnerList in _enemySpawnerLists)
        {
            StartCoroutine(SpawnEnemies(spawnerList));
        }
    }

    // Coroutine to spawn enemies
    private IEnumerator SpawnEnemies(EnemySpawnerList spawnerList)
    {
        // Ensure the NavMesh for the specific NavMeshSurface is built
        spawnerList.NavMeshSurface.BuildNavMesh();

        // Use the SpawnDelay from the current spawnerList
        WaitForSeconds spawnDelayWaitForSeconds = new(spawnerList.SpawnDelay);

        foreach (EnemyPrefabAndSize enemyPrefabAndSize in spawnerList.EnemiesPrefabs)
        {
            List<GameObject> enemiesPrefab = new();

            // Populate the enemiesPrefab list with the required number of prefabs
            for (int i = 0; i < enemyPrefabAndSize.Size; i++)
            {
                // Inside the SpawnEnemies coroutine of EnemySpawner
                GameObject pooledObject = _objectPool.GetPooledObject(enemyPrefabAndSize.EnemyPrefab);
                if (pooledObject != null)
                {
                    enemiesPrefab.Add(pooledObject);
                }
                else
                {
                    Logging.LogError("Failed to get a pooled object. Is the pool size configured correctly?");
                    yield break; // Exit if we can't get enough objects from the pool
                }
            }

            int spawnedEnemies = 0;

            // Spawn enemies until the number specified in enemyPrefabAndSize.Size is reached
            while (spawnedEnemies < enemyPrefabAndSize.Size)
            {
                // Use the SpawnMethod from the current spawnerList
                if (spawnerList.SpawnMethod == SpawnMethod.RoundRobin)
                    SpawnRoundRobinEnemy(spawnedEnemies, enemiesPrefab, spawnerList.NavMeshSurface);
                else if (spawnerList.SpawnMethod == SpawnMethod.Random)
                    SpawnRandomEnemy(enemiesPrefab, spawnerList.NavMeshSurface);

                spawnedEnemies++;

                yield return spawnDelayWaitForSeconds;
            }
        }
    }

    // Correct the method signature to accept NavMeshSurface
    private void SpawnRoundRobinEnemy(int spawnedEnemies, List<GameObject> enemiesPrefab, NavMeshSurface surface)
    {
        int enemyIndex = spawnedEnemies % enemiesPrefab.Count;
        DoSpawnEnemy(enemyIndex, enemiesPrefab, surface);
    }

    // Correct the method signature to accept NavMeshSurface
    private void SpawnRandomEnemy(List<GameObject> enemiesPrefab, NavMeshSurface surface) => DoSpawnEnemy(Random.Range(0, enemiesPrefab.Count), enemiesPrefab, surface);

    // Spawn an enemy
    private void DoSpawnEnemy(int enemyIndex, List<GameObject> enemiesPrefab, NavMeshSurface surface)
    {
        GameObject enemyPool = enemiesPrefab[enemyIndex];

        if (enemyPool != null && enemyPool.TryGetComponent(out Enemy enemy))
        {
            Vector3 spawnPosition = SampleNavMeshPosition(surface);

            if (spawnPosition != Vector3.zero)
            {
                enemy.NavMeshAgent.Warp(spawnPosition);
                enemy.NavMeshAgent.enabled = true;
                enemy.gameObject.SetActive(true);
                enemy.NavmeshEnemyMovment.Spawn();
            }
            else
                Logging.LogError("Failed to sample position");
        }
        else
            Logging.LogError("Enemy is null or does not have an Enemy component");
    }

    // Sample a position on the NavMesh
    private Vector3 SampleNavMeshPosition(NavMeshSurface surface)
    {
        // Assuming you want to sample a position within a certain range around the center of the surface
        Vector3 center = surface.transform.position;
        float range = 10.0f; // Example range within which to sample a position

        // Try to find a valid position on the NavMesh within the specified range
        if (NavMesh.SamplePosition(center, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // If no valid position was found, return Vector3.zero or handle accordingly
        return Vector3.zero;
    }
}