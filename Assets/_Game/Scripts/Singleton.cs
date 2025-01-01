using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public bool dontDestroy = true;
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
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        this.transform.parent = null;
        if (instance == null)
        {
            instance = this as T;
            if(dontDestroy) DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }


}