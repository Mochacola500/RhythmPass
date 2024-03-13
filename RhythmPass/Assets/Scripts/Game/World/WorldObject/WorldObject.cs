using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Dev
{
    public abstract class WorldObject : MonoBehaviour , IGameMessageReceiver
    {
        [Header("WorldObject")]
        [SerializeField,FitPosition] protected Vector2Int _index;
        [SerializeField] protected WorldObjectResource _worldObjectResource;
        [SerializeField] protected PatternController _patternController;
        public Vector2Int Index => _index;
        public WorldObjectResource WorldObjectResource => _worldObjectResource;
#if UNITY_EDITOR
        void OnValidate()
        {
            _patternController = GetComponent<PatternController>();
        }
#endif
        public virtual void Init()
        {
            SetIndex(_index);
            if(null != _patternController)
                _patternController.Init(this);
        }
        public virtual void SetIndex(Vector2Int index)
        {
            _index = index;
        }
        //박자 타이밍
        protected virtual void OnBeatTime(long beatTime, int beatIndex)
        {
            if(false == Game.World.CurrentStage.IsDragging)
                if(null != _patternController)
                    _patternController.OnBeatTime(beatIndex);
        }
        public virtual void OnTouchObject()
        {

        }
        public void ActiveRender(bool isActive)
        {
            if (null != _worldObjectResource)
                _worldObjectResource.ActiveRender(isActive);
        }
        public virtual void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch((GameMessageEnum)messageName)
            {
                case GameMessageEnum.BeatTime:
                    {
                        GameMessage.BeatTime beatTimeMessage = message.Cast<GameMessage.BeatTime>();
                        OnBeatTime(beatTimeMessage.NodeTime, beatTimeMessage.Index);
                    }
                    break;
            }
        }
        public virtual void FitPosition()
        {
            var height = transform.position.y;
            transform.position = new Vector3(Index.x, height, Index.y);
        }

        public virtual void PlayVfx(GameObject instnace)
        {

        }

        public virtual void PlayVfxToTarget(GameObject instnace)
        {

        }
    }


}