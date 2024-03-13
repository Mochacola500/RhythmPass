using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening;
using System;

namespace Dev
{
    
    [Serializable]
    public class FieldObjectEvent
    {
        public enum EventType
        {
            PlayAnimation,
            Instantiate
        }

        [Serializable]
        public struct EventInfo
        {
            public EventType Type;
            public List<GameObject> Assets;
        }
        
        [SerializeField] private List<EventInfo> _eventInfos;
        public void Excute(WorldObject onwer)
        {
            if (null == onwer)
                return;

            for (int i = 0; i < _eventInfos.Count; i++)
            {
                var info = _eventInfos[i];
                switch (info.Type)
                {
                    case EventType.PlayAnimation:
                        {

                        }
                        break;
                    case EventType.Instantiate:
                        {

                        }
                        break;
                }
            }
        }

    }

    //환경
    public class FieldObject : WorldObject
    {
        [SerializeField] private FieldObjectEvent _fieldObjectEvent;
        [SerializeField] private long _interval = 3000;
        private long _lastExcuteTime;

        public void Start()
        {
            
        }
        
        public override void OnTouchObject()
        {
            Debug.Log("ㅅ싱싯ㅇㅅ이시잇잇이");
            var currentTime = Game.GameTime.GetClientLocalTime();
            var delta = currentTime - _lastExcuteTime;
            if(_interval <= delta)
            {
                _fieldObjectEvent.Excute(this);
                _lastExcuteTime = currentTime;
            }                        
        }

        
    }
}