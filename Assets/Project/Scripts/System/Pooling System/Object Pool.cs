using System.Collections.Generic;
using UnityEngine;

// ObjectPool class is used to manage a pool of objects for reuse, to avoid the overhead of instantiating and destroying
public class ObjectPool : MonoBehaviour
{
    // Serialized fields that can be set in the Unity editor
    [SerializeField] internal List<Pool> pools; // List of pools

    [SerializeField] internal Dictionary<GameObject, List<GameObject>> poolDictionary; // Dictionary of pools

    // Called when the object is first initialized
    private void Awake()
    {
        // Register the ObjectPool as a service
        ServiceLocator.Instance.RegisterService(this, false);

        // Initialize the pool dictionary
        poolDictionary = new();
    }

    // Called before the first frame update
    private void Start() => InitializeList();

    // Initialize the list of pools
    private void InitializeList()
    {
        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new();

            // Instantiate the objects in the pool
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            // Add the pool to the pool dictionary
            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    // Get a pooled object
    internal GameObject GetPooledObject(GameObject prefab)
    {
        // Attempt to remove "(Clone)" from the prefab name to match the original prefab name in the dictionary
        string prefabName = prefab.name.Replace("(Clone)", "").Trim();

        // Find the original prefab in the poolDictionary by matching the prefabName
        GameObject originalPrefab = null;
        foreach (var key in poolDictionary.Keys)
        {
            if (key.name == prefabName)
            {
                originalPrefab = key;
                break;
            }
        }

        if (originalPrefab == null)
        {
            Logging.LogError($"ObjectPool does not contain a pool for prefab: {prefab.name}");
            return null;
        }

        // Use the originalPrefab to access the poolDictionary
        foreach (GameObject obj in poolDictionary[originalPrefab])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is found, create a new one
        GameObject newObj = ExtendObjects(originalPrefab);
        if (newObj != null)
        {
            Logging.Log($"Extended pool for prefab: {originalPrefab.name}");
            newObj.SetActive(true);
            return newObj;
        }

        Logging.LogError($"Failed to extend pool for prefab: {originalPrefab.name}. Check if the prefab is correctly assigned in the pool.");
        return null;
    }

    // Extend the objects in the pool
    private GameObject ExtendObjects(GameObject prefab)
    {
        // Find the pool for the prefab
        Pool pool = null;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].prefab == prefab)
            {
                pool = pools[i];
                break;
            }
        }

        // If the pool is found, instantiate a new object and add it to the pool
        if (pool != null)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            pool.pooledObjects.Add(obj);
            poolDictionary[prefab].Add(obj);
            return obj;
        }

        return null;
    }

    // Return an object to the pool
    internal void ReturnToPool(int _, GameObject objectToReturn) => objectToReturn.SetActive(false);

    // Get the size of a pool
    internal int GetPoolSize(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return -1;

        return poolDictionary[prefab].Count;
    }

    // Get the pooled objects for a prefab
    internal List<GameObject> GetPooledObjects(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return null;

        return poolDictionary[prefab];
    }
}