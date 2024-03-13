using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dev
{
    [System.Serializable]
    public class PatternBase : MonoBehaviour
    {
        [SerializeField] protected string _patternName;
        [SerializeField] protected WorldObject _owner;
        [SerializeField] protected int _waitBeatCount;
        public WorldObject Owner => _owner;
        public string PatternName => _patternName;
        public int WaitBeatCount => _waitBeatCount;
        public void Init(WorldObject owner)
        {
            _owner = owner;
        }
        public virtual void Execute()
        {
            
        }
#if UNITY_EDITOR
        public void SetName(string name)
        {
            _patternName = name;
        }
        public virtual void OnInspectorGUI()
        {
            _waitBeatCount = EditorGUILayout.IntField("WaitBeatCount", _waitBeatCount);
        }
#endif
    }
}