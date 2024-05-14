using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public int tag;
    public GameObject prefab;
    public int size;
    public List<GameObject> pooledObjects;
}