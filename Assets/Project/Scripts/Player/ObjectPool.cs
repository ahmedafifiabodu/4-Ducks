using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;

    [System.Serializable]
    public class Pool
    {
        public int tag;
        public GameObject prefab;
        public int size;
        public List<GameObject> pooledObjects;
    }

    public List<Pool> pools;
    public Dictionary<int, List<GameObject>> poolDictionary;

    void Awake()
    {
        SharedInstance = this;
        poolDictionary = new Dictionary<int, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject GetPooledObject(int tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        // Find an inactive object in the pool
        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        //extend the size
        Pool pool = null;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].tag == tag && tag!=2)   // tag != 2 to disable extend for turret
            {
                pool = pools[i];
                break; // Exit the loop once the matching pool is found
            }
        }

        if (pool != null)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(true);
            pool.pooledObjects.Add(obj); // Add the new object to the pool
            return obj;
        }
       
        return null;
    }

    public void ReturnToPool(int _, GameObject objectToReturn)=>objectToReturn.SetActive(false);
    
}
