using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] internal List<Pool> pools;
    [SerializeField] internal Dictionary<GameObject, List<GameObject>> poolDictionary;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);
        poolDictionary = new();
    }

    private void Start() => InitializeList();

    private void InitializeList()
    {
        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    internal GameObject GetPooledObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return null;

        // Find an inactive object in the pool
        foreach (GameObject obj in poolDictionary[prefab])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is found, create a new one
        GameObject newObj = ExtendObjects(prefab);
        if (newObj != null)
        {
            newObj.SetActive(true);
            return newObj;
        }

        return null;
    }

    private GameObject ExtendObjects(GameObject prefab)
    {
        //extend the size
        Pool pool = null;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].prefab == prefab)   // replace turretPrefab with your turret GameObject
            {
                pool = pools[i];
                break; //Exit the loop once the matching pool is found
            }
        }

        if (pool != null)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            pool.pooledObjects.Add(obj); //Add the new object to the pool
            poolDictionary[prefab].Add(obj);
            return obj;
        }

        return null;
    }

    internal void ReturnToPool(int _, GameObject objectToReturn) => objectToReturn.SetActive(false);

    internal int GetPoolSize(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return -1;

        return poolDictionary[prefab].Count;
    }

    internal List<GameObject> GetPooledObjects(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return null;

        return poolDictionary[prefab];
    }
}