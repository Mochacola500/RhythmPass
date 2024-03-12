using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dev
{
    public class GameObjectPool
    {
        public class Entry
        {
            public readonly GameObject OriginPrefab;
            public readonly Stack<GameObject> Objects = new();

            public Entry(GameObject prefab)
            {
                OriginPrefab = prefab;
            }
        }

        readonly Transform m_Root;
        readonly Dictionary<string, Entry> m_Entries = new();
        readonly Dictionary<GameObject, Entry> m_ActiveObjects = new();
        long m_InstanceId;

        public GameObjectPool(Transform root)
        {
            m_Root = root ?? new GameObject("GameObjectPool").transform;
        }

        public T RequestGameObject<T>(string path, Transform parent = null) where T : Component
        {
            var go = RequestGameObject(path, parent);
            var comp = go.GetOrAddComponent<T>();
            return comp;
        }

        public GameObject RequestGameObject(string path, Transform parent = null)
        {
            GameObject prefab;
            if (!m_Entries.TryGetValue(path, out var entry))
            {
                prefab = AssetManager.Load<GameObject>(path);
                if (prefab == null)
                {
                    return null;
                }
                entry = new Entry(prefab);
                m_Entries.Add(path, entry);
            }
            // Acquire game object.
            if (entry.Objects.Count > 0)
            {
                prefab = entry.Objects.Pop();
            }
            else
            {
                prefab = GameObject.Instantiate(entry.OriginPrefab);
                prefab.name = "{0}_{1}".Format(entry.OriginPrefab.name, m_InstanceId++);
            }
            prefab.transform.SetParent(parent, worldPositionStays: false);
            prefab.SetActive(true);
            m_ActiveObjects[prefab] = entry;
            return prefab;
        }

        public void Release(GameObject go)
        {
            if (!m_ActiveObjects.TryGetValue(go, out var entry))
            {
                Debug.LogErrorFormat("Try to releasing not activated object from pool. {0}", go.name);
                GameObject.Destroy(go);
            }
            go.transform.SetParent(m_Root, worldPositionStays: false);
            go.SetActive(false);
            entry.Objects.Push(go);
            m_ActiveObjects.Remove(go);
        }
    }
}