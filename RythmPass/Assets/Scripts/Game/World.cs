using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Sound;
    using Data;
    using UI;
    public class World : IGameMessageReceiver
    {
        public Stage CurrentStage { get; private set; }
        public void Init(int stageID)
        {
            LoadStage(stageID);
        }
        public void Update()
        {
            if (null != CurrentStage)
                CurrentStage.Update();
        }
        public void LoadStage(int stageID)
        {
            Game.Instance.StartCoroutine(CoroutineLoadStage(stageID));
        }
        IEnumerator CoroutineLoadStage(int stageID)
        {
            if (null != CurrentStage)
            {
                CurrentStage.Release();
                CurrentStage = null;
            }

            yield return new WaitForSeconds(1f);

            CurrentStage = InstantiateStage(stageID);
            CurrentStage.Init(stageID);

            Game.Instance.SendGameMessage(GameMessageEnum.ChangedStage, new GameMessage.ChangedStage()
            {
                StageID = stageID
            });
        }

        Stage InstantiateStage(int stageID)
        {
            StageInfo stageInfo = Game.StageManager.GetStageInfo(stageID);
            if (null == stageInfo)
                return null;

            if (stageInfo.IsTutorialStage() && false == Game.LocalData.IsTutorialClear)
                return new TutorialStage();
            else
                return new Stage();
        }
        public void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            if (null != CurrentStage)
                CurrentStage.ProcessGameMessage(messageName, message);
        }
    }
}
