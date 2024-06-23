using System;
using UnityEngine;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly Lazy<T> lazyInstance = new Lazy<T>(CreateSingleton);

    public static T Instance
    {
        get
        {
            return lazyInstance.Value;
        }
    }

    private static T CreateSingleton()
    {
        T instance = FindObjectOfType<T>();
        if (instance == null)
        {
            GameObject singletonObject = new GameObject(typeof(T).Name);
            instance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);
        }
        return instance;
    }
}
