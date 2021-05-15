
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public static T Load<T>(string path) where T : Object
    {
        var asset = Resources.Load<T>(path);
        return asset;
    }
}
