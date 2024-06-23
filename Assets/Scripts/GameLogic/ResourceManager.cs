using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public APISettings APISettings { get; private set; }

    private ResourceManager()
    {
        APISettings = Resources.Load<APISettings>("APISettings");
    }
}
