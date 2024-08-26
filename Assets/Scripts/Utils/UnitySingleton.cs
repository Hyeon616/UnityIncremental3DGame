using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //private static T instance;
    //private static readonly Lazy<T> lazyInstance = new Lazy<T>(CreateInstance);

    //public static T Instance => lazyInstance.Value;

    //private static T CreateInstance()
    //{
    //    if (instance == null)
    //    {
    //        instance = FindObjectOfType<T>();
    //        if (instance == null)
    //        {
    //            GameObject singletonObject = new GameObject(typeof(T).Name);
    //            instance = singletonObject.AddComponent<T>();
    //            DontDestroyOnLoad(singletonObject);
    //        }
    //    }
    //    return instance;
    //}

    //protected virtual void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this as T;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else if (instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                }
                    DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


}
