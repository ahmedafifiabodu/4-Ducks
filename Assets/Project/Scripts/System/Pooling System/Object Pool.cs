using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] internal List<Pool> pools;
    [SerializeField] internal Dictionary<int, List<GameObject>> poolDictionary;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);
        poolDictionary = new Dictionary<int, List<GameObject>>();
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

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    internal GameObject GetPooledObject(int tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        // Find an inactive object in the pool
        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is found, create a new one
        GameObject newObj = ExtendObjects(tag);
        if (newObj != null)
        {
            newObj.SetActive(true);
            return newObj;
        }

        return null;
    }

    private GameObject ExtendObjects(int tag)
    {
        //extend the size
        Pool pool = null;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].tag == tag && tag != 2)   // tag != 2 to disable extend for Turret
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
            poolDictionary[tag].Add(obj);
            return obj;
        }

        return null;
    }

    internal void ReturnToPool(int _, GameObject objectToReturn) => objectToReturn.SetActive(false);

    internal int GetPoolSize(int tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            return -1;

        return poolDictionary[tag].Count;
    }

    internal List<GameObject> GetPooledObjects(int tag)
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        return poolDictionary[tag];
    }
}