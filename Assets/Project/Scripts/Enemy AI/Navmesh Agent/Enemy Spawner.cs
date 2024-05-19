using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<int> _enemyTags;
    [SerializeField] private float _spawnDelay = 1f;
    [SerializeField] private SpawnMethod _spawnMethod = SpawnMethod.RoundRobin;

    private List<GameObject> _enemiesPrefab;
    private NavMeshTriangulation _navMeshTriangulation;
    private int _numberOfEnemiesToSpawn;
    private WaitForSeconds _spawnDelayWaitForSeconds;
    private int _navMeshVerticesLength;

    private ObjectPool _objectPool;

    private void Start()
    {
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        _spawnDelayWaitForSeconds = new WaitForSeconds(_spawnDelay);
        _enemiesPrefab = new List<GameObject>();

        foreach (int tag in _enemyTags)
        {
            _numberOfEnemiesToSpawn += _objectPool.GetPoolSize(tag);
            _enemiesPrefab.AddRange(_objectPool.GetPooledObjects(tag));
        }

        _navMeshTriangulation = NavMesh.CalculateTriangulation();
        _navMeshVerticesLength = _navMeshTriangulation.vertices.Length;

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int _spawnedEnemies = 0;

        while (_spawnedEnemies < _numberOfEnemiesToSpawn)
        {
            if (_spawnMethod == SpawnMethod.RoundRobin)
                SpawnRoundRobinEnemy(_spawnedEnemies);
            else if (_spawnMethod == SpawnMethod.Random)
                SpawnRandomEnemy();

            _spawnedEnemies++;

            yield return _spawnDelayWaitForSeconds;
        }
    }

    private void SpawnRoundRobinEnemy(int _spawnedEnemies)
    {
        int _enemyIndex = _spawnedEnemies % _enemiesPrefab.Count;

        DoSpawnEnemy(_enemyIndex);
    }

    private void SpawnRandomEnemy() => DoSpawnEnemy(Random.Range(0, _enemiesPrefab.Count));

    private void DoSpawnEnemy(int _enemyIndex)
    {
        GameObject _enemyPool = _enemiesPrefab[_enemyIndex];

        if (_enemyPool != null && _enemyPool.TryGetComponent(out Enemy _enemy))
        {
            Vector3 spawnPosition = SampleNavMeshPosition();

            if (spawnPosition != Vector3.zero)
            {
                _enemy.NavMeshAgent.Warp(spawnPosition);
                _enemy.NavMeshAgent.enabled = true;
                _enemy.gameObject.SetActive(true);
                _enemy.NavmeshEnemyMovment.StartChasing();
            }
            else
                Logging.LogError("Failed to sample position");
        }
        else
            Logging.LogError("Enemy is null or does not have an Enemy component");
    }

    private Vector3 SampleNavMeshPosition()
    {
        int _vertexIndex = Random.Range(0, _navMeshVerticesLength);

        if (NavMesh.SamplePosition(_navMeshTriangulation.vertices[_vertexIndex], out NavMeshHit _navMeshHit, 2.0f, -1))
            return _navMeshHit.position;

        return Vector3.zero;
    }
}