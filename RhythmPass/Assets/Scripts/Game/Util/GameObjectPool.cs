using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dev
{
    interface IObjectPool<T>
    {
        T Get();
        T Instantiate();
    }

    public class GameObjectPool<T> : IObjectPool<T> where T : Component
    {
        readonly List<T> _objectList = new List<T>();
        public GameObject Prefab { get; private set; }
        public Transform Parent { get; private set; }
        public bool IsSleepAfterInstantiate { get; private set; }
        public bool IsAutoActive { get; private set; }
        public bool IsSucceededInit => Prefab != null;

        public List<T> List => _objectList;
        public GameObjectPool()
        {
            Prefab = null;
            Parent = null;
            IsSleepAfterInstantiate = true;
            IsAutoActive = true;
        }

        public GameObjectPool(GameObject prefab, Transform parent = null, int count = 0, bool isSleepAfterInstantiate = true, bool isAutoActive = true)
        {
            Init(prefab, parent, count, isSleepAfterInstantiate, isAutoActive);
        }

        public void Init(GameObject prefab, Transform parent = null, int count = 0, bool isSleepAfterInstantiate = true, bool isAutoActive = true)
        {
            Prefab = prefab;
            Parent = parent;
            IsSleepAfterInstantiate = isSleepAfterInstantiate;
            IsAutoActive = isAutoActive;

            DestrotyAll();
            for (int i = 0; i < count; ++i)
            {
                Instantiate();
            }
        }

        public T Get()
        {
            foreach (var item in _objectList)
            {
                if (item.gameObject.activeSelf == false)
                {
                    if (IsAutoActive)
                        item.gameObject.SetActive(true);

                    return item;
                }
            }

            T result = Instantiate();
            if (IsAutoActive)
                result.gameObject.SetActive(true);
            return result;
        }

        public T Instantiate()
        {
            GameObject newObject = null;
            if (Parent)
                newObject = GameObject.Instantiate(Prefab, Parent);
            else
                newObject = GameObject.Instantiate(Prefab);

            T result = newObject.GetComponent<T>();
            if (result == null)
            {
                throw new System.Exception("No Component : " + typeof(T).Name);
            }
            _objectList.Add(result);

            if (IsSleepAfterInstantiate)
                result.gameObject.SetActive(false);

            return result;
        }

        public void Foreach(Action<T> query)
        {
            foreach (var item in _objectList)
            {
                query.Invoke(item);
            }
        }

        public void SleepAll()
        {
            foreach (var item in _objectList)
            {
                item.gameObject.SetActive(false);
            }
        }
        public T First()
        {
            if (_objectList.Count == 0)
                return null;
            return _objectList[0];
        }

        public void DestrotyAll()
        {
            foreach (var item in _objectList)
            {
                GameObject.Destroy(item.gameObject);
            }
            _objectList.Clear();
        }

        public int GetActiveCount()
        {
            int result = 0;
            for (int i = 0; i < _objectList.Count; ++i)
            {
                if (_objectList[i].gameObject.activeSelf)
                    result++;
            }
            return result;
        }
    }
}
