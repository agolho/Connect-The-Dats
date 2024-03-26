using UnityEngine;

namespace Tools
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static volatile T _instance;

        public static T Instance
        {
            get
            {
                if ((bool) (Object) MonoSingleton<T>._instance)
                    return MonoSingleton<T>._instance;
                MonoSingleton<T>._instance = Object.FindObjectOfType<T>();
                if (!(bool) (Object) MonoSingleton<T>._instance)
                    MonoSingleton<T>._instance = new GameObject(nameof (T)).AddComponent<T>();
                return MonoSingleton<T>._instance;
            }
        }
    }
}