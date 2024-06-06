using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private List<T> pool;
    private T prefab;
    private Transform parentTransform;

    public ObjectPool(T prefab, int poolSize, Transform parentTransform = null)
    {
        this.prefab = prefab;
        this.parentTransform = parentTransform;
        pool = new List<T>();

        for (int i = 0; i < poolSize; i++)
        {
            T obj = Object.Instantiate(prefab, parentTransform);
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public T GetObject()
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

        T newObj = Object.Instantiate(prefab, parentTransform);
        newObj.gameObject.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    public void ResetPool()
    {
        foreach (T obj in pool)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public bool AllObjectsInactive()
    {
        foreach (T obj in pool)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    public List<T> ActiveObjects()
    {
        List<T> activeObjects = new List<T>();
        foreach (T obj in pool)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                activeObjects.Add(obj);
            }
        }
        return activeObjects;
    }
}
