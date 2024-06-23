using System;

public abstract class Singleton<T> where T : class
{
    private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => CreateInstanceOfT());

    public static T Instance
    {
        get
        {
            return lazyInstance.Value;
        }
    }

    private static T CreateInstanceOfT()
    {
        return (T)Activator.CreateInstance(typeof(T), true);
    }
}