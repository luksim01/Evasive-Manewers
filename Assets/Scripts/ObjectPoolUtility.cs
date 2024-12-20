using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolUtility
{
    public static List<GameObject> Create(string poolName, Transform poolParent, GameObject poolObject, int poolSize)
    {
        GameObject pool = new();
        pool.name = poolName;
        pool.transform.parent = poolParent;

        // instantiate gameobjects and add to gameobject pool
        List<GameObject> poolList = new List<GameObject>();
        GameObject gameObjectToPool;
        for (int i = 0; i < poolSize; i++)
        {
            gameObjectToPool = UnityEngine.Object.Instantiate(poolObject, pool.transform);
            gameObjectToPool.SetActive(false);
            poolList.Add(gameObjectToPool);
        }

        return poolList;
    }

    public static GameObject Get(int poolSize, List<GameObject> pool)
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }


    public static void Return(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
