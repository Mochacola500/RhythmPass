using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace Dev.UI
{
    using Sound;
    public class BeatUI : ManagementUIBase
    {
        [SerializeField] double _nodeSpeed = 300;  //이후 수정
        [SerializeField] RectTransform _nodeParent;
        [SerializeField] BeatNodeUI _prefab;
        [SerializeField] RectTransform _arrivalPointTransform;
        readonly List<BeatNodeUI> _nodeList = new List<BeatNodeUI>();
        int _lastStanbyBeatIndex;
        public void Init()
        {
            _lastStanbyBeatIndex = 3;
            for(int i =0; i < 5; ++i)
            {
                InitNode();
            }
        }

        void InitNode()
        {
            if (false == Game.World.CurrentStage.BeatController.TryGetBeat(_lastStanbyBeatIndex, out long beat))
                return;

            double leftTime = (double)(beat - Game.World.CurrentStage.BeatController.GetCurrentTime()) * 0.001;
            double distance = leftTime * _nodeSpeed;
            Vector2 startPoint = _arrivalPointTransform.anchoredPosition + Vector2.right * (float)distance;
            GetSleepNode().Init(Game.World.CurrentStage.BeatController.GetCurrentTime(), beat, startPoint,_arrivalPointTransform.anchoredPosition);
            ++_lastStanbyBeatIndex;
        }

        private void Update()
        {
            if (false == Game.World.CurrentStage.IsPlaying)
                return;

            foreach(var node in _nodeList)
            {
                if (node.gameObject.activeSelf)
                    node.UpdateNode();
            }
        }
        BeatNodeUI GetSleepNode()
        {
            foreach(var node in _nodeList)
            {
                if (node.gameObject.activeSelf)
                    continue;
                return node;
            }

            BeatNodeUI newNode = Instantiate<BeatNodeUI>(_prefab, _nodeParent);
            _nodeList.Add(newNode);
            return newNode;
        }

        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch((GameMessageEnum)messageName)
            {
                case GameMessageEnum.BeatTime:
                    {
                        InitNode();
                        Game.UIManager.BounceUI(_arrivalPointTransform);
                    }
                    break;
            }
        }
    }
}
