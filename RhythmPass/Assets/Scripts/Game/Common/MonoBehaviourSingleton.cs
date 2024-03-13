using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if(null == _instance)
                {
                    _instance = FindObjectOfType<T>();

                    if(null == _instance)
                    {
                        GameObject newObject = new GameObject(typeof(T).Name);
                        _instance = newObject.AddComponent<T>();
                    }

                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }
    }

    public class Singleton<T> where T : class,new()
    {
        static T _instance;

        public static T Instance
        {
            get
            {
                if(null == _instance)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}