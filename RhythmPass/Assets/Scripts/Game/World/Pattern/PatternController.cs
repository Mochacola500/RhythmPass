using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class PatternController : MonoBehaviour
    {
        public bool IsPause { get; private set; }
        [SerializeField] List<PatternBase> _patternList;
        WorldObject _owner;
        int _beatCount;
        int _currentPatternIndex;
        public List<PatternBase> Patterns => _patternList;
        public WorldObject Owner => _owner;
        public void Init(WorldObject owner)
        {
            IsPause = false;
            _owner = owner;
            _currentPatternIndex = 0;
            _beatCount = 0;
            if (null != Patterns)
            {
                foreach (var pattern in Patterns)
                {
                    pattern.Init(_owner);
                }
            }
        }
        public void Play()
        {
            IsPause = false;
        }
        public void Pause()
        {
            IsPause = true;
        }
        public void OnBeatTime(int beatIndex)
        {
            if (IsPause)
                return;

            ++_beatCount;
            PatternBase currentPattern = GetCurrentPattern();
            if(null != currentPattern)
            {
                if(_beatCount >= currentPattern.WaitBeatCount)
                {
                    currentPattern.Execute();
                    MoveToNextPattern();
                }
            }
        }
        public PatternBase GetPattern(int index)
        {
            if (null == _patternList || index >= _patternList.Count)
                return null; 
            return _patternList[index];
        }
        public void DoRollback()
        {
            --_currentPatternIndex;
            if (_currentPatternIndex < 0)
                _currentPatternIndex = _patternList.Count - 1;
        }
        public void PlayPattern(PatternBase pattern)
        {
            pattern.Init(Owner);
            pattern.Execute();
        }
        void MoveToNextPattern()
        {
            _beatCount = 0;
            _currentPatternIndex++;
            if (_currentPatternIndex >= _patternList.Count)
            {
                _currentPatternIndex = 0;
            }
        }
        PatternBase GetCurrentPattern()
        {
            return GetPattern(_currentPatternIndex);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _owner = GetComponent<WorldObject>();      
        }
        public void AddPattern(Type patternType)
        {
            PatternBase newPattern = gameObject.AddComponent(patternType) as PatternBase;
            newPattern.hideFlags = HideFlags.HideInInspector;
            newPattern.SetName(patternType.Name);

            if (null == _patternList)
                _patternList = new List<PatternBase>();

            _patternList.Add(newPattern);
        }
        public void RemovePattern(int index)
        {
            UnityEditor.Editor.DestroyImmediate(_patternList[index]);
            _patternList.RemoveAt(index);
        }
#endif
    }
}