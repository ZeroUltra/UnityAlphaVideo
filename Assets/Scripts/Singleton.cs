using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton]: " + typeof(T) + "__程序正在退出,不再返回");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton]:" + typeof(T) + "__场景中有多个相同的单利");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).Name;

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton]:" + typeof(T) + "__场景中没有改单利,创建一个");
                    }
                    else
                    {
                        Debug.Log("[Singleton] 已创建使用实例: " + _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }
    /// <summary>
    /// If this script is placed on game objec in the scene, make sure to
    /// keep only instance.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    /// it will create a buggy ghost object that will stay on the Editor scene
    /// even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

}