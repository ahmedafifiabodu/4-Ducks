using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    // Serialized fields that can be set in the Unity editor
    [SerializeField] private List<GameObject> _enemiesPrefabs; // List of enemy prefabs

    [SerializeField] private float _spawnDelay = 1f; // Delay between spawns
    [SerializeField] private SpawnMethod _spawnMethod = SpawnMethod.RoundRobin; // Method to spawn enemies

    private List<GameObject> _enemiesPrefab; // List of enemy prefabs
    private NavMeshTriangulation _navMeshTriangulation; // NavMesh triangulation
    private int _numberOfEnemiesToSpawn; // Number of enemies to spawn
    private WaitForSeconds _spawnDelayWaitForSeconds; // WaitForSeconds object for spawn delay
    private int _navMeshVerticesLength; // Length of the NavMesh vertices

    private ObjectPool _objectPool; // Reference to the ObjectPool

    // Called before the first frame update
    private void Start()
    {
        // Get the ObjectPool from the ServiceLocator
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        _spawnDelayWaitForSeconds = new WaitForSeconds(_spawnDelay);
        _enemiesPrefab = new List<GameObject>();

        // Initialize the enemies prefab list and the number of enemies to spawn
        foreach (GameObject _enemyPrefab in _enemiesPrefabs)
        {
            _numberOfEnemiesToSpawn += _objectPool.GetPoolSize(_enemyPrefab);
            _enemiesPrefab.AddRange(_objectPool.GetPooledObjects(_enemyPrefab));
        }

        // Calculate the NavMesh triangulation
        _navMeshTriangulation = NavMesh.CalculateTriangulation();
        _navMeshVerticesLength = _navMeshTriangulation.vertices.Length;

        // Start the SpawnEnemies coroutine
        StartCoroutine(SpawnEnemies());
    }

    // Coroutine to spawn enemies
    private IEnumerator SpawnEnemies()
    {
        int _spawnedEnemies = 0;

        // Spawn enemies until the number of enemies to spawn is reached
        while (_spawnedEnemies < _numberOfEnemiesToSpawn)
        {
            // Spawn an enemy based on the spawn method
            if (_spawnMethod == SpawnMethod.RoundRobin)
                SpawnRoundRobinEnemy(_spawnedEnemies);
            else if (_spawnMethod == SpawnMethod.Random)
                SpawnRandomEnemy();

            _spawnedEnemies++;

            yield return _spawnDelayWaitForSeconds;
        }
    }

    // Spawn an enemy in a round robin manner
    private void SpawnRoundRobinEnemy(int _spawnedEnemies)
    {
        int _enemyIndex = _spawnedEnemies % _enemiesPrefab.Count;

        DoSpawnEnemy(_enemyIndex);
    }

    // Spawn a random enemy
    private void SpawnRandomEnemy() => DoSpawnEnemy(Random.Range(0, _enemiesPrefab.Count));

    // Spawn an enemy
    private void DoSpawnEnemy(int _enemyIndex)
    {
        GameObject _enemyPool = _enemiesPrefab[_enemyIndex];

        // If the enemy pool is not null and the enemy has an Enemy component
        if (_enemyPool != null && _enemyPool.TryGetComponent(out Enemy _enemy))
        {
            Vector3 spawnPosition = SampleNavMeshPosition();

            // If the spawn position is not zero
            if (spawnPosition != Vector3.zero)
            {
                // Warp the enemy to the spawn position and enable it
                _enemy.NavMeshAgent.Warp(spawnPosition);
                _enemy.NavMeshAgent.enabled = true;
                _enemy.gameObject.SetActive(true);
                _enemy.NavmeshEnemyMovment._navMeshTriangulation = _navMeshTriangulation;
                _enemy.NavmeshEnemyMovment.Spawn();
            }
            else
                Logging.LogError("Failed to sample position");
        }
        else
            Logging.LogError("Enemy is null or does not have an Enemy component");
    }

    // Sample a position on the NavMesh
    private Vector3 SampleNavMeshPosition()
    {
        int _vertexIndex = Random.Range(0, _navMeshVerticesLength);

        // If a position on the NavMesh can be sampled
        if (NavMesh.SamplePosition(_navMeshTriangulation.vertices[_vertexIndex], out NavMeshHit _navMeshHit, 2.0f, -1))
            return _navMeshHit.position;

        return Vector3.zero;
    }
}