using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class TimerHandler
    {
        public int ID { get; private set; }
        public long EndTime { get; private set; }
        public Action<TimerHandler> CallbackTimerComplete { get; private set; }
        public TimerHandler(long endTimeVerverUTC, Action<TimerHandler> callbackTimerComplete)
        {
            EndTime = endTimeVerverUTC;
            CallbackTimerComplete = callbackTimerComplete;
        }
        public void SetID(int id)
        {
            ID = id;
        }
        public bool IsComplete()
        {
            return EndTime <= Game.GameTime.GetClientLocalTime();
        }
        public void Destroy()
        {
            Game.TimerManager.DestroyTimer(ID);
        }
    }
    /// <summary>
    /// 타이머들을 항상 EndTime 내림차순으로 정렬해 관리합니다.
    /// 항상 List의 마지막에 있는 핸들러들이 가장 먼저 끝나는 타이머 입니다. 
    /// </summary>
    public class TimerManager : MonoBehaviour
    {
        class TimerHandlerGroup
        {
            public long EndTimeServerUTC { get; private set; }
            public List<TimerHandler> TimerList { get; private set; } = new List<TimerHandler>();
            public TimerHandlerGroup(long endTimeServerUTC)
            {
                EndTimeServerUTC = endTimeServerUTC;
            }
            public bool IsComplete()
            {
                return EndTimeServerUTC <= Game.GameTime.GetClientLocalTime();
            }
            public TimerHandler CreateTimerHandler(Action<TimerHandler> callbackTimerComplete)
            {
                TimerHandler handler = new TimerHandler(EndTimeServerUTC, callbackTimerComplete);
                handler.SetID(handler.GetHashCode());
                TimerList.Add(handler);
                return handler;
            }
            public bool RemoveTimer(int id)
            {
                for (int i = 0; i < TimerList.Count; ++i)
                {
                    if (TimerList[i].ID == id)
                    {
                        TimerList.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            public void ExecuteNoti()
            {
                foreach (var timer in TimerList)
                {
                    timer.CallbackTimerComplete?.Invoke(timer);
                }
            }
        }

        readonly List<TimerHandlerGroup> _timerContainer = new List<TimerHandlerGroup>();
        WaitForSeconds _waitForSeconds = new WaitForSeconds(0.1f);
        Coroutine _coroutine;
#if UNITY_EDITOR
        public int TimerGroupCount;
        public int TotalTimerCount;

        private void FixedUpdate()
        {
            TimerGroupCount = _timerContainer.Count;
            TotalTimerCount = 0;
            foreach (var group in _timerContainer)
                TotalTimerCount += group.TimerList.Count;
        }
#endif
        public TimerHandler CreateTimer(long endTimeServerUTC, Action<TimerHandler> callbackCompleteTimer)
        {
            if (endTimeServerUTC <= Game.GameTime.GetClientLocalTime())
            {
                Debug.LogErrorFormat("TimerManager::CreateTimer endTime is already pass EndTime : {0}   , Time : {1}", endTimeServerUTC, UtilTime.TimeStampToDateTime(endTimeServerUTC).ToString());
                return null;
            }

            TimerHandlerGroup group = GetTimerHandlerGroup(endTimeServerUTC);
            if (group == null)
            {
                group = CreateNewTimerHandlerGroup(endTimeServerUTC);
                AddTimerHanlderGroup(group);
            }

            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(CoroutineCheckTimer());
            }

            return group.CreateTimerHandler(callbackCompleteTimer);
        }
        public void DestroyTimer(int id)
        {
            for (int i = 0; i < _timerContainer.Count; ++i)
            {
                if (_timerContainer[i].RemoveTimer(id) == true)
                {
                    if (_timerContainer[i].TimerList.Count == 0)
                    {
                        RemoveTimerHandlerGroup(i);
                        return;
                    }
                }
            }
        }
        public void SetTimerCheckDelay(float delayTime)
        {
            _waitForSeconds = new WaitForSeconds(delayTime);
        }
        void AddTimerHanlderGroup(TimerHandlerGroup timerHandlerGroup)
        {
            _timerContainer.Add(timerHandlerGroup);
            SortTimerGroup();
        }
        void RemoveTimerHandlerGroup(int index)
        {
            if (_timerContainer.Count <= index)
                return;
            _timerContainer.RemoveAt(index);
            SortTimerGroup();
        }
        TimerHandlerGroup GetTimerHandlerGroup(long endServerUTCTime)
        {
            foreach (var timerHandlerGroup in _timerContainer)
            {
                if (timerHandlerGroup.EndTimeServerUTC == endServerUTCTime)
                {
                    return timerHandlerGroup;
                }
            }
            return null;
        }
        TimerHandlerGroup CreateNewTimerHandlerGroup(long endServerUTCTime)
        {
            return new TimerHandlerGroup(endServerUTCTime);
        }
        void SortTimerGroup()
        {
            _timerContainer.Sort((left, right) =>
            {
                return right.EndTimeServerUTC.CompareTo(left.EndTimeServerUTC);
            });
        }
        IEnumerator CoroutineCheckTimer()
        {
            while (_timerContainer.Count != 0)
            {
                TimerHandlerGroup group = _timerContainer[_timerContainer.Count - 1];
                if (group.IsComplete())
                {
                    group.ExecuteNoti();
                    RemoveTimerHandlerGroup(_timerContainer.Count - 1);
                }
                yield return _waitForSeconds;
            }
            _coroutine = null;
        }
    }
}
