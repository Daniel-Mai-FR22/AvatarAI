using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T instance;

    private static object _lock = new object();

    public static T Instance {
        get {
            if (instance == null) {
                GameObject obj = new GameObject();
                obj.AddComponent<T>();
                instance = obj.GetComponent<T>();
            }
            return instance;
        }
    }

    protected virtual void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
}